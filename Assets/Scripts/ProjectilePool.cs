using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public enum ProjectileType
{
    a, b, c, d, e, f,
}

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<ProjectileType, ObjectPool<GameObject>> _projectilePools = new Dictionary<ProjectileType, ObjectPool<GameObject>>();
    public static ProjectilePool Instance;
    private void Awake()
    {
        Instance = this;
    }

    private ObjectPool<GameObject> CreateProjectilePool(GameObject prefab, Transform parent)
    {
        return new ObjectPool<GameObject>(
            () =>
            {
                var projectileObject = Instantiate(prefab);
                projectileObject.transform.SetParent(parent, true);
                return projectileObject;
            },
            OnProjectileGet,
            OnProjectileRelease,
            OnProjectileDestroy,
            true, 0, int.MaxValue);
    }

    public void InitializeAndPreWarmPool(ProjectileType projectileType, GameObject prefab, int desiredSize)
    {
        if(!_projectilePools.TryGetValue(projectileType, out  ObjectPool<GameObject> pool))
        {
            var owner = new GameObject($"{projectileType} Projectile Container");
            owner.transform.SetParent(transform);
            pool = CreateProjectilePool(prefab, owner.transform);
            _projectilePools.Add(projectileType, pool);
        }
        int amountToCreate = Mathf.Max(0, desiredSize - pool.CountAll);
        if (amountToCreate == 0) return;
        List<GameObject> tempObjectList = new List<GameObject>(amountToCreate);
        for (int i = 0; i < amountToCreate; i++)
        {
            GameObject projectileObject = pool.Get();
            tempObjectList.Add(projectileObject);
        }
        foreach (GameObject projectileObject in tempObjectList)
        {
            pool.Release(projectileObject);
        }
    }

    public GameObject GetProjectile(ProjectileType projectileType)
    {
        if(_projectilePools.ContainsKey(projectileType))
        {
            return _projectilePools[projectileType].Get();
        }
        Debug.LogWarning($"Projectile type {projectileType} not found in pool.");
        return null;
    }

    public void ReleaseProjectile(ProjectileType projectileType, GameObject projectile)
    {
        if (_projectilePools.ContainsKey(projectileType))
        {
            _projectilePools[projectileType].Release(projectile);
        }
        else
        {
            Destroy(projectile);
        }
    }

    private void OnProjectileGet(GameObject projectile)
    {
        projectile.SetActive(true);
        projectile.GetComponent<TestProjectile>().Initialize();
    }

    private void OnProjectileRelease(GameObject projectile)
    {
        projectile.SetActive(false);
    }

    private void OnProjectileDestroy(GameObject projectile)
    {
        Destroy(projectile);
    }
}