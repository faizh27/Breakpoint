using UnityEngine;

/// <summary>
/// Manages game state: serves, scoring (tennis format 0/15/30/40/game),
/// rally detection, win conditions. Interfaces with Player, AIOpponent, and BallController.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BallController ball;
    [SerializeField] private PlayerController player;
    [SerializeField] private AIOpponent opponent;
    [SerializeField] private CourtSystem court;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private Transform playerServePosition;
    [SerializeField] private Transform opponentServePosition;

    // Tennis scoring: 0, 15, 30, 40, game
    private int playerPoints = 0;
    private int opponentPoints = 0;

    private bool isServing = true;
    private bool playerServingNext = true;
    private bool rallyActive = false;

    private float ballOutOfBoundsTimeout = 2f;
    private float ballOutOfBoundsTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (playerServePosition == null)
            playerServePosition = player.transform;
        if (opponentServePosition == null)
            opponentServePosition = opponent.transform;

        StartServe();
    }

    private void Update()
    {
        if (!rallyActive && !isServing) return;

        // Check if ball is out of bounds for too long (player didn't hit it)
        if (rallyActive && !ball.IsMoving)
        {
            ballOutOfBoundsTimer += Time.deltaTime;
            if (ballOutOfBoundsTimer > ballOutOfBoundsTimeout)
            {
                EndRally(ballOutOfBoundsTimer > 0.5f); // other player wins
            }
        }
        else
        {
            ballOutOfBoundsTimer = 0f;
        }

        // Check if ball goes out of court bounds
        if (rallyActive && ball.IsMoving)
        {
            if (!court.IsInBounds(ball.GroundPosition))
            {
                EndRally(false); // hitter loses
            }
        }
    }

    public void StartServe()
    {
        isServing = true;
        rallyActive = false;
        ballOutOfBoundsTimer = 0f;

        Vector2 servePos = playerServingNext ? playerServePosition.position : opponentServePosition.position;
        ball.ResetBall(servePos);

        if (playerServingNext)
        {
            player.AllowServe();
        }
        else
        {
            opponent.ServeTowardOpponent(player.transform.position);
            isServing = false;
            rallyActive = true;
        }
    }

    public void OnPlayerServed()
    {
        isServing = false;
        rallyActive = true;
    }

    /// <summary>Called when rally ends (someone failed to return, ball went out, etc).</summary>
    public void EndRally(bool opponentWins)
    {
        rallyActive = false;

        if (opponentWins)
        {
            opponentPoints += 1;
        }
        else
        {
            playerPoints += 1;
        }

        uiManager.UpdateScore(playerPoints, opponentPoints);

        // Check win condition (first to 4 points, win by 2)
        if (playerPoints >= 4 && playerPoints - opponentPoints >= 2)
        {
            OnGameWon(true);
            return;
        }
        if (opponentPoints >= 4 && opponentPoints - playerPoints >= 2)
        {
            OnGameWon(false);
            return;
        }

        // Next serve
        playerServingNext = !playerServingNext;
        Invoke(nameof(StartServe), 1.5f);
    }

    private void OnGameWon(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Opponent wins!");
        uiManager.ShowGameOver(playerWon);
        // TODO: add restart logic
    }
}
