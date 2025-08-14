using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeCard", menuName = "Upgrades/UpgradeCard", order = 1)]
public class UpgradeCard : ScriptableObject
{
    [Range(1, 3)]
    public int Rarity;
    public bool CanBeCollectedMultipleTimes = true;
    public List<ShotUpgradeSO> ShotUpgrades;
    public List<ProjectileUpgradeSO> ProjectileUpgrades;
    public List<StatUpgradeSO> StatUpgrades;
    public List<UpgradeCard> DependencyCards;
}
