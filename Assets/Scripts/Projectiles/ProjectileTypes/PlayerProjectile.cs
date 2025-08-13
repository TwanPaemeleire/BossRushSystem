using UnityEngine;
using System.Collections.Generic;

public class PlayerProjectile : Projectile
{
    [SerializeField] private float _speed = 5.0f;
    private List<ProjectileUpgradeSO> _upgrades = new List<ProjectileUpgradeSO>();
    public List<ProjectileUpgradeSO> Upgrades { get { return _upgrades; } set { _upgrades = value; } }
    private PlayerProjectileStats _playerProjectileStats;
    public PlayerProjectileStats PlayerProjectileStats { get { return _playerProjectileStats; } set { _playerProjectileStats = value; } }
    private void Update()
    {
        transform.position += _speed * SpeedMultiplier * Time.deltaTime * ShotDirection;
        foreach(var upgrade in _upgrades)
        {
            upgrade.OnUpdate(this, _playerProjectileStats); 
        }
    }

    private void OnEnable()
    {
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