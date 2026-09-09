using UnityEngine;

public class InitScene : MonoBehaviour
{
    void Awake()
    {
        ShowDownLoadSize();
    }

    // 팝업 띄워서 다운로드 받을건지 확인
    private void ShowDownLoadSize()
    {
        AddressableMgr.Instance.GetDownLoadSize(ShowPopup);
    }

    // 팝업 띄우기 -> 팝업에서 yes 누르면 넘어가는걸로
    private void ShowPopup(long size)
    {
        LoadTitleScene();
    }

    // 타이틀씬 로드
    private void LoadTitleScene()
    {
        SceneMgr.Instance.LoadTitleScene();
    }
}
