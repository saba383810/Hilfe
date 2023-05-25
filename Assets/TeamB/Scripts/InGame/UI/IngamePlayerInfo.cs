using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インゲームで表示されるユーザ名とスコア
/// </summary>
public class IngamePlayerInfo : MonoBehaviour
{
    [SerializeField, Header("ユーザ名のTMP")]
    private TMP_Text _userNameTMPText;
    [SerializeField, Header("スコアのTMP")]
    private TMP_Text _scoreTMPText;

    [SerializeField] private Image _cursor;

    [SerializeField, Header("カーソルの画像")] private List<Sprite> _cursorList;
    [SerializeField, Header("ユーザネームのカラーコード")] private List<Color> _userNameColor;

    public void Initialization(string userName, Player.PlayerType playerType)
    {
        _userNameTMPText.text = userName;
        SetScore(0);
        _cursor.sprite = _cursorList[(int)playerType];
        _userNameTMPText.color = _userNameColor[(int)playerType];
    }

    // スコアのテキストをセット
    public void SetScore(int score)
    {
        if (score < 0)
        {
            Debug.LogError("スコアがマイナスです");
            _scoreTMPText.text = 0.ToString();
            return;
        }

        _scoreTMPText.text = score.ToString();
    }
}
