using DG.Tweening;
using UnityEngine;

public class PopupTween : MonoBehaviour, IPopup
{
    private readonly float _duration = 0.2f;

    public virtual void Open()
    {
        PopupMgr.Instance.Add(this);
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, _duration);
    }

    public virtual void Close()
    {
        PopupMgr.Instance.Remove(this);
        transform.DOScale(0f, _duration).OnComplete(() => gameObject.SetActive(false));
    }

    public virtual void Refresh()
    {

    }
}
