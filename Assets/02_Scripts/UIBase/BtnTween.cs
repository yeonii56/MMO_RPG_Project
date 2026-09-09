using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnTween : Button
{
    private bool _canClick = true;
    private BtnType _type = BtnType.Click;

    public enum BtnType
    {
        None,
        Click,
        Roof
    }

    public override void OnPointerClick(PointerEventData data)
    {
        if (!_canClick || _type == BtnType.None)
            return;

        PlayTween(data);
    }

    private void PlayTween(PointerEventData data)
    {
        Sequence sequence = DOTween.Sequence();

        switch (_type)
        {
            case BtnType.None:
                break;
            case BtnType.Click:
                sequence.Append(this.transform.DOPunchScale((Vector2.one * 0.2f), 0.2f).OnComplete(() => base.OnPointerClick(data)));
                break;
        }
    }
}
