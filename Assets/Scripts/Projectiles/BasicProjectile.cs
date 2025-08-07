using UnityEngine;

public class BasicProjectile : Projectile
{
    [SerializeField] private float _speed = 5.0f;
    private void Update()
    {
        transform.position += _speed * SpeedMultiplier * Time.deltaTime * ShotDirection;
    }
}
