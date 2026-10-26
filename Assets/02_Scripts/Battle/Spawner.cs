using Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool _isSpawn = false;
    private float _spawnTimer;
    private Dictionary<int, int> _spawnData = new(); // 현재 맵 몬스터 데이터 (MonsterId, Count)
    private Dictionary<int, List<MonsterController>> _aliveMonsters = new(); // 맵에서 사용 중인 몬스터
    private Action<GameObject> _spawnCallback;
    private Action<GameObject> _deSpawnCallback;
    private Func<Vector3Int> _randomPos;

    private void Update()
    {
        if (_isSpawn)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer > 3f)
            {
                _isSpawn = false;
                _spawnTimer = 0f;
                StartSpwan();
            }
        }
    }

    public void InitSpawn(Dictionary<int, int> spawnData, Action<GameObject> callback, Action<GameObject> desCallback, Func<Vector3Int> randomPos)
    {
        ResetSpwaner();

        _spawnData = spawnData;
        _spawnCallback = callback;
        _deSpawnCallback = desCallback;
        _randomPos = randomPos;

        StartSpwan();
    }

    private void ResetSpwaner()
    {
        foreach (var monsterList in _aliveMonsters.Values)
        {
            if (monsterList.Count > 0)
            {
                for (int i = 0; i < monsterList.Count; i++)
                {
                    Despwan(monsterList[0]);
                }
            }
        }

        _isSpawn = false;
        _spawnData.Clear();
    }

    private void StartSpwan()
    {
        foreach (var data in _spawnData)
        {
            int id = data.Key;

            if (_aliveMonsters.TryGetValue(id, out var monsterList) == false)
            {
                monsterList = new();
                _aliveMonsters.Add(id, monsterList);
            } 
            
            if (_spawnData[id] > monsterList.Count)
            {
                _isSpawn = true;
                Spwan(id, data.Value - monsterList.Count);
            }
        }
    }

    private void Spwan(int id, int count)
    {
        if (count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            MonsterController mc = ObjectPool.Instance.Get(id);
            mc.CellPos = _randomPos();
            mc.OnDead += HandleMonsterDead;
            mc.Init();

            _aliveMonsters[id].Add(mc);
            _spawnCallback?.Invoke(mc.gameObject);
        }
    }

    private void Despwan(MonsterController mc)
    {
        mc.OnDead -= HandleMonsterDead;

        _aliveMonsters[mc.MonsterId].Remove(mc);
        ObjectPool.Instance.Add(mc.MonsterId, mc);
        _deSpawnCallback?.Invoke(mc.gameObject);        

        _isSpawn = true;
    }

    private void HandleMonsterDead(MonsterController mc)
    {
        Despwan(mc);
    }
}

public class MonsterMgr
{
    private List<GameObject> _monsters = new List<GameObject>();

    public void Add(GameObject obj)
    {
        _monsters.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        _monsters.Remove(obj);
    }

    public void Clear()
    {
        _monsters.Clear();
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (var obj in _monsters)
        {
            if (obj.activeSelf == false)
                continue;

            if (obj.TryGetComponent<CharaController>(out var cc) == false)
            {
                Debug.LogError($"cc null error: {obj.name}");
                continue;
            }
            else
            {
                if (cc.CellPos == cellPos)
                    return obj;
            }
        }

        return null;
    }
}