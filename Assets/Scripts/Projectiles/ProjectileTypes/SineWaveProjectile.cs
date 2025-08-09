using System.Collections;
using UnityEngine;

public class SineWaveProjectile : Projectile
{
    [SerializeField] private float _halfWaveHeight = 0.2f;
    [SerializeField] private float _timePerHalfWave = 0.2f;
    [SerializeField] private float _movementSpeed = 5.0f;

    private Vector3 _basePosition;
    private Vector3 _sineOffset;

    public override void Initialize()
    {
        _basePosition = transform.position;
        StartCoroutine(SineWaveLoop());
    }

    void Update()
    {
        _basePosition += _movementSpeed * SpeedMultiplier * Time.deltaTime * ShotDirection;
        transform.position = _basePosition + _sineOffset;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SineWaveLoop()
    {
        Vector3 up = new Vector3(-ShotDirection.y, ShotDirection.x);
        float elapsedTime = 0f;
        while (true)
        {
            float sin = Mathf.Sin(elapsedTime * Mathf.PI / _timePerHalfWave);
            _sineOffset = up * (_halfWaveHeight * sin);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}