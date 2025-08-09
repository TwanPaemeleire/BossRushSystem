using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTestAttack : BossAttack
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _amountOfShooters = 4;
    [SerializeField] private float _bulletShootDelay = 0.2f;
    [SerializeField] private float _amountOffFullRotations = 1.0f;
    [SerializeField] private float _rotationSpeed = 1.0f;
    [SerializeField] private float _rotationSpeedApplyTime = 2.0f;
    private bool _rotatingClockWise = true;

    private float _shooterAngleInterval;
    List<Coroutine> _shootingCoroutines = new List<Coroutine>();
    List<float> _shootingAngles = new List<float>();

    public override void StartAction()
    {
        int randomDirection = Random.Range(0, 2);
        _rotatingClockWise = randomDirection == 0;

        _shooterAngleInterval = 360.0f / (float)_amountOfShooters;
        for (int shooterIdx = 0; shooterIdx < _amountOfShooters; ++shooterIdx)
        {
            float angle = shooterIdx * _shooterAngleInterval;
            _shootingAngles.Add(angle);
            _shootingCoroutines.Add(StartCoroutine(ShootWhileRotating(angle, shooterIdx)));
        }
    }

    public override void StopAction()
    {
        StopAllCoroutines();
    }

    IEnumerator ShootWhileRotating(float startAngle, int shooterIdx)
    {
        float angleTracker = 0.0f;
        _shootingAngles[shooterIdx] = startAngle;

        StartCoroutine(ShootBullet(shooterIdx));

        float elapsedTime = 0.0f;
        while (angleTracker < _amountOffFullRotations * 360.0f)
        {
            elapsedTime += Time.deltaTime;
            float smoothSpeedIncrease = Mathf.SmoothStep(0.0f, _rotationSpeed, elapsedTime / _rotationSpeedApplyTime);
            float angleToAdd = smoothSpeedIncrease * Time.deltaTime;

            if (!_rotatingClockWise) _shootingAngles[shooterIdx] -= angleToAdd;
            else _shootingAngles[shooterIdx] += angleToAdd;
            angleTracker += angleToAdd;
            yield return null;
        }

        _shootingAngles.Clear();
        OnActionFinished.Invoke();
        StopAllCoroutines();
    }

    IEnumerator ShootBullet(int shooterIdx)
    {
        while (true)
        {
            Vector2 direction = new Vector2(Mathf.Cos(_shootingAngles[shooterIdx] * Mathf.Deg2Rad), Mathf.Sin(_shootingAngles[shooterIdx] * Mathf.Deg2Rad));

            var bulletObj = ProjectilePool.Instance.GetProjectile(_projectilePrefab, ProjectileSpeedMultiplier, ProjectileDamageMultiplier, transform.position, direction.normalized);
            yield return new WaitForSeconds(_bulletShootDelay);
        }
    }
}