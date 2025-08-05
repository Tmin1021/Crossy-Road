using UnityEngine;
using UnityEngine.UI; 

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; 
    private int score;
    void Start()
    {
        score = 0;
        if (PlayerPrefs.GetInt("IsTwoPlayerMode", 0) == 1)
        {
            gameObject.SetActive(false);
        }
        UpdateScoreText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }
    
    public void SetScore(int newScore)
    {
        score = newScore;
        UpdateScoreText();
    }
    
    public int GetCurrentScore()
    {
        return score;
    }
    
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
