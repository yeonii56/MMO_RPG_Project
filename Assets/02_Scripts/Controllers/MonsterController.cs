using Manager;
using System;
using System.Collections;
using UnityEngine;
using static Define;

public class MonsterController : CharaController
{
    public int MonsterId { get; private set; }
    public event Action<MonsterController> OnDead;

    private Vector3Int _destCellPos;
    private Coroutine _coDead;
    private Coroutine _coPatrol;

    protected override CharaState State
    {
        get { return _state; }
        set
        {
            base.State = value;

            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }
        }
    }

    public override void Init()
    {
        base.Init();

        State = CharaState.Idle;
        Dir = MoveDir.None;
    }

    public void Init(int id)
    {
        MonsterId = id;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int movePos = _destCellPos - CellPos;

        if (movePos.x > 0)
            Dir = MoveDir.Right;
        else if (movePos.x < 0)
            Dir = MoveDir.Left;
        else if (movePos.y > 0)
            Dir = MoveDir.Up;
        else if (movePos.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
            case MoveDir.None:
                State = CharaState.Idle;
                break;
        }

        if (Manager.SceneMgr.Instance.CurMap.CanGo(destPos) && SceneMgr.Instance.Monsters.Find(destPos) == null)
        {
            CellPos = destPos;
        }
        else
        {
            State = CharaState.Idle;
        }
    }

    IEnumerator CoPatrol()
    {
        float time = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(time);

        for (int i = 0; i < 2; i++)
        {
            Vector3Int rand = new Vector3Int()
            {
                x = UnityEngine.Random.Range(-5, 6),
                y = UnityEngine.Random.Range(-5, 6),
                z = 0
            };

            if (SceneMgr.Instance.CurMap.CanGo(rand) && SceneMgr.Instance.Monsters.Find(rand) == null)
            {
                _destCellPos = CellPos + rand;
                State = CharaState.Move;
                yield break;
            }
        }

        State = CharaState.Idle;
    }

    protected override void UpdateDead()
    {
        _coDead = StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _anim.Rebind();
        OnDead?.Invoke(this);
        _coDead = null;
    }
}
