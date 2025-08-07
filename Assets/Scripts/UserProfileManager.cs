using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class UserProfileManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI emailText;
    public Button logoutButton;
    public Button deleteAccountButton;
    
    [Header("Settings")]
    public string loginSceneName = "LoginScene";
    
    private FirebaseAuth auth;
    private DatabaseManager databaseManager;
    
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseManager = FindObjectOfType<DatabaseManager>();
        
        SetupUI();
        DisplayUserInfo();
    }
    
    void SetupUI()
    {
        if (logoutButton != null)
            logoutButton.onClick.AddListener(LogoutUser);
            
        if (deleteAccountButton != null)
            deleteAccountButton.onClick.AddListener(ConfirmDeleteAccount);
    }
    
    void DisplayUserInfo()
    {
        if (auth.CurrentUser != null)
        {
            FirebaseUser user = auth.CurrentUser;
            
            if (usernameText != null)
            {
                string displayName = user.DisplayName;
                if (string.IsNullOrEmpty(displayName))
                    displayName = "User";
                usernameText.text = displayName;
            }
            
            if (emailText != null)
            {
                emailText.text = user.Email ?? "No email";
            }
        }
        else if (databaseManager != null && databaseManager.IsUserAuthenticated())
        {
            // Using hardcoded mode
            if (usernameText != null)
                usernameText.text = databaseManager.GetCurrentUserDisplayName();
                
            if (emailText != null)
                emailText.text = databaseManager.GetCurrentUserEmail();
        }
        else
        {
            if (usernameText != null)
                usernameText.text = "Not logged in";
                
            if (emailText != null)
                emailText.text = "";
        }
    }
    
    public void LogoutUser()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("User signed out");
        }
        
        // Redirect to login scene
        SceneManager.LoadScene(loginSceneName);
    }
    
    public void ConfirmDeleteAccount()
    {
        // Show confirmation dialog (you can implement a proper dialog)
        if (Application.isEditor)
        {
            bool confirm = UnityEditor.EditorUtility.DisplayDialog(
                "Delete Account",
                "Are you sure you want to delete your account? This action cannot be undone.",
                "Delete",
                "Cancel"
            );
            
            if (confirm)
            {
                DeleteAccount();
            }
        }
        else
        {
            // In build, you'd want to implement a proper in-game confirmation dialog
            Debug.LogWarning("Account deletion requires confirmation. Implement proper dialog for builds.");
        }
    }
    
    void DeleteAccount()
    {
        if (auth.CurrentUser != null)
        {
            FirebaseUser user = auth.CurrentUser;
            
            user.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled) 
                {
                    Debug.LogError("Account deletion was cancelled.");
                    return;
                }
                if (task.IsFaulted) 
                {
                    Debug.LogError("Account deletion encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User account deleted successfully.");
                
                // Redirect to login scene on main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    SceneManager.LoadScene(loginSceneName);
                });
            });
        }
    }
    
    // Method to update profile display (call after profile changes)
    public void RefreshUserInfo()
    {
        DisplayUserInfo();
    }
}

// Simple main thread dispatcher for Firebase callbacks
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private System.Collections.Generic.Queue<System.Action> _executionQueue = new System.Collections.Generic.Queue<System.Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            GameObject go = new GameObject("MainThreadDispatcher");
            _instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
        return _instance;
    }

    public void Enqueue(System.Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}
