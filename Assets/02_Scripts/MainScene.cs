using Manager;
using System.Threading;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    private async void Init()
    {
        Manager.SceneMgr.Instance.SetMapData(101);
        await Manager.SceneMgr.Instance.ChangeMap();
        MakeChara("Player");
    }

    private void MakeChara(string name)
    {
        GameObject asset = Manager.AddressableMgr.Instance.GetAsset<GameObject>(name);
        if (asset == null)
            return;

        GameObject chara = Instantiate(asset);
        Manager.SceneMgr.Instance.MonsterState.Add(chara);

        if (chara.TryGetComponent<CharaController>(out var cc))
        {
            cc.CellPos = SceneMgr.Instance.SpwanPos;
        }
    }
}
