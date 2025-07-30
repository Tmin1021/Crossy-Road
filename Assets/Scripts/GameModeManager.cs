using UnityEngine;

public enum GameDifficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;
    
    [Header("Game Mode Settings")]
    public GameDifficulty currentDifficulty = GameDifficulty.Medium;
    
    [Header("Easy Mode Settings")]
    public float easyVehicleSpeedMultiplier = 0.7f;
    public float easySpawnDelayMultiplier = 1.5f;
    public int easyScoreMultiplier = 1;
    
    [Header("Medium Mode Settings")]
    public float mediumVehicleSpeedMultiplier = 1.0f;
    public float mediumSpawnDelayMultiplier = 1.0f;
    public int mediumScoreMultiplier = 1;
    
    [Header("Hard Mode Settings")]
    public float hardVehicleSpeedMultiplier = 1.5f;
    public float hardSpawnDelayMultiplier = 0.6f;
    public int hardScoreMultiplier = 2;
    
    public System.Action<GameDifficulty> OnDifficultyChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDifficulty();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        ApplyDifficultySettings();
        Debug.Log($"GameModeManager initialized with difficulty: {currentDifficulty}");
    }
    
    public void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty;
        SaveDifficulty();
        ApplyDifficultySettings();
        OnDifficultyChanged?.Invoke(currentDifficulty);
        Debug.Log($"Game difficulty set to: {currentDifficulty}");
    }
    
    public void SetDifficulty(int difficultyIndex)
    {
        SetDifficulty((GameDifficulty)difficultyIndex);
    }
    
    void ApplyDifficultySettings()
    {
        ApplyVehicleSettings();
        ApplySpawnerSettings();
        Debug.Log($"Applied {currentDifficulty} mode settings");
    }
    
    void ApplyVehicleSettings()
    {
        VehicleMover[] vehicles = FindObjectsOfType<VehicleMover>();
        float speedMultiplier = GetVehicleSpeedMultiplier();
        
        foreach (VehicleMover vehicle in vehicles)
        {
            vehicle.ApplySpeedMultiplier(speedMultiplier);
        }
        
        Debug.Log($"Applied speed multiplier {speedMultiplier} to {vehicles.Length} vehicles");
    }
    
    void ApplySpawnerSettings()
    {
        VehicleSpawner[] spawners = FindObjectsOfType<VehicleSpawner>();
        float delayMultiplier = GetSpawnDelayMultiplier();
        
        foreach (VehicleSpawner spawner in spawners)
        {
            spawner.ApplyDelayMultiplier(delayMultiplier);
        }
        
        Debug.Log($"Applied spawn delay multiplier {delayMultiplier} to {spawners.Length} spawners");
    }
    
    public float GetVehicleSpeedMultiplier()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Easy: return easyVehicleSpeedMultiplier;
            case GameDifficulty.Medium: return mediumVehicleSpeedMultiplier;
            case GameDifficulty.Hard: return hardVehicleSpeedMultiplier;
            default: return mediumVehicleSpeedMultiplier;
        }
    }
    
    public float GetSpawnDelayMultiplier()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Easy: return easySpawnDelayMultiplier;
            case GameDifficulty.Medium: return mediumSpawnDelayMultiplier;
            case GameDifficulty.Hard: return hardSpawnDelayMultiplier;
            default: return mediumSpawnDelayMultiplier;
        }
    }
    
    public int GetScoreMultiplier()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Easy: return easyScoreMultiplier;
            case GameDifficulty.Medium: return mediumScoreMultiplier;
            case GameDifficulty.Hard: return hardScoreMultiplier;
            default: return mediumScoreMultiplier;
        }
    }
    
    public string GetDifficultyName()
    {
        return currentDifficulty.ToString();
    }
    
    public Color GetDifficultyColor()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Easy: return Color.green;
            case GameDifficulty.Medium: return Color.yellow;
            case GameDifficulty.Hard: return Color.red;
            default: return Color.white;
        }
    }
    
    void SaveDifficulty()
    {
        PlayerPrefs.SetInt("GameDifficulty", (int)currentDifficulty);
        PlayerPrefs.Save();
    }
    
    void LoadDifficulty()
    {
        int savedDifficulty = PlayerPrefs.GetInt("GameDifficulty", (int)GameDifficulty.Medium);
        currentDifficulty = (GameDifficulty)savedDifficulty;
    }
    
    public void RegisterNewVehicle(VehicleMover vehicle)
    {
        vehicle.ApplySpeedMultiplier(GetVehicleSpeedMultiplier());
    }
    
    public void RegisterNewSpawner(VehicleSpawner spawner)
    {
        spawner.ApplyDelayMultiplier(GetSpawnDelayMultiplier());
    }
}
