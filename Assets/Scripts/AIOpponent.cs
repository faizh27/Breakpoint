using UnityEngine;

/// <summary>
/// AI opponent for Breakpoint. Tracks the ball, moves toward it,
/// and attempts to return shots with varying arc heights and directions.
/// </summary>
public class AIOpponent : MonoBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private PlayerController player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.5f;
    [SerializeField] private float hitRange = 1.2f;

    [Header("Shot Tuning")]
    [SerializeField] private float[] returnApexMultipliers = { 0.7f, 1f, 1.3f }; // flat, normal, lob
    [SerializeField] private float returnFlightDuration = 0.8f;

    [Header("AI Behavior")]
    [SerializeField] private float readyZoneRadius = 2f; // how far to move ahead of ball's trajectory
    [SerializeField] private float reactionTime = 0.1f; // delay before committing to a hit

    private float reactionTimer = 0f;
    private bool shouldAttemptReturn = false;
    private Vector2 targetPosition;

    private void Update()
    {
        if (!ball.IsMoving)
        {
            // Ball is idle — return to center court
            MoveTowardPosition(Vector2.zero, 0.5f);
            return;
        }

        // Predict where ball will land
        targetPosition = PredictBallLandingPosition();

        // Move toward intercept point
        MoveTowardPosition(targetPosition, 1f);

        // Check if we can hit it
        float distanceToBall = Vector2.Distance(transform.position, ball.GroundPosition);
        if (distanceToBall <= hitRange)
        {
            if (reactionTimer < reactionTime)
            {
                reactionTimer += Time.deltaTime;
            }
            else
            {
                AttemptReturn();
            }
        }
        else
        {
            reactionTimer = 0f;
        }
    }

    /// <summary>Very basic prediction: assume ball lands in a straight line from its current position.</summary>
    private Vector2 PredictBallLandingPosition()
    {
        if (ball.GroundVelocity.sqrMagnitude < 0.01f)
            return ball.GroundPosition;

        // Extrapolate where ball will be when it lands (height == 0)
        // This is a rough estimate; for a full game you'd solve the full parabola
        Vector2 landingPos = ball.GroundPosition + ball.GroundVelocity * 1.5f;
        return landingPos;
    }

    private void MoveTowardPosition(Vector2 target, float speedMultiplier = 1f)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target);

        if (distance > 0.3f)
        {
            transform.position += (Vector3)direction * moveSpeed * speedMultiplier * Time.deltaTime;
        }
    }

    private void AttemptReturn()
    {
        // Pick a shot type (flat, normal, lob) based on ball height and distance
        float arcMultiplier = ChooseShotType();

        // Aim back at player's court, slightly left or right for variety
        Vector2 aimTarget = (Vector2)player.transform.position;
        aimTarget += Random.insideUnitCircle * 1.5f; // some variation

        ball.Launch(aimTarget, arcMultiplier, returnFlightDuration);
        reactionTimer = 0f;

        Debug.Log($"AI returns with arc multiplier: {arcMultiplier}");
    }

    /// <summary>Simple shot selection: if ball is high, lob; if we're far back, drive harder.</summary>
    private float ChooseShotType()
    {
        // If ball is coming in high, return a lob
        if (ball.Height > 1f)
            return returnApexMultipliers[2]; // lob

        // If we're far from opponent, hit a normal shot
        float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distToPlayer > 6f)
            return returnApexMultipliers[1]; // normal

        // Otherwise, aggressive flat shot
        return returnApexMultipliers[0]; // flat
    }

    public void ServeTowardOpponent(Vector3 targetPos)
    {
        Vector2 serveTarget = new Vector2(targetPos.x, targetPos.y) + Random.insideUnitCircle * 2f;
        ball.Launch(serveTarget, 1f, 0.9f);
    }
}
