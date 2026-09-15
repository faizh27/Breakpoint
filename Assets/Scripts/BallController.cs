using UnityEngine;

/// <summary>
/// Custom parabolic ball trajectory for a top-down tennis game.
/// Not using Rigidbody physics directly for the flight arc — we want precise,
/// tunable control over apex height, hang time, and bounce behavior rather
/// than fighting Unity's gravity/drag settings.
///
/// Setup:
/// - Attach to the ball GameObject (needs a SpriteRenderer for the ball sprite
///   and a separate child SpriteRenderer for the shadow, assigned below).
/// - The ball's visual Y position (transform) represents court position (x, z-ish).
/// - "Height" (bounce arc) is faked with a separate float and applied as a
///   local Y offset + scale, so it reads as height in a top-down view.
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform ballVisual;   // child object holding the ball sprite
    [SerializeField] private Transform shadowVisual; // child object holding the shadow sprite

    [Header("Flight Tuning")]
    [Tooltip("How high the ball arcs at the peak of its flight (visual units).")]
    [SerializeField] private float baseApexHeight = 2f;

    [Tooltip("Gravity strength — higher = faster fall, shorter hang time.")]
    [SerializeField] private float gravity = 9.8f;

    [Tooltip("Velocity retained after each bounce (0-1). 0.6 = loses 40% speed per bounce.")]
    [SerializeField, Range(0f, 1f)] private float bounceDamping = 0.6f;

    [Tooltip("Below this height, the ball is considered to have stopped bouncing.")]
    [SerializeField] private float minBounceHeight = 0.05f;

    [Header("Shadow Tuning")]
    [Tooltip("Shadow scale when ball is at ground level.")]
    [SerializeField] private float shadowScaleAtGround = 1f;

    [Tooltip("Shadow scale when ball is at apex height (should be smaller — implies distance from court).")]
    [SerializeField] private float shadowScaleAtApex = 0.5f;

    [Tooltip("Ball sprite scale when at apex, to sell height (bigger = closer to camera).")]
    [SerializeField] private float ballScaleAtApex = 1.3f;
    [SerializeField] private float ballScaleAtGround = 1f;

    [Tooltip("How far the ball sprite visually lifts off its shadow per unit of height. Higher = more visible separation.")]
    [SerializeField] private float visualHeightOffsetMultiplier = 0.6f;

    // Runtime state
    private Vector2 groundPosition;   // actual court X/Z position (as X/Y in 2D top-down)
    private Vector2 groundVelocity;   // court-plane velocity
    private float height;             // current visual height above the court
    private float verticalVelocity;   // current vertical (bounce) velocity
    private bool isMoving;

    public bool IsMoving => isMoving;
    public Vector2 GroundPosition => groundPosition;
    public Vector2 GroundVelocity => groundVelocity;
    public float Height => height;

    private void Awake()
    {
        groundPosition = transform.position;
    }

    private void Update()
    {
        // TEMP TEST TRIGGERS — remove once step 2 (real hitbox/swing input) drives this.
        // Compare different shot "shapes" side by side without restarting Play mode.
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;

            if (kb.digit1Key.wasPressedThisFrame)
                Launch(new Vector2(5f, 3f), 0.6f, 0.5f);   // flat, fast, low arc

            if (kb.digit2Key.wasPressedThisFrame)
                Launch(new Vector2(5f, 3f), 1f, 0.8f);     // standard rally shot

            if (kb.digit3Key.wasPressedThisFrame)
                Launch(new Vector2(5f, 3f), 1.8f, 1.3f);   // high lob

            if (kb.rKey.wasPressedThisFrame)
                ResetBall(new Vector2(0f, 0f));            // reset to start position
        }

        if (!isMoving) return;

        float dt = Time.deltaTime;

        // Move along the court plane
        groundPosition += groundVelocity * dt;

        // Apply vertical (bounce) physics
        verticalVelocity -= gravity * dt;
        height += verticalVelocity * dt;

        if (height <= 0f)
        {
            height = 0f;
            verticalVelocity = -verticalVelocity * bounceDamping;
            groundVelocity *= bounceDamping; // ball also loses ground speed on bounce

            if (verticalVelocity < minBounceHeight)
            {
                isMoving = false;
                verticalVelocity = 0f;
            }
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Launch the ball from its current ground position toward a target ground
    /// position, arriving with a given apex height multiplier and flight duration.
    /// Call this from the shot/serve system.
    /// </summary>
    public void Launch(Vector2 targetGroundPosition, float apexHeightMultiplier, float flightDuration)
    {
        Vector2 displacement = targetGroundPosition - groundPosition;
        groundVelocity = displacement / flightDuration;

        float apex = baseApexHeight * apexHeightMultiplier;

        // Solve initial vertical velocity so the ball reaches 'apex' height
        // partway through the flight, then descends — using basic projectile math.
        // v0 = sqrt(2 * g * apex) gets us the peak; timing naturally falls out
        // of gravity + apex, so flightDuration mainly affects ground speed feel.
        verticalVelocity = Mathf.Sqrt(2f * gravity * apex);
        height = 0f;
        isMoving = true;
    }

    private void UpdateVisuals()
    {
        // Preserve existing Z so we don't clobber sorting/depth set in the scene.
        transform.position = new Vector3(groundPosition.x, groundPosition.y, transform.position.z);

        float apex = baseApexHeight; // used only for normalizing 0-1 height ratio below
        float heightRatio = apex > 0f ? Mathf.Clamp01(height / apex) : 0f;

        if (ballVisual != null)
        {
            // Offset the ball sprite upward visually and scale it up to sell height
            ballVisual.localPosition = new Vector3(0f, height * visualHeightOffsetMultiplier, 0f);
            float ballScale = Mathf.Lerp(ballScaleAtGround, ballScaleAtApex, heightRatio);
            ballVisual.localScale = Vector3.one * ballScale;
        }

        if (shadowVisual != null)
        {
            // Shadow shrinks and (optionally) fades as the ball gets higher —
            // this is the main cue that sells "height" in a top-down view.
            float shadowScale = Mathf.Lerp(shadowScaleAtGround, shadowScaleAtApex, heightRatio);
            shadowVisual.localScale = Vector3.one * shadowScale;
        }
    }

    /// <summary>Immediately stop and place the ball (e.g. resetting for a new serve).</summary>
    public void ResetBall(Vector2 position)
    {
        groundPosition = position;
        groundVelocity = Vector2.zero;
        height = 0f;
        verticalVelocity = 0f;
        isMoving = false;
        UpdateVisuals();
    }
}