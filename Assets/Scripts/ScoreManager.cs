using UnityEngine;
using UnityEngine.UI;  // Make sure to include this if you're using UI Text

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;  // Reference to the UI Text element
    private int score;

    void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
