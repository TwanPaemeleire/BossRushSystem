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
    private bool _canHitBoss = false;
    public bool CanHitBoss { get { return _canHitBoss; } set { _canHitBoss = value; } }
    private bool _isBeingReleased = false;

    public virtual void Initialize() 
    {
        _canHitBoss = false;
        _isBeingReleased = false;
    }
    private void OnBecameInvisible()
    {
        if(_destroyOnScreenLeft && !_isBeingReleased) DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        _isBeingReleased = true;
        ProjectilePool.Instance.ReleaseProjectile(_originalPrefab, this);
    }
}
