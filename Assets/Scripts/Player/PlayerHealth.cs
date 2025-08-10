using System.ComponentModel;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    [SerializeField] private Slider _healthSlider;
    private float _currenthHealth;

    public UnityEvent OnDamageTaken = new UnityEvent();
    public UnityEvent OnHealed = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();

    public void TakeDamage(float damage)
    {
        _currenthHealth -= damage;
        OnDamageTaken.Invoke();
        if(_currenthHealth <= 0 )
        {
            _currenthHealth = 0;
            OnDeath.Invoke();
        }
        UpdateUI();
    }

    public void Heal(float healing)
    {
        _currenthHealth = Mathf.Min(_currenthHealth + healing, _maxHealth);
        OnHealed.Invoke();
        UpdateUI();
    }

    public void IncreaseMaxHealth(float amountToAdd, bool increaseCurrentHealth = true)
    {
        _maxHealth += amountToAdd;
        if(increaseCurrentHealth) Heal(amountToAdd);
        UpdateUI();
    }

    public void DecreaseMaxHealth(float amountToRemove, bool decreaseCurrentHealth = false)
    {
        _maxHealth -= amountToRemove;
        _currenthHealth -= amountToRemove;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float healthPercentage = _currenthHealth / _maxHealth;
        _healthSlider.value = healthPercentage;
    }
}
