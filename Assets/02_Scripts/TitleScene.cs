using Manager;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    private string _sceneName;

    private void Start()
    {
        Init();
    }

    public void ClickStartBtn()
    {
        SceneMgr.Instance.LoadScene(_sceneName);
    }

    private void Init()
    {
        DBMgr.Init();
        SetData();
    }

    // 서버에서 데이터 받아오게 수정
    private void SetData()
    {
        SceneMgr.Instance.SpwanPos = Vector3Int.zero;
        _sceneName = "MainScene";
    }
}
