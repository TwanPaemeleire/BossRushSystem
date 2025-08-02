using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossHealthData", menuName = "Boss/BossHealthData", order = 3)]
public class BossHealthData : ScriptableObject
{
    public float Health;
    public List<float> PhaseTriggerPercentages = new List<float>();
}
