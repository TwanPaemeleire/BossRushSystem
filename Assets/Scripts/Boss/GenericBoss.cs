using System.Collections.Generic;
using UnityEditor.UIElements;
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

[System.Serializable]
public class PoolToModifyData
{
    public GameObject Prefab;
    public int Size;
}

[System.Serializable]
public class PoolsToModifyAfterPhaseChange
{
    public List<PoolToModifyData> PoolsToModify;
}

public class GenericBoss : BossBehavior
{
    [SerializeField] private BossVersionData _bossVersionData;
    public BossVersionData BossVersionData { get { return _bossVersionData; } }
    [SerializeField] private List<PhaseAttacks> _phaseAttacks;
    [SerializeField] private List<PoolToModifyData> _poolsToCreate;
    [SerializeField] private List<PoolsToModifyAfterPhaseChange> _poolsToModifyAfterPhaseChange;
    [SerializeField] private float _delayBeforeFirstAttack = 2.0f;
    [SerializeField] private float _delayBeforeFirstAttackAfterPhaseTransition = 1.0f;
    [SerializeField] private float _delayAfterPhaseTransitionToBeginPoolTrimming = 2.0f;
    private BossAttack _currentAttack = null;
    private int _currentAttackIndex = -1;
    private int _currentPhaseIndex = 0;

    private BossHealth _health;

    private void Start()
    {
        _health = GetComponent<BossHealth>();
        _health.InitializeHealth(_bossVersionData.HealthData, _bossVersionData.HealthMultiplier);
        _health.OnPhaseTransition.AddListener(OnPhaseChangeHealthReached);
        StartBossFight();
    }

    public override void StartBossFight()
    {
        foreach (var poolToCreate in _poolsToCreate)
        {
            ProjectilePool.Instance.InitializeAndPreWarmPool(poolToCreate.Prefab, poolToCreate.Size);
        }

        foreach(var phaseAttacks in _phaseAttacks)
        {
            foreach(var weightedAttack in phaseAttacks.Attacks)
            {
                weightedAttack.Attack.InitializeAttack(_bossVersionData);
            }
        }
        CalculateWeights();
        Invoke(nameof(StartNewAttack), _delayBeforeFirstAttack);
    }

    private void CalculateWeights()
    {
        foreach(var phaseAttacks in _phaseAttacks)
        {
            phaseAttacks.WeightSum = 0;
            foreach(var weightedAttack in phaseAttacks.Attacks)
            {
                phaseAttacks.WeightSum += weightedAttack.Weight;
            }
        }
    }

    private void StartNewAttack()
    {
        // If there's a guaranteed next attack, use that one
        if(_currentAttack != null && _currentAttack.NextGuaranteedAttack != null)
        {
            _currentAttack = _currentAttack.NextGuaranteedAttack;
            _currentAttackIndex = GetIndexOfAttackInPhaseAttacks(_currentAttack);
            _currentAttack.OnAttackFinished.AddListener(OnAttackFinished);
            _currentAttack.StartAttack();
            Debug.Log($"Starting guaranteed attack: {_currentAttack.name}");
            return;
        }

        // Look for new random attack, based on the weights
        int randomNumberInWeightRange = Random.Range(0, _phaseAttacks[_currentPhaseIndex].WeightSum);
        int currentWeightSum = 0;
        for (int i = 0; i < _phaseAttacks[_currentPhaseIndex].Attacks.Count; ++i)
        {
            var weightedAttack = _phaseAttacks[_currentPhaseIndex].Attacks[i];
            currentWeightSum += weightedAttack.Weight;
            
            if (randomNumberInWeightRange < currentWeightSum && (_currentAttack == null || i != _currentAttackIndex || _currentAttack.CanExecuteConsecutive))
            {
                _currentAttack = weightedAttack.Attack;
                _currentAttackIndex = i;
                _currentAttack.OnAttackFinished.AddListener(OnAttackFinished);
                _currentAttack.StartAttack();
                Debug.Log($"Starting new attack: {_currentAttack.name}");
                break;
            }
        }
    }

    private void OnPhaseChangeHealthReached()
    {
        Invoke(nameof(ApplyPoolChangesAfterTransition), _delayAfterPhaseTransitionToBeginPoolTrimming);
        ++_currentPhaseIndex;
        if(_currentAttack != null)
        {
            _currentAttack.OnAttackFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAttackEarly();
            _currentAttack = null;
        }
        Invoke(nameof(StartNewAttack), _delayBeforeFirstAttackAfterPhaseTransition);
    }

    private void ApplyPoolChangesAfterTransition()
    {
        Debug.Log("Starting pool trimming");
        foreach (var poolToModify in _poolsToModifyAfterPhaseChange[_currentPhaseIndex - 1].PoolsToModify)
        {
            ProjectilePool.Instance.RequestTrimToSizeOverTime(poolToModify.Prefab, poolToModify.Size);
        }
    }

    private void OnAttackFinished()
    {
        if (_currentAttack != null)
        {
            _currentAttack.OnAttackFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAttackEarly();
        }
        Invoke(nameof(StartNewAttack), _currentAttack.DelayAfterAttack);
    }

    private int GetIndexOfAttackInPhaseAttacks(BossAttack attack)
    {
        int amountOfPhaseAttacks = _phaseAttacks[_currentPhaseIndex].Attacks.Count;
        for(int attackIdx = 0; attackIdx < amountOfPhaseAttacks; ++attackIdx)
        {
            if (_phaseAttacks[_currentPhaseIndex].Attacks[attackIdx].Attack == attack)
            {
                return attackIdx;
            }
        }
        return -1;
    }
}