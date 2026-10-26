using Manager;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    private PlayerController _player;

    private void Awake()
    {
        Init();
    }

    private async void Init()
    {
        Manager.SceneMgr.Instance.SetMapData(101);
        await Manager.SceneMgr.Instance.ChangeMap();

        if (_player == null)
            MakeChara("Player");
    }

    private void MakeChara(string name)
    {
        GameObject asset = Manager.AddressableMgr.Instance.GetAsset<GameObject>(name);
        if (asset == null)
            return;

        GameObject obj = Instantiate(asset);

        if (obj.TryGetComponent<PlayerController>(out var _player))
        {
            _player.CellPos = SceneMgr.Instance.SpwanPos;
        }
    }
}
