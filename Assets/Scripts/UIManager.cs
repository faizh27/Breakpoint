using UnityEngine;
using TMPro;

/// <summary>
/// Handles all UI — score display, game over screen, etc.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private string[] scoreNames = { "0", "15", "30", "40" };

    private void Start()
    {
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    public void UpdateScore(int playerPoints, int opponentPoints)
    {
        string playerScore = GetScoreName(playerPoints);
        string opponentScore = GetScoreName(opponentPoints);

        if (scoreText != null)
        {
            scoreText.text = $"{playerScore} - {opponentScore}";
        }
    }

    private string GetScoreName(int points)
    {
        if (points < 4)
            return scoreNames[points];
        return "40"; // deuce/advantage shown differently if needed
    }

    public void ShowGameOver(bool playerWon)
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = playerWon ? "YOU WIN!" : "OPPONENT WINS!";
        }
    }
}
