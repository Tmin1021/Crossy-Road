using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int gameMode;
    public bool isTwoPlayerMode;

    public int player1CharacterIndex;
    public int player2CharacterIndex;
    public int currentScore;
    public int currentCoins;
    public Vector3 player1Position;
    public Vector3 player2Position;
    public float lastLaneY;

    public Vector3 cameraPosition;
    public float lastSpawnY;
    public int currentLaneCount;

    public string saveTimestamp;
    public float playTime;

    public float volume;

    public GameSaveData()
    {
        saveTimestamp = System.DateTime.Now.ToBinary().ToString();
    }
}