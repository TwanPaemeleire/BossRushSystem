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
    }
    public override void OnUpdate(PlayerProjectile projectile, PlayerProjectileStats stats)
    {
        Vector2 toTarget = (_bossTransform.position - projectile.transform.position).normalized;
        if(toTarget.x > 0.0f)
        {
            Vector2 dirWithStrenght = toTarget *= stats.HomingStrenght * Time.deltaTime;
            projectile.transform.position += new Vector3(0.0f, dirWithStrenght.y, 0.0f);
        }   
    }
}