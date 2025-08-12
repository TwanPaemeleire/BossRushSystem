using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeCard", menuName = "Upgrades/UpgradeCard", order = 1)]
public class UpgradeCard : ScriptableObject
{
    public List<ShotUpgradeSO> ShotUpgrades;
    public List<ProjectileUpgradeSO> ProjectileUpgrades;
    public List<StatUpgradeSO> StatUpgrades;
}
