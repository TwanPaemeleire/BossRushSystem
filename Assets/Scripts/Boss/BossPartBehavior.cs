using System.Collections.Generic;
using UnityEngine;

public class BossPartBehavior : MonoBehaviour
{
    [SerializeField] private BossPartData _bossPartData;
    [SerializeField] private List<BossAttack> _attacks;
    [SerializeField] private float _delayBeforeFirstAttack = 2f;

    private BossAttack _currentAttack = null;
    private int _currentAttackIndex = -1;

    public void InitializePart(BossVersionData versionData, BossHealth mainBossHealth)
    {
        BossPartHealth bossPartHealth = GetComponent<BossPartHealth>();
        bossPartHealth.InitializeHealth(versionData, mainBossHealth, _bossPartData);
    }

    public void StartAttackCycle()
    {
        foreach (BossAttack attack in _attacks)
        {
            attack.InitializeAttack();
        }
        Invoke(nameof(StartNewAttack), _delayBeforeFirstAttack);
    }

    private void StartNewAttack()
    {
        if (_currentAttack != null)
        {
            _currentAttack.OnAttackFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAttackEarly();
        }

        // If there's a guaranteed next attack, use that one
        if (_currentAttack != null && _currentAttack.NextGuaranteedAttack != null)
        {
            _currentAttack = _currentAttack.NextGuaranteedAttack;
            _currentAttack.OnAttackFinished.AddListener(OnAttackFinished);
            _currentAttack.StartAttack();
            return;
        }

        int randomAttack;
        do
        {
            randomAttack = Random.Range(0, _attacks.Count);
        } while (randomAttack == _currentAttackIndex && !_attacks[_currentAttackIndex].CanExecuteConsecutive);
        _currentAttackIndex = randomAttack;
        _currentAttack = _attacks[_currentAttackIndex];
        _currentAttack.OnAttackFinished.AddListener(OnAttackFinished);
        _currentAttack.StartAttack();

    }

    private void OnAttackFinished()
    {
        Invoke(nameof(StartNewAttack), _currentAttack.DelayAfterAttack);
    }
}
