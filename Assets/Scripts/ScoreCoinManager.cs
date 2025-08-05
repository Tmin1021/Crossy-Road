using UnityEngine;
using UnityEngine.UI;

public class ScoreCoinManager : MonoBehaviour
{
    public Text scoreText;
    public Text coinText;
    private int score;
    private int coins;
    void Start()
    {
        score = 0;
        coins = 0; // Tmin: Connect the global variable here
        if (PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1)
        {
            gameObject.SetActive(false);
        }
        UpdateScoreText();
        UpdateCoinText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }
    public void IncreaseCoin(int amount)
    {
        coins += amount;
        UpdateCoinText();
    }

    // public void SetScore(int newScore)
    // {
    //     score = newScore;
    //     UpdateScoreText();
    // }

    public int GetCurrentScore()
    {
        return score;
    }
    public int GetCurrentCoins()
    {
        return coins;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    void UpdateCoinText()
    {
        coinText.text = " Coins: " + coins.ToString();
    }
}
