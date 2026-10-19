using Manager;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool _isSpawn = false;
    private float _spawnTimer;
    private int _monsterId;
    private int _monsterCnt;
    private List<MonsterController> _list = new();

    private void Update()
    {
        if (_isSpawn)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer > 3f)
            {
                _spawnTimer = 0f;
                Spwan();
            }
        }
    }

    public void ResetSpwaner()
    {
        if (_list.Count > 0)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                Despwan(_list[0]);
            }
        }

        _isSpawn = false;
        _monsterId = 0;
        _monsterCnt = 0;
    }

    public void InitSpawn(int id, int count)
    {
        ResetSpwaner();

        _monsterId = id;
        _monsterCnt = count;

        Spwan(count);
    }

    private void Spwan()
    {
        if (_monsterCnt <= _list.Count)
        {
            _isSpawn = false;
            _spawnTimer = 0;
            return;
        }

        Spwan(_monsterCnt - _list.Count);
    }

    private void Spwan(int count)
    {
        for (int i = 0; i < count; i++)
        {
            MonsterController mc = ObjectPool.Instance.Get(_monsterId);
            mc.CellPos = SceneMgr.Instance.CurMap.RandomPos();
            mc.OnDead += HandleMonsterDead;
            mc.Init();

            _list.Add(mc);
            SceneMgr.Instance.MonsterState.Add(mc.gameObject);
        }
    }

    private void Despwan(MonsterController mc)
    {
        mc.OnDead -= HandleMonsterDead;

        _list.Remove(mc);
        ObjectPool.Instance.Add(_monsterId, mc);
        SceneMgr.Instance.MonsterState.Remove(mc.gameObject);        

        _isSpawn = true;
    }

    private void HandleMonsterDead(MonsterController mc)
    {
        Despwan(mc);
    }
}