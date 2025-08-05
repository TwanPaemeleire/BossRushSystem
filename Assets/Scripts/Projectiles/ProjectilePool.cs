using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public enum ProjectileType
{
    a, b, c, d, e, f,
}

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<GameObject, DynamicProjectilePool> _projectilePools = new Dictionary<GameObject, DynamicProjectilePool>();
    public static ProjectilePool Instance;
    private void Awake()
    {
        Instance = this; // Will change in future to have a MonoSingleton script
    }

    public void InitializeAndPreWarmPool(GameObject prefab, int desiredSize)
    {
        if(!_projectilePools.TryGetValue(prefab, out DynamicProjectilePool pool))
        {
            var owner = new GameObject($"{prefab.name} Pool");
            pool = owner.AddComponent<DynamicProjectilePool>();
            pool.Initialize(prefab, owner.transform);
            _projectilePools.Add(prefab, pool);
        }
        pool.PreWarmPool(desiredSize);
    }

    public GameObject GetProjectile(GameObject prefab)
    {
        if(_projectilePools.ContainsKey(prefab))
        {
            return _projectilePools[prefab].Get();
        }
        Debug.LogWarning($"Projectile type {prefab} not found in pool.");
        return null;
    }

    public void ReleaseProjectile(GameObject prefab, GameObject projectile)
    {
        if (_projectilePools.ContainsKey(prefab))
        {
            _projectilePools[prefab].Release(projectile);
        }
        else
        {
            Destroy(projectile);
        }
    }
}