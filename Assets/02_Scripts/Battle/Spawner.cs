using Manager;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private BattleScene _battleScene;
    private ObjectPool _pool;
    private bool _isSpawn = false;
    private float _spawnTimer;
    private Dictionary<int, int> _spawnData = new(); // 현재 맵 몬스터 데이터 (MonsterId, Count)
    private Dictionary<int, List<MonsterController>> _aliveMonsters = new(); // 맵에서 사용 중인 몬스터

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

    public void Set(BattleScene b, ObjectPool p)
    {
        _battleScene = b;
        _pool = p;
    }

    public void InitSpawn(Dictionary<int, int> spawnData)
    {
        ResetSpwaner();

        _spawnData = spawnData;
        StartSpwan();
    }

    public void ResetSpwaner()
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
            MonsterController mc = _pool.Get(id);
            mc.CellPos = SceneMgr.Instance.CurMap.RandomPos();
            mc.OnDead += HandleMonsterDead;
            mc.Init();

            _aliveMonsters[id].Add(mc);
            SceneMgr.Instance.Monsters.Add(mc);
        }
    }

    private void Despwan(MonsterController mc)
    {
        mc.OnDead -= HandleMonsterDead;

        _pool.Return(mc.MonsterId, mc);
        _aliveMonsters[mc.MonsterId].Remove(mc);
        SceneMgr.Instance.Monsters.Remove(mc);

        _isSpawn = true;
    }

    private void HandleMonsterDead(MonsterController mc)
    {
        Despwan(mc);
    }
}