using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiShotUpgrade", menuName = "Upgrades/ShotUpgrades/MultiShotUpgrade")]
public class MultiShotUpgrade : ShotUpgradeSO
{
    [SerializeField] private int _amountOfExtraShots = 1;
    [SerializeField] private float _minShootingAngle = -45.0f;
    [SerializeField] private float _maxShootingAngle = 45.0f;

    public override void ModifyShots(List<ShotData> shots)
    {
        float centerAngle = _minShootingAngle + _maxShootingAngle;
        for(int i = 0; i < _amountOfExtraShots; ++i)
        {
            shots.Add(shots[0].Clone());
        }

        int amountOfPairs = shots.Count / 2;
        int leftOver = shots.Count % 2;
        float angleToPlaceAt = (((_maxShootingAngle - centerAngle) / 2.0f )/ amountOfPairs) * Mathf.Deg2Rad;
        for(int pairIdx = 0; pairIdx < amountOfPairs; ++pairIdx)
        {
            float angle = angleToPlaceAt * (pairIdx +1);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            direction.Normalize();
            shots[2 * pairIdx].Direction = direction;
            direction.y *= -1;
            shots[2 * pairIdx +1].Direction = direction;
        }
        if(leftOver > 0)
        {
            shots[shots.Count - 1].Direction = new Vector2(Mathf.Cos(centerAngle), Mathf.Sin(centerAngle)).normalized;
        }
    }
}
