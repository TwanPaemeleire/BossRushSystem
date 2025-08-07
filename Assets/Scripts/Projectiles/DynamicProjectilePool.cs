using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicProjectilePool : MonoBehaviour
{
    private GameObject _prefab;
    private Transform _ownderTransform;
    private Stack<Projectile> _inactiveInstances = new Stack<Projectile>();
    private HashSet<Projectile> _allInstances = new HashSet<Projectile>();

    private Coroutine _trimCoroutine;
    private int _sizeToCurrentlyTrimTo = -1;
    private int _trimmedObjectsPerFrame = 3;

    public void Initialize(GameObject prefab, Transform owner)
    {
        _prefab = prefab;
        _ownderTransform = owner;
    }

    public void PreWarmPool(int desiredSize)
    {
        Stack<Projectile> tempStack = new Stack<Projectile>(desiredSize);
        for(int i = 0; i < desiredSize; ++i)
        {
            tempStack.Push(Get(1.0f, 1.0f, Vector2.zero));
        }
        for (int i = 0; i < desiredSize; ++i)
        {
            Release(tempStack.Pop());
        }
    }

    private void CreateInstance()
    {
        var projectileObject = Instantiate(_prefab);
        projectileObject.SetActive(false);
        projectileObject.transform.SetParent(_ownderTransform);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.OriginalPrefab = _prefab;
        _allInstances.Add(projectile);
        _inactiveInstances.Push(projectile);
    }

    public Projectile Get(float speedMultiplier, float damageMultiplier, Vector2 shotDirection)
    {
        if (_inactiveInstances.Count == 0) CreateInstance();
        var projectile = _inactiveInstances.Pop();
        projectile.SpeedMultiplier = speedMultiplier;
        projectile.DamageMultiplier = damageMultiplier;
        projectile.ShotDirection = shotDirection;
        projectile.Initialize();
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void Release(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _inactiveInstances.Push(projectile);
    }

    public void RequestTrimToSizeOverTime(int desiredSize)
    {
        if(desiredSize > _allInstances.Count)
        {
            Debug.LogWarning("Attempted to trim pool to largr size than current size");
            return;
        }
        _sizeToCurrentlyTrimTo = Mathf.Max(0, desiredSize);
        _trimCoroutine = StartCoroutine(TrimToSizeOverTime());
    }

    public void ClearAndDestroyAllObjects()
    {
        foreach (var projectile in _allInstances)
        {
            Destroy(projectile.gameObject);
        }
        _allInstances.Clear();
        _inactiveInstances.Clear();
        if(_trimCoroutine != null) StopCoroutine(_trimCoroutine);
    }

    private IEnumerator TrimToSizeOverTime()
    {
        int amountOfTimesToFullTrim = (_allInstances.Count - _sizeToCurrentlyTrimTo) / _trimmedObjectsPerFrame;
        int amountPartialTrim = (_allInstances.Count - _sizeToCurrentlyTrimTo) % _trimmedObjectsPerFrame;
        for(int trimCounter = 0; trimCounter < amountOfTimesToFullTrim; ++trimCounter)
        {
            RemoveTrimAmount(_trimmedObjectsPerFrame);
            yield return null;
        }
        RemoveTrimAmount(amountPartialTrim);
        _trimCoroutine = null;
    }

    private void RemoveTrimAmount(int amount)
    {
        for(int i = 0; i < amount; ++i)
        {
            if(_inactiveInstances.Count > 0)
            {
                var projectileToDestroy = _inactiveInstances.Pop();
                _allInstances.Remove(projectileToDestroy);
                Destroy(projectileToDestroy.gameObject);
            }
            else
            {
                var projectileToDestroy = _allInstances.First();
                _allInstances.Remove(projectileToDestroy);
                Destroy(projectileToDestroy.gameObject);
            }
        }
    }
}