using UnityEngine;
using System.Collections.Generic;

public class ShotData
{
    public GameObject Prefab;
    public Vector3 Position;
    public Vector3 Direction;
    public float BaseSpeed;

    public ShotData Clone()
    {
        return new ShotData
        {
            Prefab = this.Prefab,
            Position = this.Position,
            Direction = this.Direction,
            BaseSpeed = this.BaseSpeed
        };
    }
}

public class PlayerShootingHandler : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;

    [SerializeField] private List<ShotUpgradeSO> _shotUpgrades = new List<ShotUpgradeSO>();
    private List<ShotData> _dataToShoot = new List<ShotData>();
    [SerializeField] private List<ProjectileUpgradeSO> _projectileUpgrades = new List<ProjectileUpgradeSO>();

    public List<ShotUpgradeSO> ShotUpgrades {get {return _shotUpgrades;} set { _shotUpgrades = value; } }
    public List<ProjectileUpgradeSO> ProjectileUpgrades { get { return _projectileUpgrades; } set { _projectileUpgrades = value; } }

    private PlayerStatsHolder _playerStatsHolder;
    private float _shootingDelay => 1.0f / _playerStatsHolder.PlayerStats.FireRatePerSecond;
    private void Start()
    {
        _playerStatsHolder = GetComponentInParent<PlayerStatsHolder>();
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
            projectile.CanHitBoss = true;
            projectile.Upgrades = _projectileUpgrades;
            projectile.PlayerProjectileStats = _playerStatsHolder.ProjectileStats;
        }
        _dataToShoot.Clear();
    }

    public void AddShotUpgrade(ShotUpgradeSO upgrade)
    {
        _shotUpgrades.Add(upgrade);
        _shotUpgrades.Sort(new ShotUpgradeComparer());
    }

    public void AddProjectileUpgrade(ProjectileUpgradeSO upgrade)
    {
        _projectileUpgrades.Add(upgrade);
        _projectileUpgrades.Sort(new ProjectileUpgradeComparer());
    }
}
