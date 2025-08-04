using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public void GetTop10Scores()
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .OrderByChild("score")
            .LimitToLast(10)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Failed to get leaderboard: " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;

                List<(string uid, int score)> leaderboard = new List<(string, int)>();

                foreach (DataSnapshot child in snapshot.Children)
                {
                    string uid = child.Key;
                    int score = int.Parse(child.Child("score").Value.ToString());
                    leaderboard.Add((uid, score));
                }

                leaderboard.Reverse(); // Optional: to make top score first

                foreach (var entry in leaderboard)
                {
                    Debug.Log("User: " + entry.uid + " | Score: " + entry.score);
                }
            });
    }
}
