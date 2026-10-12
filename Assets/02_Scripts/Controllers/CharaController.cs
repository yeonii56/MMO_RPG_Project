using Manager;
using UnityEngine;
using static Define;

public class CharaController : MonoBehaviour
{
    [SerializeField] protected Animator _anim;
    [SerializeField] protected Transform _trans;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;
    protected float _speed = 5f;
    protected float _hp = 100f;
    protected float _damage = 100f;


    private CharaState _state = CharaState.Idle;
    protected CharaState State
    {
        get {  return _state; }
        set
        {
            if (value == _state)
                return;

            _state = value;
            UpdateAnimation();
        }
    }
    private MoveDir _lastDir = MoveDir.Left;
    private MoveDir _dir = MoveDir.None;
    protected MoveDir Dir
    {
        get
        {
            return _dir;
        }
        set
        {
            if (value == _dir)
                return;

            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    public Vector3Int GetFrontCell()
    {
        Vector3Int cell = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cell += Vector3Int.up;
                break;
            case MoveDir.Down:
                cell += Vector3Int.down;
                break;
            case MoveDir.Right:
                cell += Vector3Int.right;
                break;
            case MoveDir.Left:
                cell += Vector3Int.left;
                break;
        }
        return cell;
    }

    public void OnDamaged(float damage)
    {
        _hp -= damage;

        if (_hp <= 0)
        {
            State = CharaState.Dead;
        }
    }

    public virtual void Init()
    {
        State = Define.CharaState.Idle;
        Dir = Define.MoveDir.None;

        transform.position = Manager.SceneMgr.Instance.Map.CellToWorld(CellPos);
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CharaState.Idle:
                UpdateIdle();
                break;
            case CharaState.Move:
                UpdateMove();
                break;
            case CharaState.Attack:
                UpdateAttack();
                break;
            case CharaState.Dead:
                UpdateDead();
                break;
        }
    }

    // 애니메이션 업데이트
    protected virtual void UpdateAnimation()
    {
        switch (State)
        {
            case CharaState.Idle:
                _anim.Play("Idle");
                break;
            case CharaState.Move:
                _anim.Play("Run");

                if (Dir == MoveDir.Right)
                {
                    _trans.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if (Dir == MoveDir.Left)
                {
                    _trans.localScale = new Vector3(1f, 1f, 1f);
                }
                break;
            case CharaState.Attack:
                _anim.Play("Attack");
                break;
            case CharaState.Dead:
                _anim.Play("Die");
                break;
            default:
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    
    }

    protected virtual void UpdateMove()
    {
        if (State == CharaState.Move)
        {
            Vector3 destPos = Manager.SceneMgr.Instance.Map.CellToWorld(CellPos);
            Vector3 moveDir = destPos - transform.position;

            if (moveDir.magnitude < _speed * Time.deltaTime)
            {
                transform.position = destPos;
                MoveToNextPos();
            }
            else
            {
                transform.position += moveDir.normalized * _speed * Time.deltaTime;
                State = CharaState.Move;
            }
        }
    }

    protected virtual void MoveToNextPos()
    {
        if (_dir == MoveDir.None)
        {
            State = CharaState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;

        switch (_dir)
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
        }

        if (Manager.SceneMgr.Instance.CanGo(destPos))
        {
            if (Manager.ObjectMgr.Instance.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }

    protected virtual void UpdateAttack()
    {
    }

    protected virtual void UpdateDead()
    {

    }
}
