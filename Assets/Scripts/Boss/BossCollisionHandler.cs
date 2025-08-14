using UnityEngine;

public class BossCollisionHandler : MonoBehaviour
{
    [SerializeField] private BossHealth _bossHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerProjectile>(out PlayerProjectile playerProjectile))
        {
            if(playerProjectile.CanHitBoss)
            {
                _bossHealth.TakeDamage(playerProjectile.PlayerProjectileStats.ProjectileDamage * playerProjectile.DamageMultiplier);
                playerProjectile.DestroyProjectile();
            }
        }
    }
}
