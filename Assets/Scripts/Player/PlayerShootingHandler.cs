using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PlayerShootingStats
{
    public float FireRatePerSecond;
    public float DefaultShotSpeed;
}

public struct ShotData
{
    public GameObject Prefab;
    public Vector3 Position;
    public Vector3 Direction;
    public float BaseSpeed;
}

public class PlayerShootingHandler : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private PlayerShootingStats _playerShootingStats;

    private List<ShotUpgradeSO> _shotUpgrades = new List<ShotUpgradeSO>();
    private List<ShotData> _dataToShoot = new List<ShotData>();

    private float _shootingDelay => 1.0f / _playerShootingStats.FireRatePerSecond;
    private void Start()
    {
        StartShooting();
    }

    public void StartShooting()
    {
        ProjectilePool.Instance.InitializeAndPreWarmPool(_projectilePrefab, 20);
        InvokeRepeating(nameof(Shoot), 0.0f, _shootingDelay);
    }

    private void Shoot()
    {
        ShotData shotdata = new ShotData 
        { 
            Prefab = _projectilePrefab, 
            Position = transform.position, 
            Direction = transform.right, 
            BaseSpeed = 1.0f 
        };
        _dataToShoot.Add(shotdata);
        foreach(var shotUpgrade in _shotUpgrades)
        {
            shotUpgrade.ModifyShots(_dataToShoot);
        }
        foreach(var shotData in _dataToShoot)
        {
            PlayerProjectile projectile = ProjectilePool.Instance.GetProjectile(shotData.Prefab, 1.0f, 1.0f, shotData.Position, shotData.Direction) as PlayerProjectile;
        }
    }
}
