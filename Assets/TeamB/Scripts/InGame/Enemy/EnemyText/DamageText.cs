using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageText : MonoBehaviour
{
    [SerializeField, Header("ハートのスケール")]
    private float _scale = 5f;
    [SerializeField, Header("拡大に掛かる時間")]
    private float _expansionTime = 0.25f;
    [SerializeField, Header("縮小，フェードアウトに掛かる時間")]
    private float _shrinkTime = 0.25f;


    public void DamageTextAnimation()
    {
        var _sprite = gameObject.transform;
        DOTween.Sequence()
            .OnStart(() =>
            {
                // 初期位置のセット
                _sprite.localPosition = Vector3.zero;
                _sprite.localScale = Vector3.zero;
            })
            .Append(_sprite.transform.DOScale(new Vector3(_scale, _scale, 1f), _expansionTime))
            .Append(_sprite.transform.DOScale(new Vector3(0f, 0f, 0f), _shrinkTime));
        //.OnComplete(DamageTextDestroy);
    }
    
    // ダメージテキスト削除
    private void DamageTextDestroy()
    {
        Destroy(gameObject);
    }
}
