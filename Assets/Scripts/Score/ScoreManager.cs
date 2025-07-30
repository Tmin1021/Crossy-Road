using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PlayerScoreData
{
    public int playerID;
    public int currentScore;
    public int bestScore;
    public string playerName;
    public DateTime lastPlayedDate;
    public int gamesPlayed;
    public GameDifficulty lastDifficulty;
    
    public PlayerScoreData(int id, string name = "")
    {
        playerID = id;
        playerName = string.IsNullOrEmpty(name) ? $"Player{id}" : name;
        currentScore = 0;
        bestScore = 0;
        lastPlayedDate = DateTime.Now;
        gamesPlayed = 0;
        lastDifficulty = GameDifficulty.Medium;
    }
}

[System.Serializable]
public class GameSession
{
    public string sessionID;
    public DateTime startTime;
    public DateTime endTime;
    public GameDifficulty difficulty;
    public int gameMode;
    public List<PlayerScoreData> playerScores;
    public int winner;
    
    public GameSession()
    {
        sessionID = System.Guid.NewGuid().ToString();
        startTime = DateTime.Now;
        playerScores = new List<PlayerScoreData>();
        winner = 0;
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [Header("Score Settings")]
    public int scorePerStep = 10;
    
    private Dictionary<int, PlayerScoreData> playerData = new Dictionary<int, PlayerScoreData>();
    private GameSession currentSession;
    private List<GameSession> gameHistory = new List<GameSession>();
    
    public System.Action<int, int> OnScoreChanged;
    public System.Action<int, int> OnNewBestScore;
    public System.Action<GameSession> OnGameSessionComplete; 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        InitializeScores();
        StartNewGameSession();
    }
    
    void InitializeScores()
    {
        LoadPlayerData(1);
        LoadPlayerData(2);
        
        Debug.Log($"ScoreManager: Initialized - P1: {playerData[1].currentScore} (Best: {playerData[1].bestScore}), P2: {playerData[2].currentScore} (Best: {playerData[2].bestScore})");
        
        OnScoreChanged?.Invoke(1, playerData[1].currentScore);
        OnScoreChanged?.Invoke(2, playerData[2].currentScore);
    }
    
    void LoadPlayerData(int playerID)
    {
        string playerName = PlayerPrefs.GetString($"Player{playerID}Name", $"Player{playerID}");
        int bestScore = PlayerPrefs.GetInt($"Player{playerID}BestScore", 0);
        int gamesPlayed = PlayerPrefs.GetInt($"Player{playerID}GamesPlayed", 0);
        
        PlayerScoreData data = new PlayerScoreData(playerID, playerName);
        data.bestScore = bestScore;
        data.gamesPlayed = gamesPlayed;
        data.currentScore = 0;
        
        playerData[playerID] = data;
    }
    
    void StartNewGameSession()
    {
        currentSession = new GameSession();
        currentSession.difficulty = GameModeManager.Instance != null ? 
            GameModeManager.Instance.currentDifficulty : GameDifficulty.Medium;
        currentSession.gameMode = PlayerPrefs.GetInt("SelectedGameMode", 1);
        
        foreach (var kvp in playerData)
        {
            currentSession.playerScores.Add(new PlayerScoreData(kvp.Key, kvp.Value.playerName));
        }
    }
    
    public void AddScore(int playerID, int points, string reason = "")
    {
        if (!playerData.ContainsKey(playerID))
        {
            LoadPlayerData(playerID);
        }
        
        int finalPoints = points;
        if (GameModeManager.Instance != null)
        {
            finalPoints = points * GameModeManager.Instance.GetScoreMultiplier();
        }
        
        playerData[playerID].currentScore += finalPoints;
        
        var sessionPlayer = currentSession.playerScores.Find(p => p.playerID == playerID);
        if (sessionPlayer != null)
        {
            sessionPlayer.currentScore += finalPoints;
        }
        
        Debug.Log($"ScoreManager: Player {playerID} scored {finalPoints} points (base: {points}) for {reason}. Total: {playerData[playerID].currentScore}");
        
        OnScoreChanged?.Invoke(playerID, playerData[playerID].currentScore);
        
        if (playerData[playerID].currentScore > playerData[playerID].bestScore)
        {
            playerData[playerID].bestScore = playerData[playerID].currentScore;
            SavePlayerData(playerID);
            OnNewBestScore?.Invoke(playerID, playerData[playerID].bestScore);
        }
    }
    
    void SavePlayerData(int playerID)
    {
        if (!playerData.ContainsKey(playerID)) return;
        
        var data = playerData[playerID];
        PlayerPrefs.SetString($"Player{playerID}Name", data.playerName);
        PlayerPrefs.SetInt($"Player{playerID}BestScore", data.bestScore);
        PlayerPrefs.SetInt($"Player{playerID}GamesPlayed", data.gamesPlayed);
        PlayerPrefs.Save();
    }
    
    public int GetScore(int playerID)
    {
        return playerData.ContainsKey(playerID) ? playerData[playerID].currentScore : 0;
    }
    
    public int GetBestScore(int playerID)
    {
        return playerData.ContainsKey(playerID) ? playerData[playerID].bestScore : 0;
    }
    
