using Manager;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterController : CharaController
{
    public event Action<MonsterController> OnDead;    
    private Coroutine _coroutine;

    protected override void UpdateDead()
    {
        _coroutine = StartCoroutine(DeadCoroutine());
    }

    IEnumerator DeadCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _anim.Rebind();
        OnDead?.Invoke(this);
        _coroutine = null;
    }
}
