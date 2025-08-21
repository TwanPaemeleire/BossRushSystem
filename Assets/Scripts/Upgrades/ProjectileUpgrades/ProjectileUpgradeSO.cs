using System.Collections.Generic;
using UnityEngine;
public class ProjectileUpgradeSO : ScriptableObject
{
    public virtual void OnSpawn(PlayerProjectile projectile, PlayerProjectileStats stats) { }
    public virtual void OnHit(PlayerProjectile projectile, Collider2D collision, PlayerProjectileStats stats) { }
    public virtual void OnUpdate(PlayerProjectile projectile, PlayerProjectileStats stats) { }
    public int ExecutionOrder = 0;
}

public class ProjectileUpgradeComparer : IComparer<ProjectileUpgradeSO>
{
    public int Compare(ProjectileUpgradeSO x, ProjectileUpgradeSO y)
    {
        return x.ExecutionOrder - y.ExecutionOrder;
    }
}