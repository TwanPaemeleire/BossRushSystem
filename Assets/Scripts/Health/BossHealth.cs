using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;
    private BossHealthData _bossHealthData;
    private float _maxHealth;
    private float _currentHealth;
    private int _currentPhase = 0;
    private float _healthToTriggerNextPhase = -1.0f;
    private bool _isInFinalPhase = false;

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
            CalculateHealthToTriggerNextPhase();
        }
        else
        {
            _isInFinalPhase = true;
        }
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if(_currentHealth <= 0) // Boss has died
        {
            _currentHealth = 0;
            OnDeath.Invoke();
        }
        else if(!_isInFinalPhase && _currentHealth <= _healthToTriggerNextPhase) // If the boss has more than one phase and has taken enough damage
        {
            ++_currentPhase;
            if(_currentPhase != _bossHealthData.PhaseTriggerPercentages.Count)
            {
                CalculateHealthToTriggerNextPhase();
            }
            else
            {
                _isInFinalPhase = true;
            }
            OnPhaseTransition.Invoke();
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        _healthBar.value = _currentHealth / _maxHealth;
    }

    private void CalculateHealthToTriggerNextPhase()
    {
        _healthToTriggerNextPhase = _maxHealth * _bossHealthData.PhaseTriggerPercentages[_currentPhase];
    }
}