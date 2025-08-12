using UnityEngine;

public class BossCollisionHandler : MonoBehaviour
{
    [SerializeField] private BossHealth _bossHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Projectile>(out Projectile projectile))
        {
            if(projectile.CanHitBoss)
            {
                projectile.DestroyProjectile();
                _bossHealth.TakeDamage(1.0f);
            }
        }
    }
}
