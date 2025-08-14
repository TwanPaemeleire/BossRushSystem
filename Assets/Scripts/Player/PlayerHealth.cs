using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    private float _currentHealth;
    private PlayerBaseStats _playerBaseStats;

    public UnityEvent OnDamageTaken = new UnityEvent();
    public UnityEvent OnHealed = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();

    private void Start()
    {
        _playerBaseStats= GetComponent<PlayerStatsHolder>().PlayerStats;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        OnDamageTaken.Invoke();
        if(_currentHealth <= 0 )
        {
            _currentHealth = 0;
            OnDeath.Invoke();
        }
        UpdateUI();
    }

    public void Heal(float healing)
    {
        _currentHealth = Mathf.Min(_currentHealth + healing, _playerBaseStats.MaxHealth);
        OnHealed.Invoke();
        UpdateUI();
    }

    public void IncreaseMaxHealth(float amountToAdd, bool increaseCurrentHealth = true)
    {
        _playerBaseStats.MaxHealth += amountToAdd;
        if(increaseCurrentHealth) Heal(amountToAdd);
        UpdateUI();
    }

    public void DecreaseMaxHealth(float amountToRemove, bool decreaseCurrentHealth = false)
    {
        _playerBaseStats.MaxHealth -= amountToRemove;
        _currentHealth -= amountToRemove;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float healthPercentage = _currentHealth / _playerBaseStats.MaxHealth;
        _healthSlider.value = healthPercentage;
    }
}
