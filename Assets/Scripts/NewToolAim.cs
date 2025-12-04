using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NewToolAim : MonoBehaviour
{
    [Header("Combat Settings")]
    public float damage = 10f;
    public DamageSource damageSource = DamageSource.None;
    public float attackCooldown = 0.5f;
    public float attackRange = 2f;
    public float detectionRadius = 0.3f; // <- How big the circle is around the mouse
    public LayerMask blockLayer;

    [Header("Visual Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer toolSprite;
    [SerializeField] private string attackButton = "Fire1";
    [SerializeField] private float followSmoothness = 10f;
    [SerializeField] private GameObject cursorIndicator;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Color highlightColor = Color.yellow;

    private float currentCooldown = 0f;
    private Camera mainCamera;
    private Vector2 mousePosition;
    private bool isAttacking = false;
    private Material originalBlockMaterial;
    private SpriteRenderer currentTarget;


    private void OnDisable()
    {
        if (currentTarget != null && originalBlockMaterial != null)
        {
            currentTarget.material = originalBlockMaterial;
            currentTarget = null;
        }
    }
    void Start()
    {
        mainCamera = Camera.main;

        if (animator == null && cursorIndicator != null)
            animator = cursorIndicator.GetComponent<Animator>();

        if (toolSprite == null && cursorIndicator != null)
            toolSprite = cursorIndicator.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (inventory.Instance.IsInventoryOpen)
            return;
        currentCooldown -= Time.deltaTime;
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        HandleAttackInput();
        FollowMouse();
        UpdateVisuals();
        HighlightTargetBlock();
    }

    private void HandleAttackInput()
    {
        if (Input.GetButton(attackButton) && currentCooldown <= 0f)
        {
            isAttacking = true;
            currentCooldown = attackCooldown;

            if (animator != null)
                animator.SetTrigger("Attack");

            TryDamageBlock();
        }
        else if (Input.GetButtonUp(attackButton))
        {
            isAttacking = false;
        }
    }

    private void TryDamageBlock()
    {
        Vector2 playerPosition = transform.parent.position;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (cursorIndicator != null)
        {
            cursorIndicator.SetActive(true);
            cursorIndicator.transform.position = mouseWorldPosition;
        }

        if (Vector2.Distance(playerPosition, mouseWorldPosition) > attackRange)
            return;

        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, detectionRadius, blockLayer);

        if (hit != null)
        {
            IDamageable block = hit.GetComponent<IDamageable>();
            if (block != null)
            {
                block.TakeDamage(damage, damageSource);
            }
        }
    }

    private void FollowMouse()
    {
        if (cursorIndicator == null) return;

        Vector2 playerPosition = transform.parent.position;
        Vector2 direction = (mousePosition - playerPosition).normalized;
        float distanceToMouse = Vector2.Distance(playerPosition, mousePosition);

        if (distanceToMouse > attackRange)
        {
            cursorIndicator.SetActive(false);
            return;
        }

        cursorIndicator.SetActive(true);
        float effectiveDistance = Mathf.Min(distanceToMouse, attackRange);
        Vector2 targetPosition = playerPosition + direction * effectiveDistance;

        cursorIndicator.transform.position = Vector2.Lerp(
            cursorIndicator.transform.position,
            targetPosition,
            followSmoothness * Time.deltaTime
        );

        FlipSpriteBasedOnRotation();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cursorIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateVisuals()
    {
        if (toolSprite != null)
        {
            float angle = cursorIndicator.transform.eulerAngles.z;
            toolSprite.flipX = angle > 90 && angle < 270;
        }
    }

    private void HighlightTargetBlock()
    {
        if (currentTarget != null && originalBlockMaterial != null)
        {
            currentTarget.material = originalBlockMaterial;
            currentTarget = null;
        }

        Vector2 playerPosition = transform.parent.position;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(playerPosition, mouseWorldPosition) > attackRange)
            return;

        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, detectionRadius, blockLayer);

        if (hit != null)
        {
            currentTarget = hit.GetComponent<SpriteRenderer>();
            if (currentTarget != null)
            {
                originalBlockMaterial = currentTarget.material;

                if (highlightMaterial != null)
                {
                    currentTarget.material = highlightMaterial;
                }
                else
                {
                    currentTarget.color = highlightColor;
                }
            }
        }
    }

    private void FlipSpriteBasedOnRotation()
    {
        if (toolSprite == null) return;

        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180) zRotation -= 360;

        toolSprite.flipY = !(zRotation <= 90 && zRotation >= -90);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;

        Vector2 playerPosition = transform.parent != null ? transform.parent.position : Vector2.zero;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distanceToMouse = Vector2.Distance(playerPosition, mouseWorldPosition);

        Handles.color = new Color(1, 1, 0, 0.1f);
        Handles.DrawSolidDisc(playerPosition, Vector3.forward, attackRange);

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(playerPosition, Vector3.forward, attackRange);

        Handles.color = distanceToMouse <= attackRange ? Color.cyan : Color.gray;
        Handles.DrawWireDisc(mouseWorldPosition, Vector3.forward, detectionRadius);

        Collider2D hit = Physics2D.OverlapCircle(mouseWorldPosition, detectionRadius, blockLayer);
        if (hit != null)
        {
            Handles.color = Color.green;
            Handles.DrawSolidDisc(hit.transform.position, Vector3.forward, 0.15f);
        }
    }
#endif
}
