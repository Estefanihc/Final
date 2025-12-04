using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NeighborDetectionTester : MonoBehaviour
{
    /*[Header("Detection Settings")]
    [Tooltip("Detection distance from block edge")]
    [SerializeField] private float checkDistance = 0.1f;
    [Tooltip("Layer containing blocks to detect")]
    [SerializeField] private LayerMask blockLayer;
    [Tooltip("Visualization color")]
    [SerializeField] private Color gizmoColor = new Color(0, 1, 1, 0.7f);
    [Tooltip("Show detection gizmos")]
    [SerializeField] private bool drawGizmos = true;
    [Tooltip("Check multiple points per side")]
    [SerializeField] private bool useMultiPointDetection = true;

    [Header("Test Controls")]
    [SerializeField] private bool manualUpdate = false;
    [SerializeField] private bool continuousUpdate = false;

    [Header("Results")]
    [Space(10)]
     public bool hasTopNeighbor;
     public bool hasRightNeighbor;
     public bool hasBottomNeighbor;
     public bool hasLeftNeighbor;
     public bool shouldCastShadow;
     public string detectionDetails;

    private BoxCollider2D _collider;
    private int _selfInstanceID; // Unique identifier to avoid self-detection

    void Awake()
    {
        _selfInstanceID = gameObject.GetInstanceID();
        EnsureComponents();
    }

    void Start()
    {
        StartCoroutine(InitialDetection());
    }

    void Update()
    {
        if (manualUpdate)
        {
            manualUpdate = false;
            UpdateDetection();
        }

        if (continuousUpdate && Application.isPlaying)
        {
            UpdateDetection();
        }
    }

    private IEnumerator InitialDetection()
    {
        yield return new WaitForFixedUpdate();
        yield return null;
        UpdateDetection();
    }

    [ContextMenu("Update Detection Now")]
    public void UpdateDetection()
    {
        if (_collider == null) EnsureComponents();

        hasTopNeighbor = CheckDirection(Vector2.up);
        hasRightNeighbor = CheckDirection(Vector2.right);
        hasBottomNeighbor = CheckDirection(Vector2.down);
        hasLeftNeighbor = CheckDirection(Vector2.left);

        shouldCastShadow = !(hasTopNeighbor && hasRightNeighbor && hasBottomNeighbor && hasLeftNeighbor);

        detectionDetails = GetDetectionReport();
    }

    private bool CheckDirection(Vector2 direction)
    {
        // Always check center point
        bool centerHit = CheckSinglePoint(GetCenterPoint(direction));

        if (!useMultiPointDetection) return centerHit;

        // Check additional points if enabled
        bool edgeHit1 = CheckSinglePoint(GetEdgePoint(direction, 0.66f));
        bool edgeHit2 = CheckSinglePoint(GetEdgePoint(direction, -0.66f));
        bool edgeHit3 = CheckSinglePoint(GetEdgePoint(direction, 0.33f));
        bool edgeHit4 = CheckSinglePoint(GetEdgePoint(direction, -0.33f));

        return centerHit || edgeHit1 || edgeHit2 || edgeHit3 || edgeHit4;
    }

    private bool CheckSinglePoint(Vector2 point)
    {
        Collider2D[] hits = new Collider2D[5];
        int hitCount = Physics2D.OverlapBoxNonAlloc(
            point,
            new Vector2(checkDistance, checkDistance),
            0f,
            hits,
            blockLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            // Use instance ID comparison for absolute safety
            if (hits[i].gameObject.GetInstanceID() != _selfInstanceID)
            {
                Debug.DrawLine(_collider.bounds.center, point, Color.green, 1f);
                return true;
            }
        }

        Debug.DrawLine(_collider.bounds.center, point, Color.red, 1f);
        return false;
    }

    private Vector2 GetCenterPoint(Vector2 direction)
    {
        return (Vector2)_collider.bounds.center + (direction * _collider.bounds.extents.x);
    }

    private Vector2 GetEdgePoint(Vector2 direction, float edgeOffset)
    {
        Vector2 edgeDir = new Vector2(direction.y, -direction.x);
        return GetCenterPoint(direction) + (edgeDir * _collider.bounds.extents.x * edgeOffset);
    }

    private string GetDetectionReport()
    {
        string report = $"Detection Report for {name}:\n";
        report += $"Top: {(hasTopNeighbor ? "BLOCKED" : "open")}\n";
        report += $"Right: {(hasRightNeighbor ? "BLOCKED" : "open")}\n";
        report += $"Bottom: {(hasBottomNeighbor ? "BLOCKED" : "open")}\n";
        report += $"Left: {(hasLeftNeighbor ? "BLOCKED" : "open")}\n";
        report += $"Shadow casting: {(shouldCastShadow ? "ENABLED" : "disabled")}";
        return report;
    }

    private void EnsureComponents()
    {
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider2D>();
            Debug.LogWarning("Added BoxCollider2D for testing", this);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || _collider == null) return;

        Gizmos.color = gizmoColor;
        Vector2 center = _collider.bounds.center;
        Vector2 extents = _collider.bounds.extents;

        DrawDirectionGizmos(center, extents, Vector2.up, hasTopNeighbor);
        DrawDirectionGizmos(center, extents, Vector2.right, hasRightNeighbor);
        DrawDirectionGizmos(center, extents, Vector2.down, hasBottomNeighbor);
        DrawDirectionGizmos(center, extents, Vector2.left, hasLeftNeighbor);

        // Display summary
        GUIStyle style = new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = gizmoColor },
            fontSize = 10,
            alignment = TextAnchor.UpperCenter
        };
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.6f,
            $"Top: {hasTopNeighbor}\nRight: {hasRightNeighbor}\nBottom: {hasBottomNeighbor}\nLeft: {hasLeftNeighbor}",
            style);
    }

    private void DrawDirectionGizmos(Vector2 center, Vector2 extents, Vector2 direction, bool hasNeighbor)
    {
        Color color = hasNeighbor ? Color.red : Color.green;
        color.a = 0.5f;

        Vector2 mainPoint = center + direction * extents.x;
        Gizmos.color = color;
        Gizmos.DrawCube(mainPoint, Vector2.one * checkDistance);

        if (useMultiPointDetection)
        {
            DrawEdgePoint(mainPoint, new Vector2(direction.y, -direction.x), extents.x * 0.66f, color);
            DrawEdgePoint(mainPoint, new Vector2(direction.y, -direction.x), extents.x * -0.66f, color);
        }

        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(center, mainPoint);
    }

    private void DrawEdgePoint(Vector2 mainPoint, Vector2 edgeDir, float distance, Color color)
    {
        Vector2 edgePoint = mainPoint + edgeDir * distance;
        Gizmos.color = color;
        Gizmos.DrawCube(edgePoint, Vector2.one * checkDistance);
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(mainPoint, edgePoint);
    }

    void OnValidate()
    {
        if (_collider == null) EnsureComponents();
        if (!Application.isPlaying && drawGizmos)
        {
            UnityEditor.EditorApplication.delayCall += () => { if (this != null) UpdateDetection(); };
        }
    }
#endif
}*/
}