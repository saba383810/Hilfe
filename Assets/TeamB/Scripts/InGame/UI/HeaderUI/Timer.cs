using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// タイマー
/// </summary>
public class Timer : MonoBehaviour
{
    [SerializeField, Header("分数テキスト")] private TMP_Text _minuteText;
    [SerializeField, Header("コロン")] private TMP_Text _colonText;
    [SerializeField, Header("秒数テキスト")] private TMP_Text _secondText;
    [SerializeField, Header("制限時間（秒数）")] private float _timeLimit = 180;

    private float _time; 

    public void Initialization()
    {
        _minuteText.text = $"{(_timeLimit / 60):00}";
        _secondText.text = $"{(_timeLimit % 60):00}"; 
    }
    
    // 分数のセット
    public void SetMinuteText(float time)
    {
        if (time < 0)
        {
            _minuteText.text = "00";
            return;
        }
        var minute = (int)(time / 60f);
        _minuteText.text = $"{minute:00}";
    }
    
    // 秒数のセット
    public void SetSecondText(float time)
    {
        if (time < 0)
        {
            _secondText.text = "00";
            return;
        }
        var second = (int)(time % 60f);
        _secondText.text = $"{second:00}";
    }

    public void ChangeColor()
    {
        _secondText.color = Color.red;
        _colonText.color = Color.red;
        _minuteText.color = Color.red;
        
    }
}
