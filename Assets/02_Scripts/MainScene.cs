using UnityEngine;

public class MainScene : MonoBehaviour
{
    void Awake()
    {
        Manager.SceneMgr.Instance.UpdateMap(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
