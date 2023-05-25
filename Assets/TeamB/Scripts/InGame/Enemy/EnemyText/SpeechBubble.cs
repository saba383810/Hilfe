using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    // 文字のスケール
    private float _scale = 1f;
    // 拡大に掛かる時間
    private float _expansionTime = 0.5f;
    // 縮小，フェードアウトに掛かる時間
    private float _shrinkTime = 0.5f;
    // テキストが変化せず表示されている時間
    private float _delayTime;
    // 表示されない時間
    private float _hideTime = 1f;

    public void Initialization(TMP_Text tmpText, string bubbleText, float totalTime)
    {
        // テキストが変化せずに表示されている時間
        _delayTime = SetDelayTime(totalTime);
        // テキストの表示（α値を最大にしてから拡大）及び非表示（フェードアウトしつつ縮小）
        SpeechBubbleTextAnimation(tmpText, bubbleText);
    }

    private float SetDelayTime(float totalTime)
    {
        var delayTime = totalTime - (_expansionTime + _shrinkTime + _hideTime);
        return delayTime;
    }

    private void SpeechBubbleTextAnimation(TMP_Text tmpText, string bubbleText)
    {
        DOTween.Sequence()
            .OnStart(() =>
            {
                // テキストのセット
                tmpText.text = bubbleText;
            })
            .Append(tmpText.transform.DOScale(new Vector3(_scale, _scale, 1f), _expansionTime))
            .AppendInterval(_delayTime)
            .Append(tmpText.transform.DOScale(new Vector3(0f, 0f, 0f), _shrinkTime));
    }
}
