using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicProjectilePool : MonoBehaviour
{
    private GameObject _prefab;
    private Transform _ownderTransform;
    private Stack<GameObject> _inactiveObjects = new Stack<GameObject>();
    private HashSet<GameObject> _allObjects = new HashSet<GameObject>();

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
        Stack<GameObject> tempStack = new Stack<GameObject>(desiredSize);
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
        projectileObject.GetComponent<Projectile>().OriginalPrefab = _prefab;
        projectileObject.SetActive(false);
        projectileObject.transform.SetParent(_ownderTransform);
        _allObjects.Add(projectileObject);
        _inactiveObjects.Push(projectileObject);
    }

    public GameObject Get(float speedMultiplier, float damageMultiplier, Vector2 shotDirection)
    {
        if (_inactiveObjects.Count == 0) CreateInstance();
        var projectileObject = _inactiveObjects.Pop();
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.SpeedMultiplier = speedMultiplier;
        projectile.DamageMultiplier = damageMultiplier;
        projectile.ShotDirection = shotDirection;
        projectile.Initialize();
        projectileObject.SetActive(true);
        return projectileObject;
    }

    public void Release(GameObject projectileObject)
    {
        projectileObject.SetActive(false);
        _inactiveObjects.Push(projectileObject);
    }

    public void RequestTrimToSizeOverTime(int desiredSize)
    {
        if(desiredSize > _allObjects.Count)
        {
            Debug.LogWarning("Attempted to trim pool to largr size than current size");
            return;
        }
        _sizeToCurrentlyTrimTo = Mathf.Max(0, desiredSize);
        _trimCoroutine = StartCoroutine(TrimToSizeOverTime());
    }

    public void ClearAndDestroyAllObjects()
    {
        foreach (var projectileObject in _allObjects)
        {
            Destroy(projectileObject);
        }
        _allObjects.Clear();
        _inactiveObjects.Clear();
        if(_trimCoroutine != null) StopCoroutine(_trimCoroutine);
    }

    private IEnumerator TrimToSizeOverTime()
    {
        int amountOfTimesToFullTrim = (_allObjects.Count - _sizeToCurrentlyTrimTo) / _trimmedObjectsPerFrame;
        int amountPartialTrim = (_allObjects.Count - _sizeToCurrentlyTrimTo) % _trimmedObjectsPerFrame;
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
            if(_inactiveObjects.Count > 0)
            {
                var objectToDestroy = _inactiveObjects.Pop();
                _allObjects.Remove(objectToDestroy);
                Destroy(objectToDestroy);
            }
            else
            {
                var objectToDestroy = _allObjects.First();
                _allObjects.Remove(objectToDestroy);
                Destroy(objectToDestroy);
            }
        }
    }
}