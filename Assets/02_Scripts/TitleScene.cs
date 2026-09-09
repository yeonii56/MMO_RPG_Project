using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public void ClickStartBtn()
    {
        LoadMainScene();
    }

    private void LoadMainScene()
    {
        SceneMgr.Instance.LoadMainScene();
    }
}
