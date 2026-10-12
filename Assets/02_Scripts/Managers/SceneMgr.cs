using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneMgr : Singleton<SceneMgr>
    {        
        public Grid Map { get; private set;  }
        private int[,] collision; // 0: 갈 수 있음, 1: 충돌물체 있음, 2: 포탈있음
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        // 타이틀씬 로드
        public void LoadTitleScene()
        {
            Action callback = () => SceneManager.LoadSceneAsync("TitleScene");
            AddressableMgr.Instance.StartDefaultAssetLoad(callback);
        }

        // 씬 로드
        public void LoadScene(string sceneName)
        {
            AddressableMgr.Instance.StartLoadAssets(async () => await SceneManager.LoadSceneAsync(sceneName));
        }

        // 맵 교체
        public void UpdateMap(int mapId)
        {
            if (Map != null)
            {
                DestroyMap();
            }

            MakeMap(mapId);
            LeadCollision(mapId);
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

            } while (CanGo(cellPos) == false || ObjectMgr.Instance.Find(cellPos) != null);

            return cellPos;
        }

        // 충돌 체크
        public bool CanGo(Vector3Int cellPos)
        {
            if (cellPos.x < _minX || cellPos.x >= _maxX)
                return false;
            if (cellPos.y < _minY || cellPos.y >= _maxY)
                return false;

            bool canGo = collision[_maxY - cellPos.y - 1, cellPos.x - _minX] != 1;
            return canGo;
        }

        // 포탈 체크
        public bool IsPotal(Vector3Int cellPos)
        {
            if (cellPos.x < _minX || cellPos.x >= _maxX)
                return false;
            if (cellPos.y < _minY || cellPos.y >= _maxY)
                return false;

            int col = collision[_maxY - cellPos.y - 1, cellPos.x - _minX];
            return col == 2;
        }

        // 맵 충돌 확인
        private void LeadCollision(int mapId)
        {
            // Collision 파일 
            string name = mapId.ToString("000");
            TextAsset txt = AddressableMgr.Instance.GetAsset<TextAsset>(name);
            StringReader reader = new StringReader(txt.text);

            _minX = int.Parse(reader.ReadLine());
            _maxX = int.Parse(reader.ReadLine());
            _minY = int.Parse(reader.ReadLine());
            _maxY = int.Parse(reader.ReadLine());

            // 맵 크기
            int sizeX = _maxX - _minX;
            int sizeY = _maxY - _minY;

            collision = new int[sizeY, sizeX];

            for (int y = 0; y < sizeY; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < sizeX; x++)
                {
                    if (int.TryParse(line[x].ToString(), out int num))
                        collision[y, x] = num;
                    else
                        Debug.LogError($"not int {line[x]}");
                }
            }
        }

        private void DestroyMap()
        {
            GameObject obj = Map.gameObject;
            Map = null;
            Destroy(obj);
        }
        
        // 맵 생성
        private void MakeMap(int mapNum)
        {
            if (mapNum == 0)
                return;

            string mapName = "Map_" + mapNum.ToString("000");
            GameObject obj = AddressableMgr.Instance.GetAsset<GameObject>(mapName);
            Instantiate(obj, Vector3.zero, Quaternion.identity);

            Map = obj.GetComponent<Grid>();
        }        
    }
}