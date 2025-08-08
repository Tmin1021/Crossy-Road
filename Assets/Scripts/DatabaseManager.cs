using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using System;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    
    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
            Debug.Log("[DatabaseManager] Instance created and set to persist across scenes");
        }
        else
        {
            Debug.Log("[DatabaseManager] Duplicate instance found, destroying...");
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (auth == null || dbRef == null)
        {
            InitializeFirebase();
        }
    }

    void InitializeFirebase()
    {
        try
        {
            auth = FirebaseAuth.DefaultInstance;
            string databaseURL = "https://tmin-crossy-road-default-rtdb.asia-southeast1.firebasedatabase.app";
            dbRef = FirebaseDatabase.GetInstance(databaseURL).RootReference;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[DatabaseManager] Firebase initialization failed: " + e.Message);
        }
    }

    private string GetCurrentUserId()
    {
        if (auth == null)
        {
            InitializeFirebase();
            if (auth == null)
            {
                return null;
            }
        }
        
        if (auth.CurrentUser != null)
        {
            return auth.CurrentUser.UserId;
        }
        
        return null;
    }

    private void LogToFile(string message)
    {
        try
        {
            string logPath = Path.Combine(Application.persistentDataPath, "scoreboard_debug.log");
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] [DatabaseManager] {message}\n";
            File.AppendAllText(logPath, logMessage);
        }
        catch { /* Ignore file errors */ }
    }

    public bool IsUserAuthenticated()
    {
        try
        {
            if (auth == null)
            {
                return false;
            }
            
            return auth.CurrentUser != null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[DatabaseManager] Exception in IsUserAuthenticated(): " + e.Message);
            return false;
        }
    }

    public string GetCurrentUserEmail()
    {;
        return auth.CurrentUser?.Email ?? "Unknown";
    }

    public string GetCurrentUserDisplayName()
    {
        return auth.CurrentUser?.DisplayName ?? auth.CurrentUser?.Email ?? "Unknown User";
    }

    public void SaveUserData(int coins, int score)
    {
        string userId = GetCurrentUserId();
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("Cannot save user data: No authenticated user");
            return;
        }

        DatabaseReference userRef = dbRef.Child("user").Child(userId);

        userRef.Child("coins").SetValueAsync(coins);

        DatabaseReference scoreHistoryRef = userRef.Child("scoreHistory");
        string key = scoreHistoryRef.Push().Key;
        scoreHistoryRef.Child(key).SetValueAsync(score);
        
        Debug.Log($"Saved score {score} and coins {coins} for user: {userId}");
    }

    public void ReadUserScoreHistory(Action<List<int>> onScoresLoaded, Action<string> onError = null)
    {
        string userId = GetCurrentUserId();
        
        if (string.IsNullOrEmpty(userId))
        {
            onError?.Invoke("Unable to get user ID");
            return;
        }

        DatabaseReference userRef = dbRef.Child("user").Child(userId);
        
        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                string errorMessage = task.Exception?.GetBaseException().Message ?? "Unknown error occurred";
                onError?.Invoke(errorMessage);
                return;
            }

            DatabaseReference scoreHistoryRef = dbRef.Child("user").Child(userId).Child("scoreHistory");

            scoreHistoryRef.GetValueAsync().ContinueWithOnMainThread(scoreTask =>
            {
                if (scoreTask.IsFaulted || scoreTask.IsCanceled)
                {
                    string errorMessage = scoreTask.Exception?.GetBaseException().Message ?? "Unknown error occurred";
                    onError?.Invoke(errorMessage);
                    return;
                }

                DataSnapshot snapshot = scoreTask.Result;
                List<int> scores = new List<int>();

                if (snapshot.Exists)
                {
                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        if (child.Value != null && int.TryParse(child.Value.ToString(), out int score))
                        {
                            scores.Add(score);
                        }
                    }
                }
                
                scores.Sort((a, b) => b.CompareTo(a));
                onScoresLoaded?.Invoke(scores);
            });
        });
    }

    public void ReadUserCoins(Action<int> onCoinsLoaded, Action<string> onError = null)
    {
        string userId = GetCurrentUserId();
        
        if (string.IsNullOrEmpty(userId))
        {
            onError?.Invoke("Unable to get user ID");
            return;
        }

        DatabaseReference coinsRef = dbRef.Child("user").Child(userId).Child("coins");

        coinsRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                string errorMessage = task.Exception?.GetBaseException().Message ?? "Unknown error occurred";
                Debug.LogError("Failed to read coins: " + errorMessage);
                onError?.Invoke(errorMessage);
                return;
            }

            DataSnapshot snapshot = task.Result;
            int coins = 0;

            if (snapshot.Exists && snapshot.Value != null)
            {
                int.TryParse(snapshot.Value.ToString(), out coins);
            }

            Debug.Log($"Loaded {coins} coins for user: {userId}");
            onCoinsLoaded?.Invoke(coins);
        });
    }

    public void GetCoinsWithFallback(System.Action<int, string> onComplete)
    {
        if (IsUserAuthenticated())
        {
            ReadUserCoins(
                onCoinsLoaded: (coins) =>
                {
                    onComplete?.Invoke(coins, "Firebase");
                },
                onError: (error) =>
                {
                    Debug.LogError($"[DatabaseManager] Firebase error: {error}");
                    GetCoinsFromLocal(onComplete);
                }
            );
        }
        else
        {
            GetCoinsFromLocal(onComplete);
        }
    }
    
    private void GetCoinsFromLocal(System.Action<int, string> onComplete)
    {
        int localCoins = PlayerPrefs.GetInt("PlayerCoins", 80);
        onComplete?.Invoke(localCoins, "Local");
    }
    
    public void SaveCoinsOnly(int coins, System.Action<bool> onComplete = null)
    {
        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
        
        if (IsUserAuthenticated())
        {
            string userId = GetCurrentUserId();
            
            if (!string.IsNullOrEmpty(userId))
            {
                DatabaseReference userRef = dbRef.Child("user").Child(userId);
                
                userRef.Child("coins").SetValueAsync(coins).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        onComplete?.Invoke(true);
                    }
                    else
                    {
                        Debug.LogError($"[DatabaseManager] Failed to save coins to Firebase: {task.Exception?.GetBaseException().Message}");
                        onComplete?.Invoke(false);
                    }
                });
            }
            else
            {
                Debug.LogError("[DatabaseManager] Cannot save to Firebase: No user ID");
                onComplete?.Invoke(false);
            }
        }
        else
        {
            onComplete?.Invoke(true);
        }
    }
    
    // New method: Sync coins when user signs in
    public void SyncCoinsOnSignIn()
    {
        if (IsUserAuthenticated())
        {
            // Get coins from Firebase and sync with local
            GetCoinsWithFallback((firebaseCoins, source) =>
            {
                int localCoins = PlayerPrefs.GetInt("PlayerCoins", 80);
                
                if (source == "Firebase")
                {
                    // Use the higher value (in case user earned coins offline)
                    int syncedCoins = Mathf.Max(firebaseCoins, localCoins);
                    
                    if (syncedCoins != firebaseCoins)
                    {
                        SaveCoinsOnly(syncedCoins);
                    }
                    else if (syncedCoins != localCoins)
                    {
                        PlayerPrefs.SetInt("PlayerCoins", syncedCoins);
                        PlayerPrefs.Save();
                    }
                    
                    // Notify UI to refresh
                    CharacterSelectionController selectionController = FindObjectOfType<CharacterSelectionController>();
                    if (selectionController != null)
                    {
                        selectionController.RefreshCoinsDisplay();
                    }
                }
            });
        }
    }

}
