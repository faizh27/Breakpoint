using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Updated player controller with serve logic and game integration.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallController ball;
    [SerializeField] private GameManager gameManager;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Swing")]
    [SerializeField] private float swingRange = 1.2f;
    [SerializeField] private Transform swingTarget;
    [SerializeField] private float swingApexMultiplier = 1f;
    [SerializeField] private float swingFlightDuration = 0.8f;

    [Header("Serve")]
    [SerializeField] private float serveApexMultiplier = 1f;
    [SerializeField] private float serveFlightDuration = 0.9f;

    private Vector2 moveInput;
    private bool canServe = false;

    private void Update()
    {
        ReadInput();
        Move();

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (canServe)
            {
                TryServe();
            }
            else
            {
                TrySwing();
            }
        }
    }

    private void ReadInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y -= 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x += 1f;

        moveInput = moveInput.normalized;
    }

    private void Move()
    {
        if (moveInput.sqrMagnitude < 0.001f) return;

        Vector3 delta = new Vector3(moveInput.x, moveInput.y, 0f) * moveSpeed * Time.deltaTime;
        transform.position += delta;
    }

    public void AllowServe()
    {
        canServe = true;
        Debug.Log("Ready to serve — press SPACE");
    }

    private void TryServe()
    {
        if (swingTarget == null) return;

        // Serve toward opponent's side
        ball.Launch(swingTarget.position, serveApexMultiplier, serveFlightDuration);
        canServe = false;

        gameManager.OnPlayerServed();
        Debug.Log("Served!");
    }

    private void TrySwing()
    {
        if (ball == null || swingTarget == null) return;

        float distanceToBall = Vector2.Distance(transform.position, ball.GroundPosition);

        if (distanceToBall <= swingRange)
        {
            ball.Launch(swingTarget.position, swingApexMultiplier, swingFlightDuration);
            OnSwingHit();
        }
        else
        {
            OnSwingMiss();
        }
    }

    private void OnSwingHit()
    {
        Debug.Log("Hit!");
    }

    private void OnSwingMiss()
    {
        Debug.Log("Miss (ball out of range)");
        gameManager.EndRally(true); // opponent gets the point
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, swingRange);
    }
}
