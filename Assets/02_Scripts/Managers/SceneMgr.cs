using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        public MapData CurMap;
        public Vector3Int SpwanPos;

        private Dictionary<int, MapData> _mapData = new();
        private Grid _grid;
        private GameObject _map;
        private bool _isChangeMap = false;
        private bool _isLoading = false;

        // 씬 로드

        public async Task LoadScene(string sceneName)
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                await AddressableMgr.Instance.StartLoadAssets();

                var op = SceneManager.LoadSceneAsync(sceneName);
                if (op == null)
                    throw new InvalidOperationException($"Scene not found : {sceneName}");

                await op;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                AddressableMgr.Instance.UnloadMap();
            }
            finally
            {
                _isLoading = false;
            }
        }

        // 맵 교체
        public async Task ChangeMap()
        {
            if (_isChangeMap)
                return;

            try
            {
                _isChangeMap = true;

                // 맵 제거
                if (_map != null)
                {
                    Destroy(_map);
                    _map = null;
                }

                // 에셋 로드
                AddressableMgr.Instance.UnloadMap();
                await AddressableMgr.Instance.LoadAssetByMap(CurMap.MapId);

                // 맵 생성            
                if (DBMgr.Map_TB.MapList.TryGetValue(CurMap.MapId, out var mapData) == false)
                {
                    Debug.LogError("Not Found Map : " + CurMap.MapId);
                    return;
                }
                var prefab = AddressableMgr.Instance.GetMapAsset<GameObject>(mapData.Prefab);
                _map = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                if (_map.TryGetComponent<Grid>(out _grid) == false)
                {
                    Debug.LogError("grid component null error");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _isChangeMap = false;
            }           
        }

        // 맵 데이터 셋팅
        public void SetMapData(int mapId)
        {
            if (CurMap != null && mapId == CurMap.MapId)
                return;

            if (_mapData.TryGetValue(mapId, out CurMap) == false)
            {
                CurMap = new MapData(mapId);
                _mapData.Add(mapId, CurMap);
            }
        }
        
        public Vector3 GetCelltoWorld(Vector3Int cellPos)
        {
            if (_grid == null)
            {
                Debug.LogError("_grid is null");
                return default;
            }

            return _grid.CellToWorld(cellPos);
        }       
    }

    public class MapData
    {        
        public int MapId { get; private set; }
        private Dictionary<Vector3Int, TileData> _interactTiles = new(); // 상호작용 타일
        private int[,] _collision; // 충돌 체크
        private int _minX, _maxX, _minY, _maxY;

        public MapData(int mapId)
        {
            MapId = mapId;
            Init();
        }

        public async void Init()
        {
            SetInteractData(MapId);
            await LeadCollision(MapId);
        }

        public Vector3Int RandomPos()
        {
            Vector3Int cellPos;

            do
            {
                cellPos = new Vector3Int()
                {
                    x = UnityEngine.Random.Range(_minX, _maxX),
                    y = UnityEngine.Random.Range(_minY, _maxY)
                };

            } while (CanGo(cellPos) == false);

            return cellPos;
        }

        // 충돌 체크
        public bool CanGo(Vector3Int cellPos)
        {
            if (cellPos.x < _minX || cellPos.x >= _maxX)
                return false;
            if (cellPos.y < _minY || cellPos.y >= _maxY)
                return false;

            bool canGo = _collision[_maxY - cellPos.y - 1, cellPos.x - _minX] != 1;
            return canGo;
        }

        // 상호작용 체크
        public bool CanInteract(Vector3Int cellPos)
        {
            if (_interactTiles.TryGetValue(cellPos, out var tileData))
            {
                tileData.Interact();
                return true;
            }

            return false;
        }

        // 상호작용 데이터 셋팅
        private void SetInteractData(int mapId)
        {
            if (DBMgr.Map_TB.MapList.TryGetValue(mapId, out MapList mapList) == false)
            {
                Debug.LogError("Map_TB MapList error : " + mapId);
                return;
            }

            // 포탈
            foreach (var potal in DBMgr.Map_TB.PotalList.Values)
            {
                if (potal.MapIdx == mapId)
                {
                    _interactTiles.Add(new Vector3Int(potal.PosX, potal.PosY, 0), new Potal(potal.Idx));
                }
            }
        }

        // 맵 범위, 충돌 확인
        private async Task LeadCollision(int mapId)
        {
            // Collision 파일
            string key = string.Format("MapCollision_{0}", mapId);
            var handle = Addressables.LoadAssetAsync<TextAsset>(key);

            TextAsset txt = await handle.Task;
            StringReader reader = new StringReader(txt.text);

            _minX = int.Parse(reader.ReadLine());
            _maxX = int.Parse(reader.ReadLine());
            _minY = int.Parse(reader.ReadLine());
            _maxY = int.Parse(reader.ReadLine());

            // 맵 크기
            int sizeX = _maxX - _minX;
            int sizeY = _maxY - _minY;

            _collision = new int[sizeY, sizeX];

            for (int y = 0; y < sizeY; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < sizeX; x++)
                {
                    if (int.TryParse(line[x].ToString(), out int num))
                        _collision[y, x] = num;
                    else
                        Debug.LogError($"not int {line[x]}");
                }
            }

            Addressables.Release(handle);
        }


    }

    public class TileData
    {
        public TileType Type;

        public enum TileType
        {
            None,
            Potal,

        }

        public virtual void Interact()
        {

        }
    }

    public class Potal: TileData
    {
        private int _movePotalIdx;
        private string _sceneName;
        private Vector3Int _spwanPos;
        private int _moveMapIdx;

        public Potal(int potalIdx)
        {
            Type = TileType.Potal;
            Init(potalIdx);
        }

        public void Init(int potalIdx)
        {
            if (DBMgr.Map_TB.PotalList.TryGetValue(potalIdx, out var potal))
            {
                _movePotalIdx = potal.MovePotal;
            }

            if (DBMgr.Map_TB.PotalList.TryGetValue(_movePotalIdx, out var movePotal))
            {
                _spwanPos = new Vector3Int(movePotal.PosX, movePotal.PosY);
            }

            if (DBMgr.Map_TB.MapList.TryGetValue(movePotal.MapIdx, out var map))
            {
                _moveMapIdx = map.Idx;
                _sceneName = map.Scene;
            }


        }

        public override void Interact()
        {
            Debug.Log("포탈!!!!! : " + _sceneName + " : " + _moveMapIdx);

            if (DBMgr.Map_TB.MapList.TryGetValue(SceneMgr.Instance.CurMap.MapId, out var map))
            {
                // 맵 데이터 교체
                SceneMgr.Instance.SetMapData(_moveMapIdx);
                SceneMgr.Instance.SpwanPos = _spwanPos;

                // 씬 교체
                if (map.Scene != _sceneName)
                {
                    SceneMgr.Instance.LoadScene(_sceneName);
                }
            }
        }
    }
}