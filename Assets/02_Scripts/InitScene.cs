using UnityEngine;

public class InitScene : MonoBehaviour
{
    void Awake()
    {
        ShowDownLoadSize();
        Init();
    }

    // 팝업 띄워서 다운로드 받을건지 확인
    private void ShowDownLoadSize()
    {
         Manager.AddressableMgr.Instance.GetDownLoadSize(ShowPopup);
    }

    // 팝업 띄우기 -> 팝업에서 yes 누르면 넘어가는걸로
    private void ShowPopup(long size)
    {
        LoadTitleScene();
    }

    // 타이틀씬 로드
    private void LoadTitleScene()
    {
        Manager.SceneMgr.Instance.LoadTitleScene();
    }

    private void Init()
    {
        Manager.AddressableMgr.Instance.AddLoadAssets("Map_001", typeof(GameObject));
        Manager.AddressableMgr.Instance.AddLoadAssets("001", typeof(TextAsset));
    }
}
