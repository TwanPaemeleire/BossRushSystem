using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShotUpgrade", menuName = "Upgrades/ShotUpgrade", order = 2)]
public class ShotUpgradeSO : ScriptableObject
{
    public virtual void ModifyShots(List<ShotData> shots) { }
}
