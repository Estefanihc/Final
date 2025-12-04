public interface IDamageable
{
    void TakeDamage(float amount, DamageSource source =DamageSource.None);
}
[System.Serializable]
public enum DamageSource
{
    None,
    Sword,
    Axe,
    Pickaxe,
    Shovel,
    Fire,
    Explosion,
    All
    // add more if needed
}