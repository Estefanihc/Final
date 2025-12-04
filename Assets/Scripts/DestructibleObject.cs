using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ShadowCaster2D), typeof(BoxCollider2D))]
public class DestructibleObject : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 20f;
    private float currentHealth;

    [Header("Damage Settings")]
    public DamageSource allowedSource = DamageSource.Pickaxe;

    [Header("Death Settings")]
    public GameObject deathEffect;
    public GameObject dropPrefab;
    public float dropSpawnForce = 1f;

    private ShadowCaster2D shadowCaster;
    private BoxCollider2D boxCollider;



    [Header("Detection Settings")]
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

    [Header("Results")]
    [Space(10)]
    public bool hasTopNeighbor;
    public bool hasRightNeighbor;
    public bool hasBottomNeighbor;
    public bool hasLeftNeighbor;
    public bool shouldCastShadow;

    private BoxCollider2D _collider;
    private int _selfInstanceID; // Unique identifier to avoid self-detection

    [Header("Proximity Activation")]
    [SerializeField] private bool startActive = false;
    [SerializeField] private GameObject[] dependentObjects;

    private bool isActive;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    public bool dontUseShadowCaster = false;
    public void SetActivated(bool active)
    {
        if (isActive == active) return;
        isActive = active;

        // Update core components
        if (col != null) col.enabled = active;
        if (shadowCaster != null) shadowCaster.enabled = active;
        if (spriteRenderer != null) spriteRenderer.enabled = active;

        // Update dependent objects
        foreach (var obj in dependentObjects)
        {
            if (obj != null) obj.SetActive(active);
        }

        // Update shadows if activating
        if (active) UpdateDetection(false);
    }
    void Awake()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize state
        SetActivated(startActive);
        currentHealth = maxHealth;
        shadowCaster = GetComponent<ShadowCaster2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        _selfInstanceID = gameObject.GetInstanceID();
        EnsureComponents();
        // Initially disable shadow caster - we'll enable it only if needed
        shadowCaster.enabled = false;
    }

    void Start()
    {
        StartCoroutine(InitialDetection());
    }


    private IEnumerator InitialDetection()
    {
        yield return new WaitForFixedUpdate();
        yield return null;
        UpdateDetection(false);
    }


    public void TakeDamage(float damage, DamageSource source)
    {
        if (source == allowedSource || allowedSource == DamageSource.All)
        {

            currentHealth -= damage;
            if (currentHealth <= 0) Die();
        }
    }

    private void Die()
    {
        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (dropPrefab != null)
        {
            GameObject drop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            if (drop.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 randomForce = new Vector2(
                    Random.Range(-dropSpawnForce, dropSpawnForce),
                    Random.Range(0.5f, dropSpawnForce)
                );
                rb.AddForce(randomForce, ForceMode2D.Impulse);
            }
        }
        if(dontUseShadowCaster==false)
        UpdateDetection(true);
        ObjectPoolingManager.instance.ReturnObjectToPool(gameObject); // Destroy(gameObject);
    }
    public void LateDetection()
    {
        StartCoroutine(InitialDetection());
    }
    //-----------------------------------------------------
    [ContextMenu("Update Detection Now")]
    public void UpdateDetection(bool updateNeighbor)
    {
      
            if (_collider == null) EnsureComponents();

        hasTopNeighbor = CheckDirection(Vector2.up, updateNeighbor);
        hasRightNeighbor = CheckDirection(Vector2.right, updateNeighbor);
        hasBottomNeighbor = CheckDirection(Vector2.down, updateNeighbor);
        hasLeftNeighbor = CheckDirection(Vector2.left, updateNeighbor);

        shouldCastShadow = !(hasTopNeighbor && hasRightNeighbor && hasBottomNeighbor && hasLeftNeighbor);
         if (dontUseShadowCaster == true&& shadowCaster != null) { shadowCaster.enabled = false; }
      else  if (shadowCaster != null)
        shadowCaster.enabled = shouldCastShadow;
    }

    private bool CheckDirection(Vector2 direction, bool updateNeighbor)
    {
        // Always check center point
        bool centerHit = CheckSinglePoint(GetCenterPoint(direction), updateNeighbor);

        if (!useMultiPointDetection) return centerHit;

        // Check additional points if enabled
        bool edgeHit1 = CheckSinglePoint(GetEdgePoint(direction, 0.66f), updateNeighbor);
        bool edgeHit2 = CheckSinglePoint(GetEdgePoint(direction, -0.66f), updateNeighbor);
        bool edgeHit3 = CheckSinglePoint(GetEdgePoint(direction, 0.33f), updateNeighbor);
        bool edgeHit4 = CheckSinglePoint(GetEdgePoint(direction, -0.33f), updateNeighbor);

        return centerHit || edgeHit1 || edgeHit2 || edgeHit3 || edgeHit4;
    }

    private bool CheckSinglePoint(Vector2 point, bool updateNeighbor)
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
                if (updateNeighbor)
                    hits[i].GetComponent<DestructibleObject>().LateDetection();
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
    private void EnsureComponents()
    {
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider2D>();
            Debug.LogWarning("Added BoxCollider2D for testing", this);
        }
    }
    public bool CheckIfHasAnyNeighbor()
    {
        UpdateDetection(false);
        return hasTopNeighbor || hasRightNeighbor || hasBottomNeighbor || hasLeftNeighbor;
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
            UnityEditor.EditorApplication.delayCall += () => { if (this != null) UpdateDetection(false); };
        }
    }
#endif
}
