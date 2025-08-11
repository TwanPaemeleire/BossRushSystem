using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/BossData", order = 1)]
public class BossData : ScriptableObject
{
    [HideInInspector]
    public Guid ID;
    public Sprite UISprite;
    public string Name;
    public string Description;
    public List<BossVersionData> Versions;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (ID == Guid.Empty)
        {
            ID = Guid.NewGuid();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}