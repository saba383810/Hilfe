using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Fusion;
using Fusion.Photon.Realtime;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TMPro;
using UniRx;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プライベートマッチのマッチング用ポップアップ
/// </summary>
public class PrivateMatchiongPopup : Popup
{
    [SerializeField, Header("ジョイン側に切り替わるボタン")] private CommonButton _joinButton;
    [SerializeField, Header("ホスト側に切り替わるボタン")] private CommonButton _hostButton;
    [SerializeField, Header("ジョイン側ポップアップ")] private GameObject _joinPopup;
    [SerializeField, Header("ホスト側ポップアップ")] private GameObject _hostPopup;

    // By CreatePrivateMatchPopup
    [SerializeField] private TMP_Text selectMemberText; 
    [SerializeField] private CommonButton[] selectMemberButtons; 
    [SerializeField] private Image[] selectMemberButtonImages;
    [SerializeField] private TMP_Text[] selectMemberTexts;
    [SerializeField] private TMP_Text secretText; 
    [SerializeField] private TMP_InputField secretInputField; 
    [SerializeField] private CommonButton createMatchButton; 
    private readonly AsyncReactiveProperty<int> _state = new(0); 
    private int selectMemberCount = 0; 
    private IDisposable joinButtonDisposable;
    
    // By JoinPrivateMatchPopup
    [SerializeField] private TMP_InputField secretJoinInputField;
    [SerializeField] private CommonButton joinMatchButton;
    
    // By HomeMatchingController
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private string gameSceneName = null;
    private NetworkRunner _runnerInstance = null;
    
    private const int RoomMaxPlayers = 4;
    
    private void Start()
    {
        _hostPopup.SetActive(true);
        _joinPopup.SetActive(false);
        Setup();
        SetSubscribe();
    }
    
    public override void Setup()
    { 
        base.Setup(); 
        
        // By CreatePrivateMatchPopup
        _state.Value = 1;
    }

    private void SetSubscribe()
    {
        _joinButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => ChangeToJoin());
        _hostButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => ChangeToHost());
        createMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => StartHost());
        joinMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => StartClient());
            
        _state.ToObservable().TakeUntilDestroy(gameObject).Subscribe(state =>
        {
            if (state == 1) 
            { 
                selectMemberText.color = Color.white; 
                ChangeSelectButtonsColor(10); 
                secretText.color = Color.gray; 
                secretInputField.interactable = false; 
                createMatchButton.SetInteractable(false); 
            }
            else if (state == 2) 
            { 
                selectMemberText.color = Color.gray; 
                secretText.color = Color.white; 
                secretInputField.interactable = true;
                createMatchButton.SetInteractable((secretInputField.text.Length is >= 4 and <= 12) && selectMemberCount != 0); 
            }
        });
        
        secretInputField.onValueChanged.AddListener(text => 
            createMatchButton.SetInteractable(text.Length is >= 4 and <= 12 && selectMemberCount != 0)); 
        selectMemberButtons[0].OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => 
            {
                selectMemberCount = 2; 
                ChangeSelectButtonsColor(0); 
                _state.Value = 2;
            });
        selectMemberButtons[1].OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => 
            {
                selectMemberCount = 3; 
                ChangeSelectButtonsColor(1); 
                _state.Value = 2;
            });
        selectMemberButtons[2].OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => 
            {
                selectMemberCount = 4; 
                ChangeSelectButtonsColor(2); 
                _state.Value = 2;
            });
        
        joinMatchButton.SetInteractable(false); 
        secretJoinInputField.onValueChanged.AddListener(text => 
            joinMatchButton.SetInteractable(text.Length is >= 4 and <= 12));
    }

    private void ChangeSelectButtonsColor(int number) 
    { 
        for (int i = 0; i < selectMemberButtonImages.Length; i++) 
        { 
            selectMemberButtonImages[i].gameObject.SetActive(i == number);
            selectMemberTexts[i].color = i == number ? Color.white : Color.black;
        }
    }

    private void ChangeToHost()
    {
        _hostPopup.SetActive(true);
        _joinPopup.SetActive(false);
    }

    private void ChangeToJoin()
    {
        _hostPopup.SetActive(false);
        _joinPopup.SetActive(true);
    }
    
    private void StartHost()
    {
        StartGame(GameMode.Host, secretInputField.text, gameSceneName, playerCnt:selectMemberCount);
    }
    
    private void StartClient()
    {
        StartGame(GameMode.Client,secretJoinInputField.text, gameSceneName, playerCnt:selectMemberCount);
    }
    
    private async void StartGame(GameMode mode, string roomName, string sceneName, bool isVisible = false,
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
        roomName += now.Day.ToString();
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
        
        startGameArgs.SessionName = roomName;

        var result = await _runnerInstance.StartGame(startGameArgs);
        if (!result.Ok)  popup.Hide();
        
        _runnerInstance.SetActiveScene(sceneName);

    }
}
