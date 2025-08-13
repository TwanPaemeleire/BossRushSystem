using UnityEngine;
using System.Collections.Generic;

public class PlayerProjectile : Projectile
{
    private List<ProjectileUpgradeSO> _upgrades = new List<ProjectileUpgradeSO>();
    public List<ProjectileUpgradeSO> Upgrades { get { return _upgrades; } set { _upgrades = value; } }
    private PlayerProjectileStats _playerProjectileStats;
    public PlayerProjectileStats PlayerProjectileStats { get { return _playerProjectileStats; } set { _playerProjectileStats = value; } }
    private Vector3 _currentDirection;
    public Vector2 CurrentDirection { get { return _currentDirection; } set { _currentDirection = value; } }
    private void Update()
    {
        transform.position += _playerProjectileStats.MovementSpeed * SpeedMultiplier * Time.deltaTime * _currentDirection;
        foreach(var upgrade in _upgrades)
        {
            upgrade.OnUpdate(this, _playerProjectileStats); 
        }
    }

    private void OnEnable()
    {
        _currentDirection = ShotDirection;
        foreach (var upgrade in _upgrades)
        {
            upgrade.OnSpawn(this, _playerProjectileStats);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var upgrade in _upgrades)
        {
            upgrade.OnHit(this, collision, _playerProjectileStats);
        }
    }
}