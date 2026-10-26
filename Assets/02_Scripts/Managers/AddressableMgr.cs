using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

namespace Manager
{
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
                type != typeof(Grid) &&
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

        private readonly Dictionary<string, UnityEngine.Object> _mapAsset = new Dictionary<string, UnityEngine.Object>();
        private AsyncOperationHandle<IList<GameObject>> _mapHandle;


        // 기본 에셋 다운로드 사이즈
        public async void GetDownLoadSize(Action<long> callbacck)
        {
            var size = Addressables.GetDownloadSizeAsync("default");
            await size.Task;

            long sizeByte = size.Result / 1024 * 1024;
            Debug.Log($"size : {size.Result}, {sizeByte}");

            callbacck.Invoke(sizeByte);
        }

        // 기본 에셋 다운
        public async void StartDefaultAssetLoad(Action callback)
        {
            var handle = Addressables.DownloadDependenciesAsync("default", false); // false: 수동 Release

            while (!handle.IsDone)
            {
                DownloadStatus status = handle.GetDownloadStatus();
                float percent = (float)status.DownloadedBytes / status.TotalBytes;
                Debug.Log($"{status.DownloadedBytes} / {status.TotalBytes} bytes ({percent:P0})");
                await Task.Yield();
            }

            Addressables.Release(handle); // 수동으로 넘겼으니 직접 해제
            callback.Invoke();
        }

        #region Self Assets
        // 로드할 에셋 수동으로 추가
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

        // 수동으로 추가한 에셋 로드해서 딕셔너리에 저장
        public async Task StartLoadAssets()
        {
            List<AssetData> list;

            lock (_lockObj)
            {
                if (_loadingAssets == null || _loadingAssets.Count == 0)
                {
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
        }

        // 수동으로 추가한 에셋 가져오기
        public T GetAsset<T>(string name) where T : UnityEngine.Object
        {
            if (_loadedAssets.TryGetValue(name, out var asset) == false)
            {
                Debug.LogError($"Not Found Asset: {name}");
                return null;
            }

            var result = asset as T;

            if (result != null)
                return result;

            if (asset is GameObject gameObject)
            {
                result = gameObject.GetComponent<T>();
                return result;
            }

            Debug.LogError($"GetAsset type mismatch - {name}, expected: {typeof(T).Name}, type: {asset.GetType().Name}");
            return null;
        }

        // 수동 로드 에셋 메모리에서 해제
        public void ReleaseAsset(string key)
        {
            Debug.Log("Addressable ReleaseAsset - " + key);

            if (_loadedAssets.TryGetValue(key, out var obj))
            {
                _loadedAssets.Remove(key);
                Addressables.Release(obj);
            }
        }

        // 모든 수동 로드 에셋 메모리에서 해제
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
        #endregion

        // 맵 넘어갈때 에셋 관리
        public async Task LoadAssetByMap(int mapID)
        {
            Debug.Log($"Start Load Asset : map_{mapID}");
            _mapHandle = Addressables.LoadAssetsAsync<GameObject>(mapID.ToString(), OnAssetLoaded);
            _mapHandle.Completed += OnLoadCompleted;

            await _mapHandle.Task;
        }

        // 에셋이 하나씩 로드될 때마다 호출됨
        void OnAssetLoaded(GameObject prefab)
        {
            if (prefab != null)
            {
                _mapAsset.Add(prefab.name, prefab);
                // Debug.Log($"에셋 로드 완료: {prefab.name}");
            }
        }

        // 지정된 라벨/그룹의 모든 에셋 로드가 완전히 끝났을 때 호출됨
        void OnLoadCompleted(AsyncOperationHandle<IList<GameObject>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"총 {handle.Result.Count}개의 에셋을 성공적으로 불러왔습니다. AssetCount : {_mapAsset.Count}");
            }
            else
            {
                Debug.LogError($"로드 실패: {handle.OperationException}");
            }
        }


        public T GetMapAsset<T>(string key) where T : UnityEngine.Object
        {
            if (_mapAsset.TryGetValue(key, out var obj) == false)
            {
                Debug.LogError($"MapAsset null error : {key}");
                return default;
            }

            var result = obj as T;

            if (result != null)
            {
                return result;
            }

            if (obj is GameObject gameObject)
            {
                result = gameObject.GetComponent<T>();
                return result;
            }

            Debug.LogError($"GetAsset type mismatch - {key}, expected: {typeof(T).Name}, type: {obj.GetType().Name}");
            return null;
        }

        // 맵 에셋 메모리에서 해제
        public void UnloadMap()
        {
            if (_mapAsset.Count > 0)
            {
                _mapAsset.Clear();
                Addressables.Release(_mapHandle);
                _mapHandle = default;
            }
        }
    }
}