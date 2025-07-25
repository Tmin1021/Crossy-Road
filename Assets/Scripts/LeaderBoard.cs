using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;  // For button interaction

public class LeaderboardLoader : MonoBehaviour
{
    public TextMeshProUGUI[] playerNames;  // Array of 5 Texts for player names
    public TextMeshProUGUI[] playerScores; // Array of 5 Texts for scores

    public Button[] playerNameButtons;  // Array of Buttons for player names
    public Button[] playerScoreButtons; // Array of Buttons for player scores
    public Button leftButton;           // Left arrow button
    public Button rightButton;          // Right arrow button

    public Image difficultyImage;       // Image holder for difficulty (for showing difficulty state)
    public Sprite easySprite, mediumSprite, hardSprite;  // Difficulty images

    public string difficulty = "easy";  // Default difficulty (easy)

    void Start()
    {
        LoadLeaderboard(difficulty);

        // Add listeners for difficulty buttons
        leftButton.onClick.AddListener(() => ChangeDifficulty(GetPreviousDifficulty()));
        rightButton.onClick.AddListener(() => ChangeDifficulty(GetNextDifficulty()));

        // Setup player name buttons (if needed)
        for (int i = 0; i < playerNameButtons.Length; i++)
        {
            int index = i; // Local copy of i for closure
            playerNameButtons[i].onClick.AddListener(() => OnPlayerNameClick(index));
        }

        // Setup score buttons if needed
        for (int i = 0; i < playerScoreButtons.Length; i++)
        {
            int index = i; // Local copy of i for closure
            playerScoreButtons[i].onClick.AddListener(() => OnPlayerScoreClick(index));
        }
    }

    void LoadLeaderboard(string difficultyLevel)
    {
        string fileName = $"{difficultyLevel}_board.txt";
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError("Leaderboard file not found at: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);
        List<Player> players = new List<Player>();

        // Read player data from the file into a list of Player objects
        for (int i = 0; i < lines.Length; i += 2)
        {
            string name = lines[i];
            int score;
            if (int.TryParse(lines[i + 1], out score))
            {
                players.Add(new Player(name, score));
            }
        }

        // Sort the list by score in descending order
        players.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));

        // Display the top 5 players
        for (int i = 0; i < Mathf.Min(5, players.Count); i++)
        {
            playerNames[i].text = players[i].Name;
            playerScores[i].text = players[i].Score.ToString();
        }
    }

    // Change difficulty function
    void ChangeDifficulty(string newDifficulty)
    {
        if (difficulty != newDifficulty)
        {
            difficulty = newDifficulty;
            LoadLeaderboard(difficulty);

            // Update the difficulty image based on the selection
            switch (difficulty)
            {
                case "easy":
                    difficultyImage.sprite = easySprite;
                    break;
                case "medium":
                    difficultyImage.sprite = mediumSprite;
                    break;
                case "hard":
                    difficultyImage.sprite = hardSprite;
                    break;
            }
        }
    }

    // Get the previous difficulty (left button)
    string GetPreviousDifficulty()
    {
        switch (difficulty)
        {
            case "medium": return "easy";
            case "hard": return "medium";
            default: return "easy"; // No previous difficulty if on easy
        }
    }

    // Get the next difficulty (right button)
    string GetNextDifficulty()
    {
        switch (difficulty)
        {
            case "easy": return "medium";
            case "medium": return "hard";
            default: return "hard"; // No next difficulty if on hard
        }
    }

    // Handle player name click
    void OnPlayerNameClick(int index)
    {
        Debug.Log("Player " + (index + 1) + " clicked!");
    }

    // Handle player score click
    void OnPlayerScoreClick(int index)
    {
        Debug.Log("Score " + (index + 1) + " clicked!");
    }
}

// Player class to store name and score
public class Player
{
    public string Name { get; set; }
    public int Score { get; set; }

    public Player(string name, int score)
    {
        Name = name;
        Score = score;
    }
}
