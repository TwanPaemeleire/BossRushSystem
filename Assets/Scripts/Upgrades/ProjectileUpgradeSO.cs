using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileUpgrade", menuName = "Upgrades/ProjectileUpgrade", order = 3)]
public class ProjectileUpgradeSO : ScriptableObject
{
    public virtual void OnSpawn(Projectile projectile) { }
    public virtual void OnHit(Projectile projectile, RaycastHit2D hit) { }
    public virtual void OnUpdate(Projectile projectile) { }
}
