using UnityEngine;
using UnityEngine.UI; 

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; 
    private int score;
    void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        
    
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
