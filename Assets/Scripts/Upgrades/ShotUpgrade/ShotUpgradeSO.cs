using UnityEngine;
using System.Collections.Generic;
public abstract class ShotUpgradeSO : ScriptableObject
{
    public abstract void ModifyShots(List<ShotData> shots);
}