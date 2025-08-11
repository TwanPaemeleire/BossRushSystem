using UnityEngine;
public class ProjectileUpgradeSO : ScriptableObject
{
    public virtual void OnSpawn(Projectile projectile) { }
    public virtual void OnHit(Projectile projectile, RaycastHit2D hit) { }
    public virtual void OnUpdate(Projectile projectile) { }
}