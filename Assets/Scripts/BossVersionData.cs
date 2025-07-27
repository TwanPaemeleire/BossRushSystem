using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossVersionData", menuName = "Boss/BossVersionData", order = 2)]
public class BossVersionData : ScriptableObject
{
    public int Level;
    public float Health;
    public float Speed;
    public float ContactDamage;
    public float ProjectileDamageMultiplier;
    public float ProjectileSpeedMultiplier;
    public float SpeedMultiplier;
    public float FireRateMultiplier;
    public GameObject BossObject;
}