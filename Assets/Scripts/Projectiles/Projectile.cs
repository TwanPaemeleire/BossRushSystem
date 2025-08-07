using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private bool _destroyOnScreenLeft = true;
    private float _speedMultiplier = 1f;
    public float SpeedMultiplier { get { return _speedMultiplier; } set { _speedMultiplier = value; } }
    private float _damageMultiplier = 1f;
    public float DamageMultiplier { get { return _damageMultiplier; } set { _damageMultiplier = value; } }
    private GameObject _originalPrefab;
    public GameObject OriginalPrefab { get { return _originalPrefab; } set { _originalPrefab = value; } }
    private Vector3 _shotDirection = Vector3.zero; 
    public Vector3 ShotDirection { get { return _shotDirection; } set { _shotDirection = value; } }

    public virtual void Initialize() { }
    private void OnBecameInvisible()
    {
        if(_destroyOnScreenLeft) DestroyBullet();
    }

    private void DestroyBullet()
    {
        ProjectilePool.Instance.ReleaseProjectile(_originalPrefab, this);
    }
}