    public PlayerScoreData GetPlayerData(int playerID)
    {
        return playerData.ContainsKey(playerID) ? playerData[playerID] : null;
    }
    
    public void ResetScore(int playerID)
    {
        if (playerData.ContainsKey(playerID))
        {
            playerData[playerID].currentScore = 0;
            OnScoreChanged?.Invoke(playerID, 0);
        }
    }
    
    public void ResetAllScores()
    {
        foreach (var kvp in playerData)
        {
            kvp.Value.currentScore = 0;
            OnScoreChanged?.Invoke(kvp.Key, 0);
        }
    }
    
    public void OnPlayerMoveForward(int playerID)
    {
        AddScore(playerID, scorePerStep, "moving forward");
    }
    
    public void EndGameSession()
    {
        if (currentSession == null) return;
        
        currentSession.endTime = DateTime.Now;
        currentSession.winner = GetWinner();
        
        foreach (var kvp in playerData)
        {
            kvp.Value.lastPlayedDate = DateTime.Now;
            kvp.Value.gamesPlayed++;
            kvp.Value.lastDifficulty = currentSession.difficulty;
            SavePlayerData(kvp.Key);
        }
        
        gameHistory.Add(currentSession);
        SaveGameSession(currentSession);
        OnGameSessionComplete?.Invoke(currentSession);
        
        StartNewGameSession();
    }
    
    public int GetWinner()
    {
        int highestScore = -1;
        int winner = 0;
        
        foreach (var kvp in playerData)
        {
            if (kvp.Value.currentScore > highestScore)
            {
                highestScore = kvp.Value.currentScore;
                winner = kvp.Key;
            }
        }
        
        return winner;
    }
    
    public string GetFormattedScore(int playerID)
    {
        return GetScore(playerID).ToString("N0");
    }
    
    void SaveGameSession(GameSession session)
    {
        string sessionJson = JsonUtility.ToJson(session);
        string key = $"GameSession_{session.sessionID}";
        PlayerPrefs.SetString(key, sessionJson);
        
        List<string> sessionKeys = GetSessionKeys();
        sessionKeys.Add(session.sessionID);
        
        if (sessionKeys.Count > 50)
        {
            string oldestKey = sessionKeys[0];
            PlayerPrefs.DeleteKey($"GameSession_{oldestKey}");
            sessionKeys.RemoveAt(0);
        }
        
        string sessionKeysJson = JsonUtility.ToJson(new SerializableList<string>(sessionKeys));
        PlayerPrefs.SetString("GameSessionKeys", sessionKeysJson);
        PlayerPrefs.Save();
    }
    
    List<string> GetSessionKeys()
    {
        string keysJson = PlayerPrefs.GetString("GameSessionKeys", "");
        if (string.IsNullOrEmpty(keysJson))
            return new List<string>();
        
        try
        {
            var wrapper = JsonUtility.FromJson<SerializableList<string>>(keysJson);
            return wrapper.items;
        }
        catch
        {
            return new List<string>();
        }
    }
    
    public List<GameSession> GetGameHistory(int maxCount = 10)
    {
        List<GameSession> history = new List<GameSession>();
        List<string> sessionKeys = GetSessionKeys();
        
        int startIndex = Mathf.Max(0, sessionKeys.Count - maxCount);
        for (int i = startIndex; i < sessionKeys.Count; i++)
        {
            string sessionJson = PlayerPrefs.GetString($"GameSession_{sessionKeys[i]}", "");
            if (!string.IsNullOrEmpty(sessionJson))
            {
                try
                {
                    GameSession session = JsonUtility.FromJson<GameSession>(sessionJson);
                    history.Add(session);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to load game session: {e.Message}");
                }
            }
        }
        
        return history;
    }
    
    public GameSession GetCurrentSession()
    {
        return currentSession;
    }
    
    [ContextMenu("Debug Saved Scores")]
    public void DebugSavedScores()
    {
        Debug.Log("=== SAVED SCORE DATA ===");
        
        for (int i = 1; i <= 2; i++)
        {
            string name = PlayerPrefs.GetString($"Player{i}Name", $"Player{i}");
            int bestScore = PlayerPrefs.GetInt($"Player{i}BestScore", 0);
            int gamesPlayed = PlayerPrefs.GetInt($"Player{i}GamesPlayed", 0);
            
            Debug.Log($"Player {i}: {name} | Best: {bestScore} | Games: {gamesPlayed}");
        }
        
        List<string> sessionKeys = GetSessionKeys();
        Debug.Log($"Total Game Sessions Saved: {sessionKeys.Count}");
        
        if (sessionKeys.Count > 0)
        {
            Debug.Log("Recent Sessions:");
            int showCount = Mathf.Min(5, sessionKeys.Count);
            for (int i = sessionKeys.Count - showCount; i < sessionKeys.Count; i++)
            {
                string sessionJson = PlayerPrefs.GetString($"GameSession_{sessionKeys[i]}", "");
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    try
                    {
                        GameSession session = JsonUtility.FromJson<GameSession>(sessionJson);
                        Debug.Log($"Session {i+1}: Difficulty={session.difficulty}, Winner=P{session.winner}, Mode={session.gameMode}P");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to parse session: {e.Message}");
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;
    
    public SerializableList(List<T> list)
    {
        items = list;
    }
}
