using UnityEngine;

/// <summary>
/// Defines the court boundaries and detects if the ball is in or out.
/// </summary>
public class CourtSystem : MonoBehaviour
{
    [Tooltip("Court boundaries. Adjust these to match your Square object's size.")]
    [SerializeField] private float courtMinX = -5f;
    [SerializeField] private float courtMaxX = 5f;
    [SerializeField] private float courtMinY = -8f;
    [SerializeField] private float courtMaxY = 8f;

    [SerializeField] private bool visualizeCourtBounds = true;

    /// <summary>Check if a ground position is within the court.</summary>
    public bool IsInBounds(Vector2 position)
    {
        return position.x >= courtMinX && position.x <= courtMaxX &&
               position.y >= courtMinY && position.y <= courtMaxY;
    }

    /// <summary>Clamp a position to the court (useful for centering AI).</summary>
    public Vector2 ClampToCourtCenter()
    {
        float centerX = (courtMinX + courtMaxX) / 2f;
        float centerY = (courtMinY + courtMaxY) / 2f;
        return new Vector2(centerX, centerY);
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeCourtBounds) return;

        Gizmos.color = Color.green;
        Vector3 bottomLeft = new Vector3(courtMinX, courtMinY, 0);
        Vector3 bottomRight = new Vector3(courtMaxX, courtMinY, 0);
        Vector3 topLeft = new Vector3(courtMinX, courtMaxY, 0);
        Vector3 topRight = new Vector3(courtMaxX, courtMaxY, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
