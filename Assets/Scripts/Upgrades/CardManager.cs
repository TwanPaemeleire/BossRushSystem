using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    private List<UpgradeCard> _allCards;
    private List<List<int>> _cardsByRarity;
    private List<UpgradeCard> _heldCards = new List<UpgradeCard>();
    private int _numberOfRarities = 3;

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
            SortCardsByRarity();
            PickUpCard(GetRandomCard(new List<int> {5, 10, 20}));
        }
        else
        {
            Debug.LogError("Loading BossData assets failed");
        }
    }

    private void SortCardsByRarity()
    {
        _cardsByRarity = new List<List<int>>();
        for(int i = 0; i < _numberOfRarities; ++i)
        {
            _cardsByRarity.Add(new List<int>());
        }
        for(int cardIdx = 0; cardIdx < _allCards.Count; ++cardIdx)
        {
            _cardsByRarity[_allCards[cardIdx].Rarity - 1].Add(cardIdx);
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

    public void RemoveCard(UpgradeCard card)
    {
        if(!_heldCards.Contains(card))
        {
            Debug.LogWarning("Trying to remove card that is not held");
            return;
        }
        _heldCards.Remove(card);
        foreach (var shotUpgrade in card.ShotUpgrades)
        {
            _playerShootingHandler.ShotUpgrades.Remove(shotUpgrade);
        }
        foreach (var projectileUpgrade in card.ProjectileUpgrades)
        {
            _playerShootingHandler.ProjectileUpgrades.Remove(projectileUpgrade);
        }
        foreach (var statUpgrade in card.StatUpgrades)
        {
            statUpgrade.RemoveUpgrade();
        }
    }

    public UpgradeCard GetRandomCard(List<int> rarityWeights)
    {
        if(rarityWeights.Count > _numberOfRarities)
        {
            Debug.LogWarning("Number of rarity weights passed exceeds number of rarities");
            return null;
        }
        int totalWeight = 0;
        foreach(var rarityWeight in rarityWeights)
        {
            totalWeight += rarityWeight;
        }

        int randomNumber = Random.Range(0, totalWeight);
        int currentWeightSum = 0;
        for(int rarityWeightIdx = 0; rarityWeightIdx < rarityWeights.Count; ++ rarityWeightIdx)
        {
            currentWeightSum += rarityWeights[rarityWeightIdx];
            if(randomNumber < currentWeightSum)
            {
                int numberOfCardsInRarity = _cardsByRarity[rarityWeightIdx].Count;
                int randomCardIdx = Random.Range(0, numberOfCardsInRarity);
                Debug.Log("PICKED UP CARD: " + _allCards[_cardsByRarity[rarityWeightIdx][randomCardIdx]].name);
                return _allCards[_cardsByRarity[rarityWeightIdx][randomCardIdx]];
            }
        }
        return null;
    }

}
