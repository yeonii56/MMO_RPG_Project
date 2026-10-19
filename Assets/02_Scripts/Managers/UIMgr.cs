using UnityEngine;

public class UIMgr : Singleton<UIMgr>
{
    [Header("Popup")]
    [SerializeField] private KeyAlertPopup _keyAlertPopup;

    public void OpenKeyAlertPopup(string key)
    {
        if ( _keyAlertPopup != null )
        {
            _keyAlertPopup.Open(key);
        }
    }

    public void CloseKeyAlertPopup()
    {
        if (_keyAlertPopup != null)
        {
            _keyAlertPopup.Close();
        }
    }
}
