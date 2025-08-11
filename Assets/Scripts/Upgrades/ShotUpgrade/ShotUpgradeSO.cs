using UnityEngine;
using System.Collections.Generic;
public class ShotUpgradeSO : ScriptableObject
{
    public virtual void ModifyShots(List<ShotData> shots) { }
}