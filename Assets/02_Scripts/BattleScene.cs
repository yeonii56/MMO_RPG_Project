using Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;

    public MonsterMgr Monsters { get; private set; } = new();
    private PlayerController _player;


    private void Start()
    {
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
        }
    }

    private void MakeMonster(int mapId)
    {
        List<Monster> monsterDataList = DBMgr.Map_TB.Monster.Values.ToList().FindAll(x => x.MapIdx == mapId);
        Dictionary<int, int> spawnData = new();

        foreach (var monsterData in monsterDataList)
        {
            ObjectPool.Instance.SetPool(new ObjectPool.Entry() { id = monsterData.Idx, count = monsterData.Count });
            spawnData.Add(monsterData.Idx, monsterData.Count);
        }

        InitSpawner(spawnData);
    }

    private void InitSpawner(Dictionary<int, int> spawnData)
    {
        _spawner.InitSpawn(spawnData, Monsters.Add, Monsters.Remove, GetRandomPos);
    }

    private Vector3Int GetRandomPos()
    {
        Vector3Int pos;
        do
        {
            pos = SceneMgr.Instance.CurMap.RandomPos();
        }
        while (Monsters.Find(pos) != null);

        return pos;
    }
}
