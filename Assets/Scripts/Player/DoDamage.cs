using UnityEngine;

public class DoDamage : MonoBehaviour
{
    public float damage = 5f;

    [SerializeField] private float startWaitTime = 0.2f;
    private float waitTime;

    public bool selfDestroy = false;
    public float lifeTime = 3f;

    public bool damagePlayer = true;
    public bool damageEnemy = false;
    public bool damageObjects = false;

    public DamageSource damageSource = DamageSource.None;

    private void Start()
    {
        waitTime = startWaitTime;

        if (selfDestroy)
            Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (waitTime > 0)
            waitTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (waitTime > 0) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null) return;

        // Filter by tag
        if (collision.CompareTag("Player") && damagePlayer)
        {
            ApplyDamage(damageable);
        }
        else if (collision.CompareTag("Enemy") && damageEnemy)
        {
            ApplyDamage(damageable);
        }
        else if (collision.CompareTag("Object") && damageObjects)
        {
            ApplyDamage(damageable);
        }
    }

    private void ApplyDamage(IDamageable target)
    {
        target.TakeDamage(damage, damageSource);
        waitTime = startWaitTime;

        if (selfDestroy)
            Destroy(gameObject);
    }
}
