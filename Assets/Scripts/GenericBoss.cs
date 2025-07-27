using System.Collections.Generic;
using UnityEngine;

public struct WeightedAttack
{
    public BossAttack Attack;
    public float Weight;
}

public class GenericBoss : BossBehavior
{
    [SerializeField] private BossVersionData _bossVersionData;
    public BossVersionData BossVersionData { get { return _bossVersionData; } }
    [SerializeField] private List<List<WeightedAttack>> _attacks;
    [SerializeField] private float _delayBeforeFirstAttack = 2f;
    [SerializeField] private float _delayBeforeFirstAttackAfterPhaseTransition = 1f;
    private BossAttack _currentAttack = null;
    private int _previousAttackIndex = -1;

    public override void StartBossFight()
    {
        
    }

    private void StartNewAttack()
    {

    }

    private void OnPhaseChangeHealthReached()
    {

    }
}
