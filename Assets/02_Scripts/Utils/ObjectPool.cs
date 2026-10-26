using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<int, Queue<MonsterController>> _pool = new Dictionary<int, Queue<MonsterController>>();

    public class Entry // 추후에 DB랑 연결
    {
        public int id;
        public int count;
    }

    public void SetPool(Entry entry)
    {
        if (_pool.TryGetValue(entry.id, out Queue<MonsterController> queue))
        {
            if (_pool.Count < entry.count)
            {
                for (int i = 0; i < entry.count - queue.Count; i++)
                {
                    queue.Enqueue(CreateNew(entry.id));
                }
            }
            return;
        }

        queue = new();

        for (int i = 0; i < entry.count; i++)
        {
            queue.Enqueue(CreateNew(entry.id));
        }

        _pool.Add(entry.id, queue);
    }

    // 풀에서 가져오기
    public MonsterController Get(int id)
    {
        MonsterController obj;

        if (_pool.TryGetValue(id, out Queue<MonsterController> queue) == false)
        {
            Debug.LogError("not found queue: " + id);
            return null;

        }

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = CreateNew(id);
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    // 풀에 넣어주기
    public void Add(int id, MonsterController mc)
    {
        if (_pool.TryGetValue(id, out Queue<MonsterController> queue) == false)
        {
            Debug.LogError("not found pool: + " + id);
            _pool.Add(id, new Queue<MonsterController>());
        }

        mc.gameObject.SetActive(false);
        queue.Enqueue(mc);
    }

    private MonsterController CreateNew(int id)
    {
        if (DBMgr.Map_TB.Monster.TryGetValue(id, out var data) == false)
        {
            Debug.LogError("Not Found MonsterData : " + id);
            return null;
        }

        GameObject prefab = Manager.AddressableMgr.Instance.GetMapAsset<GameObject>(data.Prefab);
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj.GetComponent<MonsterController>();
    }
}
