using System.Collections.Generic;
using UnityEngine;

public class BossAttackWithVariations : BossAttack
{
    [SerializeField] private List<WeightedAttack> _variationAttacks;
    private BossAttack _selectedAttack;
    public override void StartAction()
    {
        int totalWeight = CalculateTotalWeight();
        int randomNumberInWeightRange = Random.Range(0, totalWeight);
        int currentWeightSum = 0;
        for (int i = 0; i < _variationAttacks.Count; ++i)
        {
            var weightedAttack = _variationAttacks[i];
            currentWeightSum += weightedAttack.Weight;

            if (randomNumberInWeightRange < currentWeightSum)
            {
                _selectedAttack = weightedAttack.Attack;
                _selectedAttack.OnActionFinished.AddListener(OnSelectedAttackFinished);
                _selectedAttack.StartAction();
                break;
            }
        }
    }

    public override void InitializeAttack(BossVersionData bossVersionData)
    {
        foreach (var weightedAttack in _variationAttacks)
        {
            weightedAttack.Attack.InitializeAttack(bossVersionData);
        }
    }

    public override void StopAction()
    {
        _selectedAttack.StopAction();
    }

    private int CalculateTotalWeight()
    {
        int totalWeight = 0;

        foreach (var attack in _variationAttacks)
        {
            totalWeight += attack.Weight;
        }
        return totalWeight;
    }

    private void OnSelectedAttackFinished()
    {
        _selectedAttack.OnActionFinished.RemoveListener(OnSelectedAttackFinished);
        OnActionFinished.Invoke();
    }
}