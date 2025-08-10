using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class BossDataStorer : MonoBehaviour
{
    private List<BossData> _bosses;

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
            for (int i = 0; i < 20; ++i)
            {
                BossData boss = GetRandomBoss();
                Debug.Log(boss.Name + " " + boss.ID);
            }
        }
        else
        {
            Debug.LogError("Loading BossData assets failed");
        }
    }

    public BossData GetRandomBoss()
    {
        int randomBoss = Random.Range(0, _bosses.Count);
        return _bosses[randomBoss];
    }
}
