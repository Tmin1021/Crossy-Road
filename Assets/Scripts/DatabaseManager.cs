using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{
    private DatabaseReference dbRef;
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveUserData(int coins, int score)
    {
        string userId = auth.CurrentUser.UserId;

        // Reference to the user's root node
        DatabaseReference userRef = dbRef.Child("users").Child(userId);

        // Save coins at a fixed location
        userRef.Child("coins").SetValueAsync(coins);

        // Save score with unique key (history)
        DatabaseReference scoreHistoryRef = userRef.Child("scoreHistory");
        string key = scoreHistoryRef.Push().Key;
        scoreHistoryRef.Child(key).SetValueAsync(score);
    }

}
