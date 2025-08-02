using UnityEngine;

public class TestProjectile : MonoBehaviour
{
    [SerializeField] float _speed = 6f;
    [SerializeField] private ProjectileType _projectileType;

    private bool _beingReturnedToPool = false;
    private float _speedMultiplier = 1f;
    public float speedMultiplier { get { return _speedMultiplier; }  set { _speedMultiplier = value; } }
    public void Initialize()
    {
        _beingReturnedToPool = false;
    }

    private void OnBecameInvisible()
    {
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        ProjectilePool.Instance.ReleaseProjectile(_projectileType, this.gameObject);
    }

    private void Update()
    {
        transform.position += _speed * _speedMultiplier * Time.deltaTime * transform.right;
    }
}
