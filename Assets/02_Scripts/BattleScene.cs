using Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    private Spawner _spawner;
    private ObjectPool _pool;
    private PlayerController _player;


    private void Start()
    {
        if (TryGetComponent<Spawner>(out _spawner) == false)
        {
            Debug.LogError("Null Spawner");
            return;
        }
        if (TryGetComponent<ObjectPool>(out _pool) == false)
        {
            Debug.LogError("Null Spawner");
            return;
        }

        _spawner.Set(this, _pool);
        MakeMap(SceneMgr.Instance.CurMap.MapId);
    }

    private async void MakeMap(int mapID)
    {
        await SceneMgr.Instance.ChangeMap();

        MakeMonster(mapID);

        if (_player == null)
            MakePlayer();
    }

    private void MakePlayer()
    {
        GameObject asset = Manager.AddressableMgr.Instance.GetAsset<GameObject>("Player");
        if (asset == null)
            return;

        GameObject obj = Instantiate(asset);

        if (obj.TryGetComponent<PlayerController>(out _player))
        {
            _player.CellPos = SceneMgr.Instance.SpwanPos;
            SceneMgr.Instance.Monsters.Add(_player);
        }
    }

    private void MakeMonster(int mapId)
    {
        List<Monster> monsterDataList = DBMgr.Map_TB.Monster.Values.ToList().FindAll(x => x.MapIdx == mapId);
        Dictionary<int, int> spawnData = new();

        foreach (var monsterData in monsterDataList)
        {
            _pool.SetPool(new ObjectPool.Entry() { id = monsterData.Idx, count = monsterData.Count });
            spawnData.Add(monsterData.Idx, monsterData.Count);   
        }
        _spawner.InitSpawn(spawnData);
    }
}
