using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "HomingProjectileUpgrade", menuName = "Upgrades/ProjectileUpgrades/HomingProjectileUpgrade")]
public class HomingProjectile : ProjectileUpgradeSO
{
    private Transform _bossTransform;
    public override void OnSpawn(PlayerProjectile projectile, PlayerProjectileStats stats)
    {
        // In future, give projectile an easy way to access boss transform, maybe via game manager or something
        GenericBoss boss = FindAnyObjectByType<GenericBoss>();
        if(boss)
        {
            _bossTransform = boss.transform;
        }
        else
        {
            _bossTransform = null;
        }
    }
    public override void OnUpdate(PlayerProjectile projectile, PlayerProjectileStats stats)
    {
        if (_bossTransform == null) return;
        float distance = Vector2.Distance(_bossTransform.position, projectile.transform.position);
        if (distance > stats.HomingTriggerDistance) return;
        Vector2 distanceToBoss = (_bossTransform.position - projectile.transform.position);
        if (distanceToBoss.x < 0.0f) return;
        if (Mathf.Abs(distanceToBoss.y) < 0.1f)
        {
            projectile.CurrentDirection = new Vector2(projectile.CurrentDirection.x, 0.0f);
            return;
        }
        float yDir = stats.HomingStrenght * ((distanceToBoss.y > 0.0f) ? 1 : -1);
        projectile.CurrentDirection= new Vector2(projectile.CurrentDirection.x, yDir);
    }
}