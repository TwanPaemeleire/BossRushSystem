using UnityEngine;
using UnityEngine.Events;

public abstract class BossAttack : MonoBehaviour
{
    [SerializeField] private bool _canExecuteConsecutive = false;
    public bool CanExecuteConsecutive { get { return _canExecuteConsecutive; } }

    [SerializeField] private BossAttack _nextGuaranteedAttack;
    public BossAttack NextGuaranteedAttack { get { return _nextGuaranteedAttack; } }

    [SerializeField] private float _delayAfterAttack = 1.0f;
    public float DelayAfterAttack { get { return _delayAfterAttack; } }

    public UnityEvent OnAttackFinished = new UnityEvent();
    public abstract void StartAttack();
}
