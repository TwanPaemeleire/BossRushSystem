using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/BossVersion", order = 1)]
public class BossData : ScriptableObject
{
    public Sprite UISprite;
    public string Name;
    public string Description;
    public List<BossVersionData> Versions;
}