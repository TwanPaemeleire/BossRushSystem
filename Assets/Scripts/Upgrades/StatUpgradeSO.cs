using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Upgrades/StatUpgrade", order = 1)]
public class StatUpgradeSO : ScriptableObject
{
    public virtual void ApplyUpgrade() { }
}