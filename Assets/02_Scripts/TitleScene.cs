using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    private void Start()
    {
        Init();
    }

    public void ClickStartBtn()
    {
        LoadMainScene();
    }

    private void Init()
    {
        DBMgr.Init();
        SetMapData();
    }

    private void SetMapData()
    {

    }

    private void LoadMainScene()
    {
        Manager.SceneMgr.Instance.LoadScene("MainScene");
    }
}
