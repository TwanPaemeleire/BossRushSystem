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
                Debug.Log("Player projectile hit boss");
                projectile.DestroyProjectile();
            }
        }
    }
}
