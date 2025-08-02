using UnityEngine;
using UnityEngine.Events;

public class BossPartHealth : MonoBehaviour
{
    private float _currentHealth;
    private float _mainBossMaxHealth;

    private BossHealth _mainBossHealth;
    private BossPartData _bossPartData;

    public UnityEvent OnDamaged = new UnityEvent();
    public UnityEvent OnDestroyed = new UnityEvent();

    public void InitializeHealth(BossVersionData versionData, BossHealth mainBossHealth, BossPartData partData)
    {
        _mainBossHealth = mainBossHealth;
        _bossPartData = partData;
        _mainBossMaxHealth = versionData.HealthData.Health * versionData.HealthMultiplier;
        _currentHealth = _mainBossMaxHealth * partData.PercentageOfTotalHealth;
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        OnDamaged.Invoke();
        _mainBossHealth.TakeDamage(amount);
        if(_currentHealth <= 0)
        {
            _mainBossHealth.TakeDamage(_mainBossMaxHealth * _bossPartData.PercentageOfHealthDamageOnDestroy);
            OnDestroyed.Invoke();
        }
    }
}
