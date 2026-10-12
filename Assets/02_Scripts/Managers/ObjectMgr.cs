using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class ObjectMgr : Singleton<ObjectMgr>
    {
        private List<GameObject> _objects = new List<GameObject>();

        public void Add(GameObject obj)
        {
            _objects.Add(obj);
        }

        public void Remove(GameObject obj)
        {
            _objects.Remove(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public GameObject Find(Vector3Int cellPos)
        {
            foreach (var obj in _objects)
            {
                if (obj.activeSelf == false)
                    continue;

                if (obj.TryGetComponent<CharaController>(out var cc) == false)
                {
                    Debug.LogError($"cc null error: {obj.name}");
                    continue;
                }
                else
                {
                    if (cc.CellPos == cellPos)
                        return obj;
                }
            }

            return null;
        }
    }
}
