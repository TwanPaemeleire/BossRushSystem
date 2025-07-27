using UnityEngine;

public class BossTestAttack : BossAttack
{
    [SerializeField] private float _Test;

    public override void StartAttack()
    {
        Debug.Log($"Test attack started with value: {_Test}");
    }
}
