using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public enum ProjectileType
{
    a, b, c, d, e, f,
}

public class ProjectilePool : MonoBehaviour
{
    private Dictionary<ProjectileType, ObjectPool<GameObject>> _projectilePool;
    public static ProjectilePool Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _projectilePool = new Dictionary<ProjectileType, ObjectPool<GameObject>>();
    }

    public void AddProjectilePool(ProjectileType projectileType, GameObject prefab, int initialSize = 10)
    {
        if (!_projectilePool.ContainsKey(projectileType))
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => Instantiate(prefab),
                OnProjectileGet,
                OnProjectileRelease,
                OnProjectileDestroy,
                false, initialSize, initialSize);
            _projectilePool.Add(projectileType, pool);
        }
        else
        {
            Debug.LogWarning($"Projectile pool for type {projectileType} already exists.");
        }
    }

    public GameObject GetProjectile(ProjectileType projectileType)
    {
        if(_projectilePool.ContainsKey(projectileType))
        {
            return _projectilePool[projectileType].Get();
        }
        Debug.LogWarning($"Projectile type {projectileType} not found in pool.");
        return null;
    }

    public void ReleaseProjectile(ProjectileType projectileType, GameObject projectile)
    {
        if (_projectilePool.ContainsKey(projectileType))
        {
            _projectilePool[projectileType].Release(projectile);
        }
        else
        {
            Destroy(projectile);
        }
    }

    private void OnProjectileGet(GameObject projectile)
    {
        projectile.SetActive(true);
        // call init method of projectile component
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