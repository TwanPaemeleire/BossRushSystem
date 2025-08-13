using UnityEngine;
public class ProjectileUpgradeSO : ScriptableObject
{
    public virtual void OnSpawn(PlayerProjectile projectile, PlayerProjectileStats stats) { }
    public virtual void OnHit(PlayerProjectile projectile, Collider2D collision, PlayerProjectileStats stats) { }
    public virtual void OnUpdate(PlayerProjectile projectile, PlayerProjectileStats stats) { }
}