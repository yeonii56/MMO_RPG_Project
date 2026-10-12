using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] private Spawner spawner;

    private void Start()
    {
        MakeMap();
    }

    public void MakeMap()
    {
        Manager.SceneMgr.Instance.UpdateMap(1);

        ObjectPool.Entry entry = new ObjectPool.Entry()
        {
            id = 10001,
            count = 5
        };

        ObjectPool.Instance.SetPool(entry);
        spawner.InitSpawn(entry.id, entry.count);
    }
}
