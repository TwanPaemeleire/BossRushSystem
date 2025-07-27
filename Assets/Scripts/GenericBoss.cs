using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhaseAttacks
{
    public List<WeightedAttack> Attacks;
    [HideInInspector] public int WeightSum;
}

[System.Serializable]
public class WeightedAttack
{
    public BossAttack Attack;
    public int Weight;
}

public class GenericBoss : BossBehavior
{
    [SerializeField] private BossVersionData _bossVersionData;
    public BossVersionData BossVersionData { get { return _bossVersionData; } }
    [SerializeField] private List<PhaseAttacks> _phaseAttacks;
    [SerializeField] private float _delayBeforeFirstAttack = 2f;
    [SerializeField] private float _delayBeforeFirstAttackAfterPhaseTransition = 1f;
    private BossAttack _currentAttack = null;
    private int _currentAttackIndex = -1;
    private int _currentPhaseIndex = 0;

    public override void StartBossFight()
    {
        CalculateWeights();
        StartNewAttack();
    }

    private void CalculateWeights()
    {
        foreach (var phaseAttacks in _phaseAttacks)
        {
            phaseAttacks.WeightSum = 0;
            foreach (var weightedAttack in phaseAttacks.Attacks)
            {
                phaseAttacks.WeightSum += weightedAttack.Weight;
            }
        }
    }

    private void StartNewAttack()
    {
        if(_currentAttack != null)
        {
            _currentAttack.OnAttackFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAttackEarly();
        }

        // If there's a guaranteed next attack, use that one
        if(_currentAttack != null && _currentAttack.NextGuaranteedAttack != null)
        {
            _currentAttack = _currentAttack.NextGuaranteedAttack;
            _currentAttack.OnAttackFinished.AddListener(OnAttackFinished);
            _currentAttack.StartAttack();
            return;
        }

        // Look for new random attack, based on the weights
        int randomNumberInWeightRange = Random.Range(0, _phaseAttacks[_currentPhaseIndex].WeightSum);
        int currentWeightSum = 0;
        for (int i = 0; i < _phaseAttacks[_currentPhaseIndex].Attacks.Count; ++i)
        {
            var weightedAttack = _phaseAttacks[_currentPhaseIndex].Attacks[i];
            currentWeightSum += weightedAttack.Weight;
            if (randomNumberInWeightRange < currentWeightSum && (i != _currentAttackIndex || _currentAttack.CanExecuteConsecutive))
            {
                _currentAttack = weightedAttack.Attack;
                _currentAttackIndex = i;
                break;
            }
        }
    }

    private void OnPhaseChangeHealthReached()
    {
        ++_currentPhaseIndex;
        if(_currentAttack != null)
        {
            _currentAttack.OnAttackFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAttackEarly();
        }
    }

    private void OnAttackFinished()
    {
        Invoke(nameof(StartNewAttack), _currentAttack.DelayAfterAttack);
    }
}