using Manager;
using System.Collections;
using UnityEngine;
using static Define;

public class PlayerController : CharaController
{
    private Coroutine _coroutine;
    private bool _isPotal = false;

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CharaState.Idle:
                GetDirInput();
                break;
            case CharaState.Move:
                GetDirInput();
                break;
            case CharaState.Attack:
                break;
            case CharaState.Dead:
                break;
        }

        base.UpdateController();
    }

    protected override void UpdateMove()
    {
        base.UpdateMove();

        // 포탈
        _isPotal = Manager.SceneMgr.Instance.IsPotal(CellPos);
    }

    protected override void UpdateIdle()
    {
        // 이동
        if (Dir != MoveDir.None)
        {
            State = CharaState.Move;
            return;
        }

        // 스킬
        if (Input.GetKey(KeyCode.Space))
        {
            State = CharaState.Attack;
            _coroutine = StartCoroutine(AttackCoroutine());
        }
    }

    protected override void UpdateDead()
    {
        // UI 팝업 -> 버튼 클릭하면 넘어가게 수정
        CellPos = Vector3Int.zero;
        transform.position = SceneMgr.Instance.Map.CellToWorld(CellPos);
    }

    // 이동 키보드 입력
    private void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        // 데미지 판정
        GameObject obj = Manager.ObjectMgr.Instance.Find(GetFrontCell());
        if (obj != null)
        {
            if (obj.TryGetComponent<CharaController>(out var cc))
            {
                cc.OnDamaged(_damage);
            }
            else
            {
                Debug.LogError("CharaController not found error");
            }
        }

        // 시간 대기
        yield return new WaitForSeconds(0.5f);
        State = CharaState.Idle;
        _coroutine = null;
    }
}
