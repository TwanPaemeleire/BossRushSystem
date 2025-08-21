using UnityEngine;

public class PlayerStatsHolder : MonoBehaviour
{
    [SerializeField] private PlayerBaseStats _baseStats;
    public PlayerBaseStats PlayerStats { get { return _baseStats; } set { _baseStats = value; } }
    private PlayerBaseStats _originalBaseStats;
    private PlayerBaseStats _snapShotBaseStats = null;

    [SerializeField] private PlayerProjectileStats _projectileStats;
    public PlayerProjectileStats ProjectileStats { get { return _projectileStats; } set { _projectileStats = value; } }
    private PlayerProjectileStats _originalProjectileStats;
    private PlayerProjectileStats _snapShotProjectileStats = null;


    private void Awake()
    {
        _originalBaseStats = _baseStats.Clone();
        _originalProjectileStats = _projectileStats.Clone();
    }

    public void RestoreBaseStats()
    {
        _baseStats = _originalBaseStats.Clone();
        _projectileStats = _originalProjectileStats.Clone();
    }

    public void TakeSnapShot()
    {
        _snapShotBaseStats = _baseStats.Clone();
        _snapShotProjectileStats = _projectileStats.Clone();
    }

    public void RestoreSnapShot()
    { 
        if(_snapShotBaseStats == null || _snapShotProjectileStats == null)
        {
            Debug.LogWarning("No snapshot to restore");
            return;
        }
        _baseStats = _snapShotBaseStats.Clone();
        _projectileStats = _snapShotProjectileStats.Clone();
    }
}

[System.Serializable]
public class PlayerBaseStats
{
    [Header("Base Player")]
    public float MovementSpeed = 5.0f;
    public float FocusModeMovementSpeed = 2.0f;
    public float MaxHealth = 10.0f;

    [Header("Shooting")]
    public float FireRatePerSecond = 5.0f;

    public PlayerBaseStats Clone()
    {
        return new PlayerBaseStats
        {
            MovementSpeed = MovementSpeed,
            FocusModeMovementSpeed = FocusModeMovementSpeed,
            MaxHealth = MaxHealth,
        };
    }
}

[System.Serializable]
public class PlayerProjectileStats
{
    public float ProjectileMovementSpeed = 5.0f;
    public float ProjectileDamage = 1.0f;
    public float HomingTriggerDistance = 3.0f;
    public float HomingStrenght = 1.0f;

    public PlayerProjectileStats Clone()
    {
        return new PlayerProjectileStats
        {
            ProjectileMovementSpeed = ProjectileMovementSpeed,
            ProjectileDamage = ProjectileDamage,
            HomingTriggerDistance = HomingTriggerDistance,
            HomingStrenght = HomingStrenght
        };
    }
}