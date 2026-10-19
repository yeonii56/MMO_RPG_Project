using System.Collections.Generic;

public class PopupMgr : Singleton<PopupMgr>
{
    private List<IPopup> _popupList = new();

    public void Add(IPopup popup)
    {
        _popupList.Add(popup);
    }

    public void Refresh()
    {
        foreach (IPopup popup in _popupList)
        { 
            popup.Refresh();
        }
    }

    public void Remove(IPopup popup)
    {
        _popupList.Remove(popup);
    }

    public void AllClose()
    {
        foreach(IPopup popup in _popupList)
        {
            popup.Close();
        }

        _popupList.Clear();
    }
}


