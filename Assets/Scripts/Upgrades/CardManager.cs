using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardManager : MonoBehaviour
{
    private List<UpgradeCard> _allCards;
    private List<List<int>> _cardsByRarity = new List<List<int>>();
    private List<UpgradeCard> _heldCards = new List<UpgradeCard>();
    private int _numberOfRarities = 3;

    private Dictionary<int, List<int>> _cardDependencies = new Dictionary<int, List<int>>(); // Dependencies of card
    private Dictionary<int, List<int>> _reverseCardDependencies = new Dictionary<int, List<int>>(); // Cards that depend on this card
    private HashSet<int> _availableCards = new HashSet<int>();

    private PlayerShootingHandler _playerShootingHandler;

    private void Start()
    {
        InitializeCards();
    }

    public async void InitializeCards()
    {
        _playerShootingHandler = FindAnyObjectByType<PlayerShootingHandler>();

        AsyncOperationHandle<IList<UpgradeCard>> handle = Addressables.LoadAssetsAsync<UpgradeCard>("UpgradeCardData", null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _allCards = new List<UpgradeCard>(handle.Result);
            SortCardsByRarityAndDependencies();
            InitializeAvailableCards();
            PickUpCard(GetRandomCard(new List<int> {5, 10, 20}));
        }
        else
        {
            Debug.LogError("Loading BossData assets failed");
        }
    }

    private void SortCardsByRarityAndDependencies()
    {
        _cardsByRarity.Clear();
        _cardDependencies.Clear();
        _reverseCardDependencies.Clear();

        for (int i = 0; i < _numberOfRarities; ++i)
        {
            _cardsByRarity.Add(new List<int>());
        }
        for(int cardIdx = 0; cardIdx < _allCards.Count; ++cardIdx)
        {
            UpgradeCard currentCard = _allCards[cardIdx];
            _cardsByRarity[currentCard.Rarity - 1].Add(cardIdx);
            if (currentCard.DependencyCards.Count > 0)
            {
                List<int> dependencyIndices = new List<int>();
                foreach(var dependency in currentCard.DependencyCards)
                {
                    int dependencyIdx = _allCards.IndexOf(dependency);
                    dependencyIndices.Add(dependencyIdx);

                    // Build reverse dependencies
                    if(!_reverseCardDependencies.ContainsKey(dependencyIdx))
                    {
                        _reverseCardDependencies[dependencyIdx] = new List<int>();
                    }
                    _reverseCardDependencies[dependencyIdx].Add(cardIdx);
                }
                _cardDependencies[cardIdx] = dependencyIndices;
            }
        }
    }
    private void InitializeAvailableCards()
    {
        _availableCards.Clear();
        for (int cardIdx = 0; cardIdx < _allCards.Count; cardIdx++)
        {
            if (!_cardDependencies.ContainsKey(cardIdx))
            {
                _availableCards.Add(cardIdx);
            }
        }
    }

    private void CheckNewUnlocks(int pickedUpIdx)
    {
        if (!_reverseCardDependencies.ContainsKey(pickedUpIdx)) return; // No cards rely on this card

        foreach(var dependantCardIdx in _reverseCardDependencies[pickedUpIdx]) // Loop over all cards that rely on this card
        {
            bool allDependenciesCollected = true;
            foreach(var dependency in _cardDependencies[dependantCardIdx]) // Loop over all dependencies of card that relies on this card
            {
                if(!_heldCards.Contains(_allCards[dependency])) // Check if holding current dependency to check
                {
                    allDependenciesCollected = false;
                    break;
                }
            }
            if(allDependenciesCollected) // All dependency cards for this card are held by the player
            {
                _availableCards.Add(dependantCardIdx);
            }
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

        CheckNewUnlocks(_allCards.IndexOf(card));
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
