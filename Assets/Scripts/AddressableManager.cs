using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : Singleton<AddressableManager>
{
    private Dictionary<string, AsyncOperationHandle> loadedAssets = new Dictionary<string, AsyncOperationHandle>();
    public List<Sprite> loadedSprites = new List<Sprite>();

    public override void Awake()
    {
        MakeSingleton(true);
    }
    public IEnumerator LoadSprites(string label)
    {
        if (!loadedAssets.ContainsKey(label))
        {
            AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(label, null);

            yield return handle;
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    foreach (var sprite in op.Result)
                    {
                        loadedSprites.Add(sprite);
                        Debug.Log($"Loaded sprite: {sprite.name}");
                    }
                    loadedAssets[label] = op;
                    //OnSpritesLoaded?.Invoke(loadedSprites);
                }
                else
                {
                    Debug.LogError("Failed to load sprites!");
                }
            };
        }
    }

    public List<Sprite> GetLoadedAsset(string label)
    {
        if (loadedAssets.TryGetValue(label, out AsyncOperationHandle handle))
        {
            return handle.Result as List<Sprite>; 
        }
        Debug.LogError($"Asset {label} not found or not loaded.");
        return null;
    }

    public void ReleaseAsset(string address)
    {
        if (loadedAssets.TryGetValue(address, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            loadedAssets.Remove(address);
            loadedSprites.Clear();
            Debug.Log($"Released: {address}");
        }
        else
        {
            Debug.LogWarning($"Asset {address} was not loaded, cannot release.");
        }
    }
    public void ReleaseAllAssets()
    {
        foreach (var handle in loadedAssets.Values)
        {
            Addressables.Release(handle); 
        }
        loadedAssets.Clear();
        loadedSprites.Clear();
        Debug.Log("All assets released.");
    }
}
