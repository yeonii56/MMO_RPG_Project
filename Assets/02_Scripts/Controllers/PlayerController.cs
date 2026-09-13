using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Animator _anim;

    private MoveDir _dir = MoveDir.None;   
    private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;
    private float _speed = 5f;

    void Start()
    {
        transform.position = _grid.CellToWorld(_cellPos);
    }

    void Update()
    {
        UpdateMoveState();
        UpdateIsMoving();
        UpdatePosition();
    }

    // 이동키 받기
    private void UpdateMoveState()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _dir = MoveDir.Right;
        }
        else
        {
            _dir = MoveDir.None;
        }
    }

    // 실제 포지션 이동
    private void UpdateIsMoving()
    {
        if (_isMoving == true)
            return;

        switch (_dir)
        {
            case MoveDir.Up:
                _cellPos += Vector3Int.up;
                _isMoving = true;
                break;
            case MoveDir.Down:
                _cellPos += Vector3Int.down;
                _isMoving = true;
                break;
            case MoveDir.Right:
                _cellPos += Vector3Int.right;
                _isMoving = true;
                break;
            case MoveDir.Left:
                _cellPos += Vector3Int.left;
                _isMoving = true;
                break;
        }
    }

    // 이동 모션
    private void UpdatePosition()
    {
        if (_isMoving)
        {
            Vector3 destPos = _grid.CellToWorld(_cellPos);
            Vector3 moveDir = destPos - transform.position;

            if (moveDir.magnitude < _speed * Time.deltaTime)
            {
                transform.position = destPos;
                _isMoving = false;
            }
            else
            {
                transform.position += moveDir.normalized * _speed * Time.deltaTime;
                _anim.Play("Run");
                _isMoving = true;
            }
        }
    }
}
