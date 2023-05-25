using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeAndRepositionBaseWidth : MonoBehaviour
{
    [SerializeField, Header("リサイズされるやつ")] private RectTransform _rectTransform;
    [SerializeField, Header("基準（キャンバスまたはPopupのRootなど）")] private RectTransform _canvasTransform;
    [SerializeField, Header("左右の隙間")] private float _sidePadding;
    [SerializeField, Header("下の隙間")] private float _underPadding;

    private void OnEnable()
    {
        Resize();
        if (_underPadding != 0)
        { 
            Reposition();
        }
    }

    private void Resize()
    {
        var ratio = (_canvasTransform.rect.width - (_sidePadding * 2f)) / _rectTransform.rect.width;
        _rectTransform.sizeDelta = new Vector2(ratio * _rectTransform.rect.width, ratio * _rectTransform.rect.height);
    }

    private void Reposition()
    {
        _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, (_rectTransform.rect.height / 2f) + _underPadding, _rectTransform.localPosition.z);
    }
}
