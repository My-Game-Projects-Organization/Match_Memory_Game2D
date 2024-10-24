using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteLoader : Singleton<SpriteLoader>
{
    private List<Sprite> loadedSprites = new List<Sprite>();
    public Action<List<Sprite>> OnSpritesLoaded;

    public override void Awake()
    {
        MakeSingleton(true);
    }
    public IEnumerator LoadSprites(string label)
    {
        AsyncOperationHandle<IList<Sprite>> handle = Addressables.LoadAssetsAsync<Sprite>(label, null);

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var sprite in handle.Result)
            {
                loadedSprites.Add(sprite);
                Debug.Log($"Loaded sprite: {sprite.name}");
            }

            OnSpritesLoaded?.Invoke(loadedSprites);
        }
        else
        {
            Debug.LogError("Failed to load sprites!");
        }
    }

    public void ReleaseAssets()
    {
        Addressables.Release(loadedSprites);
        loadedSprites.Clear();
    }
}
