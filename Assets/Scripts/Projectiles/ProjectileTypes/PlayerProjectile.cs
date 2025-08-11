using UnityEngine;

public class PlayerProjectile : Projectile
{
    [SerializeField] private float _speed = 5.0f;
    private void Update()
    {
        transform.position += _speed * SpeedMultiplier * Time.deltaTime * ShotDirection;
    }
}
