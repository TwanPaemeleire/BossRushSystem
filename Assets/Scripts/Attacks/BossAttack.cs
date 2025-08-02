using UnityEngine;
using UnityEngine.Events;

public abstract class BossAttack : MonoBehaviour
{
    [Header("Base Attack Settings")]
    [SerializeField] private bool _canExecuteConsecutive = false;
    public bool CanExecuteConsecutive { get { return _canExecuteConsecutive; } }

    [SerializeField] private BossAttack _nextGuaranteedAttack;
    public BossAttack NextGuaranteedAttack { get { return _nextGuaranteedAttack; } }

    [SerializeField] private float _delayAfterAttack = 1.0f;
    public float DelayAfterAttack { get { return _delayAfterAttack; } }

    protected float _projectileSpeedMultiplier;
    protected float _projectileDamageMultiplier;

    public UnityEvent OnAttackFinished = new UnityEvent();
    public abstract void StartAttack();
    public abstract void StopAttackEarly();
    public virtual void InitializeAttack(BossVersionData bossVersionData)
    {
        _projectileSpeedMultiplier = bossVersionData.ProjectileSpeedMultiplier;
        _projectileDamageMultiplier = bossVersionData.ProjectileDamageMultiplier;
    }
    public virtual void InitializeAttack(BossVersionData bossVersionData, BossPartData bossPartData)
    {
        _projectileSpeedMultiplier = bossVersionData.ProjectileSpeedMultiplier * bossPartData.ProjectileSpeedMultiplier;
        _projectileDamageMultiplier = bossVersionData.ProjectileDamageMultiplier * bossPartData.ProjectileDamageMultiplier;
    }
}