using UnityEngine;
using System.Collections.Generic;
public abstract class ShotUpgradeSO : ScriptableObject
{
    public abstract void ModifyShots(List<ShotData> shots);
    public int ExecutionOrder = 0;
}

public class ShotUpgradeComparer : IComparer<ShotUpgradeSO>
{
    public int Compare(ShotUpgradeSO x, ShotUpgradeSO y)
    {
        return x.ExecutionOrder - y.ExecutionOrder;
    }
}