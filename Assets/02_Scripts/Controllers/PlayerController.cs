using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator _anim;

    private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;
    private float _speed = 5f;
    private MoveDir _dir = MoveDir.None;
    private MoveDir Dir
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

            if (value == MoveDir.None)
            {
                _anim.Play("Idle");

            }
            else
            {
                _anim.Play("Run");

                if (value == MoveDir.Right)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if (value == MoveDir.Left)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }            
        }
    }

    void Start()
    {        
        transform.position = Manager.SceneMgr.Instance.Map.CellToWorld(_cellPos);
    }

    void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    // 키보드 입력
    private void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    // 이동 가능한 상태일 때, 실제 좌표값 변경
    private void UpdatePosition()
    {
        if (_isMoving == true)
            return;

        Vector3Int destPos = _cellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
        }

        if (Manager.SceneMgr.Instance.CanGo(destPos))
        {
            _cellPos = destPos;
            _isMoving = true;
        }
    }

    // 이동 모션
    private void UpdateIsMoving()
    {
        if (_isMoving)
        {
            Vector3 destPos = Manager.SceneMgr.Instance.Map.CellToWorld(_cellPos);
            Vector3 moveDir = destPos - transform.position;

            if (moveDir.magnitude < _speed * Time.deltaTime)
            {
                transform.position = destPos;
                _isMoving = false;
            }
            else
            {
                transform.position += moveDir.normalized * _speed * Time.deltaTime;
                _isMoving = true;
            }
        }
    }
}
