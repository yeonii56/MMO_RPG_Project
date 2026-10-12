using System.Threading;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    private void Awake()
    {
        Manager.SceneMgr.Instance.UpdateMap(1);
        MakeChara("Player");
    }

    private void MakeChara(string name)
    {
        GameObject asset = Manager.AddressableMgr.Instance.GetAsset<GameObject>(name);
        if (asset == null)
            return;

        GameObject chara = Instantiate(asset);
        Manager.ObjectMgr.Instance.Add(chara);

        if (chara.TryGetComponent<CharaController>(out var cc))
        {
            cc.CellPos = Vector3Int.zero;
        }
    }

    private void LoadTitleScene()
    {
        Manager.AddressableMgr.Instance.AddLoadAssets("10001", typeof(GameObject));
        Manager.SceneMgr.Instance.LoadScene("BattleScene");
    }
}
