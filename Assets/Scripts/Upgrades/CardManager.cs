using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    private List<UpgradeCard> _allCards;
    private List<UpgradeCard> _heldCards = new List<UpgradeCard>();

    private PlayerShootingHandler _playerShootingHandler;

    private void Start()
    {
        InitializeCards();
    }

    public async void InitializeCards()
    {
        // Fill up all cards
        _playerShootingHandler = FindAnyObjectByType<PlayerShootingHandler>();

        AsyncOperationHandle<IList<UpgradeCard>> handle = Addressables.LoadAssetsAsync<UpgradeCard>("UpgradeCardData", null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _allCards = new List<UpgradeCard>(handle.Result);
            PickUpCard(_allCards[0]);
        }
        else
        {
            Debug.LogError("Loading BossData assets failed");
        }
    }

    public void PickUpCard(UpgradeCard card)
    {
        _heldCards.Add(card);
        foreach(var shotUpgrade in card.ShotUpgrades)
        {
            _playerShootingHandler.ShotUpgrades.Add(shotUpgrade);
        }
        foreach(var projectileUpgrade in card.ProjectileUpgrades)
        {
            _playerShootingHandler.ProjectileUpgrades.Add(projectileUpgrade);
        }
        foreach(var statUpgrade in card.StatUpgrades)
        {
            statUpgrade.ApplyUpgrade();
        }
    }
}
