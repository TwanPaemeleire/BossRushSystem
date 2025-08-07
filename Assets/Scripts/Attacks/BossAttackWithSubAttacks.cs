using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackWithSubAttacks : BossAttack
{
    [Header("Sub Attack Settings")]
    [SerializeField] private List<BossAttack> _subAttacks;
    [SerializeField] private List<float> _delayBeforeAttackExecutions;
    [SerializeField] private BossAttack _attackToFinishToBeDone;

    private Coroutine _attackCoroutine;

    private void OnValidate()
    {
        if (_subAttacks == null) _subAttacks = new List<BossAttack>();
        if (_delayBeforeAttackExecutions == null) _delayBeforeAttackExecutions = new List<float>();
        while (_delayBeforeAttackExecutions.Count < _subAttacks.Count)
        {
            _delayBeforeAttackExecutions.Add(0f);
        }
        while (_delayBeforeAttackExecutions.Count > _subAttacks.Count)
        {
            _delayBeforeAttackExecutions.RemoveAt(_delayBeforeAttackExecutions.Count - 1);
        }
    }

    public override void InitializeAttack(BossVersionData bossVersionData)
    {
       foreach (BossAttack attack in _subAttacks)
       {
            attack.InitializeAttack(bossVersionData);
       }
    }

    public override void StartAction()
    {
        _attackToFinishToBeDone.OnActionFinished.AddListener(OnSubAttackFinished);
        _attackCoroutine = StartCoroutine(StartAttacks());
    }

    public override void StopAction()
    {
         _attackToFinishToBeDone.OnActionFinished.RemoveListener(OnSubAttackFinished);
        foreach (var attack in _subAttacks)
        {
            attack.StopAction();
        }
    }

    IEnumerator StartAttacks()
    {
        for(int attackIdx = 0; attackIdx < _subAttacks.Count; ++attackIdx)
        {
            yield return new WaitForSeconds(_delayBeforeAttackExecutions[attackIdx]);
            _subAttacks[attackIdx].StartAction();
        }
    }

    private void OnSubAttackFinished()
    {
        OnActionFinished.Invoke();
    }
}
