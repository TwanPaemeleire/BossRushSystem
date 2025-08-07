using UnityEngine;
using UnityEngine.Events;

public abstract class BossAction : MonoBehaviour
{
    public UnityEvent OnActionFinished = new UnityEvent();
    public abstract void StartAction();
    public abstract void StopAction();
}
