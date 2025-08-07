using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ProjectileRowToPlayerAttack : BossAttack
{
    [SerializeField] private GameObject _projectileToShoot;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private int _projectilesPerRow = 4;
    [SerializeField] private float _delayBetweenProjectilesInRow = 0.05f;
    [SerializeField] private int _amountOfRowsToShoot = 5;
    [SerializeField] private float _delayBetweenRows = 0.5f;

    public override void StartAction()
    {
        StartCoroutine(ShootProjectileRows());
    }

    public override void StopAction()
    {
        StopAllCoroutines();
    }

    private IEnumerator ShootProjectileRows()
    {
        for(int rowIdx = 0; rowIdx < _amountOfRowsToShoot; ++rowIdx)
        {
            Vector3 playerPosition = _playerTransform.position;
            for(int projectileIdx = 0; projectileIdx < _projectilesPerRow; ++projectileIdx)
            {
                var projectileObj = ProjectilePool.Instance.GetProjectile(_projectileToShoot);
                TestProjectile projectile = projectileObj.GetComponent<TestProjectile>();
                projectile.transform.position = transform.position;
                projectile.transform.right = (playerPosition - transform.position).normalized;
                projectile.speedMultiplier = _projectileSpeedMultiplier;
                if (projectileIdx == _projectilesPerRow -1) break;
                yield return new WaitForSeconds(_delayBetweenProjectilesInRow);
            }
            if(rowIdx == _amountOfRowsToShoot - 1) break;
            yield return new WaitForSeconds(_delayBetweenRows);
        }

        OnActionFinished.Invoke();
    }
}
