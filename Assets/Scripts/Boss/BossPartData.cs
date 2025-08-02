using UnityEngine;

[CreateAssetMenu(fileName = "BossPartData", menuName = "Boss/BossPartData", order = 4)]
public class BossPartData : ScriptableObject
{
    public float PercentageOfTotalHealth;
    public float PercentageOfHealthDamageOnDestroy;
    public float SpeedMultiplier = 1f;
    public float ProjectileSpeedMultiplier = 1f;
    public float ProjectileDamageMultiplier = 1f;
    public GameObject Prefab;
}
