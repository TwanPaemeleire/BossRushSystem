using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    BossHealthData _bossHealthData;
    private float _maxHealth;
    private float _currentHealth;
    private int _currentPhase = -1;
    private float _healthToTriggerNextPhase = -1.0f;

    public UnityEvent OnTakeDamage = new UnityEvent();
    public UnityEvent OnPhaseTransition = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();

    public void InitializeHealth(BossHealthData bossHealthData, float healthMultiplier)
    {
        _bossHealthData = bossHealthData;
        _maxHealth = _bossHealthData.Health * healthMultiplier;
        _currentHealth = _maxHealth;
        if(_bossHealthData.PhaseTriggerPercentages.Count > 0)
        {
            _currentPhase = 0;
            CalculateHealthToTriggerNextPhase();
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if(_currentHealth <= 0) // Boss has died
        {
            _currentHealth = 0;
            OnDeath.Invoke();
        }
        else if(_currentPhase != -1 && _currentHealth <= _healthToTriggerNextPhase) // If the boss has more than one phase and has taken enough damage
        {
            ++_currentPhase;
            CalculateHealthToTriggerNextPhase();
            OnPhaseTransition.Invoke();
        }
    }

    private void CalculateHealthToTriggerNextPhase()
    {
        _healthToTriggerNextPhase = _maxHealth * _bossHealthData.PhaseTriggerPercentages[_currentPhase];
    }
}