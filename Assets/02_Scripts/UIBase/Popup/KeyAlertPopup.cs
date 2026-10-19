using DG.Tweening;
using TMPro;
using UnityEngine;

public class KeyAlertPopup : PopupTween
{
    [SerializeField] private TextMeshProUGUI _keyLabel;

    public void Open(string key)
    {
        if (_keyLabel == null)
        {
            Debug.LogError("keyLabell null error");
            return;
        }

        _keyLabel.SetText(key);
        base.Open();
    }
}
