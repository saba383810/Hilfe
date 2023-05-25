using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using TeamB.Scripts.InGame.UI;
using UnityEngine;

/// <summary>
/// インゲームで表示されるUIのコントローラ
/// </summary>
public class IngameUIController : MonoBehaviour
{
    [SerializeField, Header("Timer")] private Timer _timer;
    [SerializeField, Header("自分のスコア")] private SelfScore _selfScore;
    [Header("ピクミンの数")] 
    [SerializeField] public SkillButtonView typeASkillButtonView;
    [SerializeField] public SkillButtonView typeBSkillButtonView;
    [SerializeField] public SkillButtonView typeCSkillButtonView;

    [SerializeField] private NumberEffectView numberEffectView;
    [SerializeField] private ScoreConsumption scoreConsumption;
    
    [SerializeField, Header("敵プレイヤのスコア表示リストのゲームオブジェクト")]
    private List<GameObject> _playerInfoGameObjects;

    private List<IngamePlayerInfo> _ingamePlayerInfos = new ();
    private List<IngamePlayerInfoMove> _ingamePlayerInfoMoves = new ();
    private int _playerIndex;
    private bool _isStartRank;
    private bool _isLastSpurt = false;
    private bool _isBuzzer;
    private CancellationTokenSource _cts;

    private int[] _playerScoreArray = {0,0,0,0};

    // シングルトン参照
    private int usableEnemyCnt;
    
    private void Awake()
    {
        usableEnemyCnt = LevelDesignSingleton.Instance.GetPikminConsumption();
        foreach (var _playerInfoGameObject in _playerInfoGameObjects)
        {
            _ingamePlayerInfos.Add(_playerInfoGameObject.GetComponent<IngamePlayerInfo>());
            _ingamePlayerInfoMoves.Add(_playerInfoGameObject.GetComponent<IngamePlayerInfoMove>());
        }

        _cts = new CancellationTokenSource();
    }


    public void SetPlayerIndex(int playerIndex)
    {
        _playerIndex = playerIndex;
    }

    // タイマーのセット
    public void SetTime(int time)
    {
        _timer.SetMinuteText(time);
        _timer.SetSecondText(time);

        if (time <= 34 && !_isBuzzer)
        {
            _isBuzzer = true;
            BGMManager.Instance.Stop();
            SEManager.Instance.Play(SEPath.INGAME_ADV_ALERT);
            Amount30();
        }
        
        if (time >= 31 || _isLastSpurt) return;
        _isLastSpurt = true; 
        _timer.ChangeColor();
        BGMManager.Instance.Play(BGMPath.INGAME_BGM_LASTSPURT);
    }

    public void Amount30()
    {
        numberEffectView.Show(NumberEffectType.Amount30);
    }

    // 自分のスコアのセット
    public void SetSelfScore(int score)
    {
        _selfScore.SetScore(score);
        _playerScoreArray[_playerIndex] = score;
        if (!_isStartRank)
        {
            SetSelfRank(_cts.Token).Forget();
            _isStartRank = true;
        }
    }
    
    //　自分のランクのセット
    private async UniTask SetSelfRank(CancellationToken token)
    {
        Debug.Log("StartRank");
        while (true)
        {
            //ランクの計算
            var playerScore = _playerScoreArray[_playerIndex];
            var rank = 1;
            foreach (var otherScore in _playerScoreArray)
            {
                if ( otherScore > playerScore)
                {
                    rank++;
                }
            }
            _selfScore.SetRank(rank);
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
            if (token.IsCancellationRequested) return;
        }
    }

    // 敵プレイヤのインフォの初期化
    public void InitializePlayerInfo(string userName, Transform targetTransform, int index)
    {
        var playerType = (Player.PlayerType)index;
        if (_playerIndex < index)
        {
            index--;
        }
        _ingamePlayerInfos[index].Initialization(userName, playerType);
        _ingamePlayerInfoMoves[index].Initialization(targetTransform);
        SetActivePlayerInfo(index);
    }

    // 敵プレイヤのスコアのセット
    public void SetPlayerScore(int score, int index)
    {
        _playerScoreArray[index] = score;
        if (_playerIndex < index)
        {
            index--;
        }
        _ingamePlayerInfos[index].SetScore(score);
    }
    
    // PlayerInfoの有効化
    public void SetActivePlayerInfo(int index)
    {
        _playerInfoGameObjects[index].SetActive(true);
    }
    
    // PlayerInfoの非有効化
    public void SetDisactivePlayerInfo(int index)
    {
        if (_playerIndex < index)
        {
            index--;
        }
        _playerInfoGameObjects[index].SetActive(false);
    }

    public void SetPikminCnt(int typeA, int typeB, int typeC)
    {
        typeASkillButtonView.Setup(typeA, usableEnemyCnt);
        typeBSkillButtonView.Setup(typeB, usableEnemyCnt);
        typeCSkillButtonView.Setup(typeC, usableEnemyCnt);
       
    }

    public void ShowNumberEffect(NumberEffectType effectType)
    {
        numberEffectView.Show(effectType);
    }

    public void ShowUseEngelTextAnimation()
    {
        scoreConsumption.ScoreConsumptionAnimation();
    }

    private void OnDestroy()
    {
        _cts.Cancel();
    }
}