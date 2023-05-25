using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ダメージ表示と吹き出し表示
/// </summary>
public class EnemyTextController : MonoBehaviour
{

    [SerializeField, Header("吹き出しテキスト")]
    private TMP_Text _speechBubbleTMPText;
    [SerializeField, Header("吹き出しの表示開始時刻")]
    private float _startTime = 2f;
    [SerializeField, Header("吹き出しの表示間隔")]
    private float _repeatTime = 5f;
    [SerializeField, Header("吹き出し表示確率")]
    private float _displayProbability = 0.75f;

    private SpeechBubble _speechBubble = new SpeechBubble();
    
    private void Start()
    {
        //InvokeRepeating("CallSpeechBubble",_startTime , _repeatTime);
    }

    private void CallSpeechBubble()
    {
        var callFlag = Random.value <= _displayProbability;
        if (callFlag)
        {
            var bubbleText = SetBubbleText();
            _speechBubble.Initialization(_speechBubbleTMPText, bubbleText, _repeatTime);
        }
    }

    private string SetBubbleText()
    {
        return "text";
    }
}
