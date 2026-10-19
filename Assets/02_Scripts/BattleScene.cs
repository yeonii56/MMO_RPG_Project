using Manager;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private Spawner spawner;

    private void Start()
    {
        MakeMap();
    }

    private async void MakeMap()
    {
        await SceneMgr.Instance.ChangeMap();
    }
}
