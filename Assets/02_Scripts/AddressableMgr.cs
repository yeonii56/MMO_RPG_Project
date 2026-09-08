using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class AssetData
{
    public string AssetName;
    public Type AssetType;

    public AssetData(string name, Type type)
    {
        AssetName = name;
        AssetType = type;

        if (type != null &&
            type != typeof(GameObject) &&
            type != typeof(TextAsset) &&
            type != typeof(SpriteAtlas) &&
            type != typeof(AudioClip) &&
            type != typeof(Tilemap) &&
            type != typeof(Texture))
        {
            AssetType = typeof(GameObject);
        }
    }
}

public class AddressableMgr : Singleton<AddressableMgr>
{
    private readonly Dictionary<string, UnityEngine.Object> _loadedAssets = new Dictionary<string, UnityEngine.Object>();
    private readonly List<AssetData> _loadingAssets = new List<AssetData>();
    private readonly object _lockObj = new object();
    private string _curMap = string.Empty;

    // 씬 로드
    public void LoadScene(string sceneName, string map)
    {
        ReleaseAsset(_curMap);
        
        _curMap = map;
        AddLoadAssets(_curMap, typeof(Tilemap));
        StartLoadAssets(() => Addressables.LoadSceneAsync(sceneName));
    }

    // 로드할 에셋 추가
    public void AddLoadAssets(string name, Type type)
    {
        if (string.IsNullOrEmpty(name) || _loadedAssets.ContainsKey(name))
            return;

        lock (_lockObj)
        {
            _loadingAssets.Add(new AssetData(name, type));
        }
    }
    
    public void AddLoadAssets(List<AssetData> assetList)
    {
        if (assetList == null || assetList.Count == 0)
            return;

        foreach (AssetData assetData in assetList)
        {
            AddLoadAssets(assetData.AssetName, assetData.AssetType);
        }
    }

    // 에셋 로드해서 딕셔너리에 저장
    public async void StartLoadAssets(Action callback)
    {
        List<AssetData> list;

        lock(_lockObj)
        {
            if (_loadingAssets == null || _loadingAssets.Count == 0)
            {
                callback?.Invoke();
                return;
            }

            list = new List<AssetData>(_loadingAssets);
        }

        foreach (AssetData assetData in list)
        {
            if (_loadedAssets.ContainsKey(assetData.AssetName))
                continue;

            var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetData.AssetName);
            await handle.Task;

            _loadedAssets.Add(assetData.AssetName, handle.Result);
        }

        callback?.Invoke();
    }

    // 에셋 가져오기
    public T GetAsset<T>(string name) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(name, out var asset) == false)
        {
            return null;
        }

        var result = asset as T;

        if (result != null)
            return result;

        if  (asset is GameObject gameObject)
        {
            result = gameObject.GetComponent<T>();
            return result;
        }

        Debug.LogError($"GetAsset type mismatch - {name}, expected: {typeof(T).Name}, type: {asset.GetType().Name}");
        return null;
    }

    // 메모리에서 해제
    public void ReleaseAsset(string key)
    {
        Debug.Log("Addressable ReleaseAsset - " + key);

        if (_loadedAssets.TryGetValue(key, out var obj))
        {
            _loadedAssets.Remove(key);
            Addressables.Release(obj);
        }
    }

    // 모든 로드 에셋 메모리에서 해제
    public void DisposeAsset()
    {
        // foreach로 Values 반복 시 Enumerator 할당을 최소화
        var assets = new List<UnityEngine.Object>(_loadedAssets.Values);
        foreach (var obj in assets)
        {
            Addressables.Release(obj);
        }
        _loadedAssets.Clear();
    }
}
