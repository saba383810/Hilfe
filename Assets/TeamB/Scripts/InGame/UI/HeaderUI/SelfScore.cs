using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelfScore : MonoBehaviour
{
    [SerializeField, Header("順位のTMP")]
    private TMP_Text _rankNumTMPText;
    [SerializeField, Header("スコアのTMP")]
    private TMP_Text _scoreTMPText;
    [SerializeField,Header("ランクのImage")]
    private Image image;
    
    [SerializeField,Header("ランクの画像素材")] 
    private Sprite[] rankSprites;
    

    public void Initialization(string userName)
    {
        SetRank(1);
        SetScore(0);
    }

    // 順位のテキストをセット
    public void SetRank(int rank)
    {
        if (rank < 1)
        {
            Debug.LogError("スコアがマイナスです");
            _rankNumTMPText.text = 1.ToString();
            return;
        }
        
        _rankNumTMPText.text = rank.ToString();
        image.sprite = rankSprites[rank - 1];
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
