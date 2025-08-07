using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PhaseData
{
    public List<WeightedAttack> Attacks;
    [HideInInspector] public int WeightSum;
    public BossAction PhaseEndAction;
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
    [SerializeField] private List<PhaseData> _phasesData;
    [SerializeField] private BossAction _bossDeathAction;
    [SerializeField] private List<PoolToModifyData> _poolsToCreate;
    [SerializeField] private List<PoolsToModifyAfterPhaseChange> _poolsToModifyAfterPhaseChange;
    [SerializeField] private float _delayBeforeFirstAttack = 2.0f;
    [SerializeField] private float _delayBeforeFirstAttackAfterPhaseTransition = 1.0f;
    [SerializeField] private float _delayAfterPhaseTransitionToBeginPoolTrimming = 2.0f;
    public BossVersionData BossVersionData { get { return _bossVersionData; } }

    private BossAttack _currentAttack = null;
    private BossAction _currentPhaseTransitionAction = null;
    private int _currentAttackIndex = -1;
    private int _currentPhaseIndex = 0;
    private BossHealth _health;

    public UnityEvent OnStartingAttack = new UnityEvent();
    public UnityEvent OnEndingAttack = new UnityEvent();
    public UnityEvent OnBossFightFinished = new UnityEvent();

    private void Start()
    {
        _health = GetComponent<BossHealth>();
        _health.InitializeHealth(_bossVersionData.HealthData, _bossVersionData.HealthMultiplier);
        _health.OnPhaseTransition.AddListener(OnPhaseChangeHealthReached);
        _health.OnDeath.AddListener(OnDeath);
        StartBossFight();
    }

    public override void StartBossFight()
    {
        foreach (var poolToCreate in _poolsToCreate)
        {
            ProjectilePool.Instance.InitializeAndPreWarmPool(poolToCreate.Prefab, poolToCreate.Size);
        }

        foreach(var phaseAttacks in _phasesData)
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
        foreach(var phaseData in _phasesData)
        {
            phaseData.WeightSum = 0;
            foreach(var weightedAttack in phaseData.Attacks)
            {
                phaseData.WeightSum += weightedAttack.Weight;
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
            _currentAttack.OnActionFinished.AddListener(OnAttackFinished);
            _currentAttack.StartAction();
            Debug.Log($"Starting guaranteed attack: {_currentAttack.name}");
            return;
        }

        // Look for new random attack, based on the weights
        int randomNumberInWeightRange = Random.Range(0, _phasesData[_currentPhaseIndex].WeightSum);
        int currentWeightSum = 0;
        for (int i = 0; i < _phasesData[_currentPhaseIndex].Attacks.Count; ++i)
        {
            var weightedAttack = _phasesData[_currentPhaseIndex].Attacks[i];
            currentWeightSum += weightedAttack.Weight;
            
            if (randomNumberInWeightRange < currentWeightSum && (_currentAttack == null || i != _currentAttackIndex || _currentAttack.CanExecuteConsecutive))
            {
                _currentAttack = weightedAttack.Attack;
                _currentAttackIndex = i;
                _currentAttack.OnActionFinished.AddListener(OnAttackFinished);
                _currentAttack.StartAction();
                Debug.Log($"Starting new attack: {_currentAttack.name}");
                break;
            }
        }
    }

    private void OnPhaseChangeHealthReached()
    {
        if (_currentPhaseTransitionAction)
        {
            _currentPhaseTransitionAction.OnActionFinished.RemoveListener(OnPhaseChangeHealthReached);
            _currentPhaseTransitionAction.StopAction();
        }

        _currentPhaseTransitionAction = _phasesData[_currentPhaseIndex].PhaseEndAction;
        if(_currentPhaseTransitionAction)
        {
            _currentPhaseTransitionAction.OnActionFinished.AddListener(OnPhaseChangeActionFinished);
            _currentPhaseTransitionAction.StartAction();
        }
        else
        {
            Invoke(nameof(StartNewAttack), _delayBeforeFirstAttackAfterPhaseTransition);
        }

        Invoke(nameof(ApplyPoolChangesAfterTransition), _delayAfterPhaseTransitionToBeginPoolTrimming);
        ++_currentPhaseIndex;
        if(_currentAttack != null)
        {
            _currentAttack.OnActionFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAction();
            _currentAttack = null;
        }
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
            _currentAttack.OnActionFinished.RemoveListener(OnAttackFinished);
            _currentAttack.StopAction();
        }
        Invoke(nameof(StartNewAttack), _currentAttack.DelayAfterAttack);
    }

    private void OnPhaseChangeActionFinished()
    {
        if(_currentPhaseTransitionAction != null)
        {
            _currentPhaseTransitionAction.OnActionFinished.RemoveListener(OnPhaseChangeActionFinished);
            _currentPhaseTransitionAction.StopAction();
            _currentPhaseTransitionAction = null;
        }
        Invoke(nameof(StartNewAttack), _delayBeforeFirstAttackAfterPhaseTransition);
    }

    private void OnDeath()
    {
        if(_bossDeathAction != null)
        {
            _bossDeathAction.OnActionFinished.AddListener(OnDeathActionFinished);
            _bossDeathAction.StartAction();
        }
        else
        {
            OnBossFightFinished.Invoke();
        }
    }

    private void OnDeathActionFinished()
    {
        _bossDeathAction.OnActionFinished.RemoveListener(OnDeathActionFinished);
        _bossDeathAction.StopAction();
        OnBossFightFinished.Invoke();
    }

    private int GetIndexOfAttackInPhaseAttacks(BossAttack attack)
    {
        int amountOfPhaseAttacks = _phasesData[_currentPhaseIndex].Attacks.Count;
        for(int attackIdx = 0; attackIdx < amountOfPhaseAttacks; ++attackIdx)
        {
            if (_phasesData[_currentPhaseIndex].Attacks[attackIdx].Attack == attack)
            {
                return attackIdx;
            }
        }
        return -1;
    }
}