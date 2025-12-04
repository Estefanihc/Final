using UnityEngine;

public class ToolDamage : MonoBehaviour
{
    public float damage = 10f;
    public float attackCooldown = 0.2f;
    public DamageSource damageSource = DamageSource.None;

    [SerializeField] private Animator animator;
    [SerializeField] private string attackButton = "Fire1"; // Left click by default

    private float currentCooldown = 0f;
    private bool canDamage = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentCooldown -= Time.deltaTime;

        if (Input.GetButtonDown(attackButton) && currentCooldown <= 0f)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            canDamage = true;
            currentCooldown = attackCooldown;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canDamage) return;

        if (collision.CompareTag("Object"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, damageSource);
                canDamage = false;
            }
        }
    }
}
