using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using JetBrains.Annotations;

public class BossDataStorer : MonoBehaviour
{
    private List<BossData> _bosses;
    private List<List<int>> _bossesPerLevel = new List<List<int>>();

    private void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        AsyncOperationHandle<IList<BossData>> handle = Addressables.LoadAssetsAsync<BossData>("BossData", null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _bosses = new List<BossData>(handle.Result);
            SortBossesPerLevel();
        }
        else
        {
            Debug.LogError("Loading BossData assets failed");
        }
    }

    private void SortBossesPerLevel()
    {
        for(int bossIdx = 0; bossIdx < _bosses.Count; ++bossIdx)
        {
            foreach(BossVersionData bossVersion in _bosses[bossIdx].Versions)
            {
                int bossLevel = bossVersion.Level;
                while(_bossesPerLevel.Count < bossLevel)
                {
                    _bossesPerLevel.Add(new List<int>());
                }
                _bossesPerLevel[bossLevel - 1].Add(bossIdx);
            }
        }
    }

    public BossData GetRandomBoss(int level)
    {
        int randomBoss = Random.Range(0, _bossesPerLevel[level].Count);
        return _bosses[_bossesPerLevel[level][randomBoss]];
    }
}