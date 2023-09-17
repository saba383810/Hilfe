using System;
using Common;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TeamB.Scripts.Common.API;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class HomeMatchingController : MonoBehaviour
{
    [SerializeField] private CommonButton randomMatchButton;
    [SerializeField] private CommonButton singleMatchButton;
    [SerializeField] private CommonButton webLinkButton;
    [SerializeField] private CommonButton scoreRankingButton;
    [SerializeField] private CommonButton optionButton;
    [SerializeField] private RectTransform safeArea;
    [SerializeField] private RectTransform footer;
    [SerializeField] private RectTransform character;

    [SerializeField] private TMP_Text highScoreText;

    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private string gameSceneName = null;
    

    [SerializeField, Header("プライベートマッチのボタン")]
    private CommonButton _privateMatchingButton;
    
    private NetworkRunner _runnerInstance = null;
    private const int RoomMaxPlayers = 4;
    private const string WebSiteURL = "https://www.mil2023-team-b.com/?utm_source=game_inflow";

    private void Start()
    {
        var mul = safeArea.rect.size.x / 1080f;
        if (mul <= 0.85f)
        {
            mul = 0.85f;
        }
        else if (mul > 1f)
        {
            mul = 1f;
        }
        footer.localScale = new Vector3(mul, mul, mul);
        character.localScale = new Vector3(mul, mul, mul);
        
        CheckTrial();
        CheckRanking();

        highScoreText.text = Preferences.GetHighScore().ToString();
        
        randomMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => StartRandom());
        singleMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => StartSingle());
        _privateMatchingButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ =>
        {
            PopupManager.ShowPopupAsync(PopupKey.PRIVATE_MATCHING_POPUP);
        });

        webLinkButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ =>
        {
            Application.OpenURL(WebSiteURL);
        });

        scoreRankingButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ =>
        {
            PopupManager.ShowRankingPopupAsync();
        });

        optionButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ =>
        {
            PopupManager.ShowPopupAsync(PopupKey.OPTION_POPUP);
        });
    }

    private async void CheckTrial()
    {
        if (!Preferences.GetTutorial())
        { 
            var popup = await PopupManager.ShowPopupAsync(PopupKey.TUTORIAL_POPUP);
            popup.OnDestroyAsObservable().TakeUntilDestroy(gameObject).Subscribe(async _ =>
            { 
                var popupAfter = await PopupManager.ShowPopupAsync(PopupKey.AFTER_TUTORIAL_POPUP);
                popupAfter.OnDestroyAsObservable().TakeUntilDestroy(gameObject).Subscribe(async _ =>
                { 
                    StartSingle();
                });
            });
        }
        else
        { 
            BGMManager.Instance.Play(BGMPath.HOME_BGM);
        }
    }

    private void CheckRanking()
    {
        if (Preferences.GetRanking())
        {
            PopupManager.ShowRankingPopupAsync();
        }
    }

    private void StartSingle()
    {
        StartGame(GameMode.Single, gameSceneName, playerCnt: 1 );
    }
    
    private void StartRandom()
    {
        StartGame(GameMode.AutoHostOrClient, gameSceneName,true, matchmakingMode: MatchmakingMode.RandomMatching);
    }
    
    private async void StartGame(GameMode mode, string sceneName, bool isVisible = false,
        int playerCnt = RoomMaxPlayers, MatchmakingMode matchmakingMode = MatchmakingMode.FillRoom)
    {
        var popup = await PopupManager.ShowPopupAsync(PopupKey.LOADING_POPUP);
        
        var playerName = Preferences.GetPlayerName();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("PlayerNameが登録されていません");
            popup.Hide();
            
            return;
        }
        
        Preferences.SetPlayerName(playerName);
        
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("sceneNameが登録されていません");
            popup.Hide();
            return;
        }

        var now = DateTime.Now;
        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(networkRunnerPrefab);
        }

        // Let the Fusion Runner know that we will be providing user input
        _runnerInstance.ProvideInput = true;
        var startGameArgs = new StartGameArgs
        {
            GameMode = mode,
            IsVisible = isVisible,
            PlayerCount = playerCnt,
            MatchmakingMode = matchmakingMode,
            ObjectPool = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
        };

        var result = await _runnerInstance.StartGame(startGameArgs);
        if (!result.Ok)  popup.Hide();
        
        _runnerInstance.SetActiveScene(sceneName);

    }
}
