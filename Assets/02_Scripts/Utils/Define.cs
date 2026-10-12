using UnityEngine;

public class Define
{
    public enum MoveDir
    {
        None,
        Up,
        Down,
        Right,
        Left
    }

    public enum CharaState
    {
        Idle,
        Move,
        Attack,
        Dead
    }
}
