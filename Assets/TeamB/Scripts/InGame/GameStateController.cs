using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Asteroids.HostSimple;
using Common;
using Cysharp.Threading.Tasks;
using Fusion;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TeamB.Scripts.Common;
using TeamB.Scripts.Common.API;
using TeamB.Scripts.InGame.Matching;
using TeamB.Scripts.InGame.UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    Initialize,
    Matching,
    InGame,
    Ending,
    Disconnect
}

public class GameStateController : NetworkBehaviour
{
    [SerializeField] private float matchingTime = 90.0f;
    [SerializeField] private NetworkPrefabRef enemyContainerNetworkedPrefab;
    [SerializeField] private NetworkPrefabRef bulletContainerNetworkedPrefab;
    [SerializeField] private MatchingUI matchingUI;
    [SerializeField] private IngameUIController inGameUIController;
    [SerializeField] private CommonButton disconnectButton;
    [SerializeField] private GameStartWindow gameStartWindow;
    [SerializeField] private GameObject gotoHomeButton;
    
    [Networked] private TickTimer Timer { get; set; }
    [Networked] private GameState GameState { get; set; }
    [Networked] private NetworkBehaviourId _winner { get; set; }
    
    public Dictionary<int,Transform> PlayerTransformDictionary = new ();
    private int _roomMaxPlayer;
    private LocalInputPoller _localInputPoller;

    /// <summary>int: photonId, string: userName</summary>
    public Dictionary<int, string> UserNameDictionary = new ();

    private CancellationTokenSource _cts;
    private bool _isGameEnded;
    private bool _isCountDownFinish = false;
    private bool _isAmount30;
    private bool _playerGenerateDone = false;
    
    private const string GrimyKey = "grimmy";
    private const string FangKey = "fangculite";
    private const string MelogardiaKey = "mellogardia";
    
    // シングルトン参照
    private float _gameSessionTime;

    private void Start()
    {
        BGMManager.Instance.Play(BGMPath.INGAME_STANDBY_BGM);
        _cts = new CancellationTokenSource();
    }

    public override void Spawned()
    {
        Debug.Log($"StateController Spawn SessionName{Runner.SessionInfo.Name}");
        // 初期画面の設定
        Application.targetFrameRate = 60;
        _gameSessionTime = LevelDesignSingleton.Instance.GetLimitTime();
        _localInputPoller = FindAnyObjectByType<LocalInputPoller>();
        _localInputPoller.InputEnabled = false;
        GameState = GameState.None;
        Initialize(_cts.Token).Forget();
    }

    private async UniTask Initialize(CancellationToken token)
    {
        GameState = GameState.Initialize;
        // Host
        if (Object.HasStateAuthority)
        {
            UserNameDictionary.Add(Runner.LocalPlayer.PlayerId, Preferences.GetPlayerName());
            matchingUI.Setup(0, Preferences.GetPlayerName());
        }
        // Client
        else
        {
            RPC_SynPlayerInfo(Runner.LocalPlayer.PlayerId, Preferences.GetPlayerName());
        }

        disconnectButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => Disconnect());

        gotoHomeButton.SetActive(Runner.GameMode == GameMode.Single);

        Matching(token).Forget();
    }

    private async UniTask Matching(CancellationToken token)
    {
        if(Object.HasStateAuthority) Timer = TickTimer.CreateFromSeconds(Runner, matchingTime);
        _roomMaxPlayer = Runner.SessionInfo.MaxPlayers;
        GameState = GameState.Matching;

        await UniTask.WaitUntil(() => Timer.ExpiredOrNotRunning(Runner) || UserNameDictionary.Count >= _roomMaxPlayer, cancellationToken: token);
        if(token.IsCancellationRequested) return;
        if (UserNameDictionary.Count >= _roomMaxPlayer)
        {
            disconnectButton.gameObject.SetActive(false);
            Runner.SessionInfo.IsOpen = false;
            Runner.SessionInfo.IsVisible = false;
            
            //投票結果などの表示
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
            await gameStartWindow.Setup();
            await UniTask.Delay(TimeSpan.FromSeconds(4), cancellationToken: token);
            inGameUIController.SetTime((int)_gameSessionTime);
            inGameUIController.gameObject.SetActive(true);
            if(token.IsCancellationRequested) return;
            await InGame(token);
            return;
        }
        Disconnect();
    }

    private async UniTask InGame(CancellationToken token)
    {
        await FindObjectOfType<PlayerSpawner>().StartPlayerSpawner(this);
        if (Object.HasStateAuthority)
        {
            await UniTask.WaitUntil(() => PlayerTransformDictionary.Count >= _roomMaxPlayer, cancellationToken: token);
            RPC_PlayerGenerateDone();
        }
        else
        {
            await UniTask.WaitUntil(() => _playerGenerateDone, cancellationToken: token);
        }
        
        matchingUI.gameObject.SetActive(false);
        await CountDownStart();
        if(Object.HasStateAuthority) Timer = TickTimer.CreateFromSeconds(Runner,_gameSessionTime);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
        Runner.Spawn(enemyContainerNetworkedPrefab);
        Runner.Spawn(bulletContainerNetworkedPrefab);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: token);
        
        _localInputPoller.InputEnabled = true;
        GameState = GameState.InGame;
    }
    
    
    public override void FixedUpdateNetwork()
    {
        if (GameState == GameState.InGame)
        {
            UpdateRunningDisplay();
            // Ends the game if the game session length has been exceeded
            if (Timer.ExpiredOrNotRunning(Runner))
            {
                GameHasEnded();
            }
        }
        else if (GameState == GameState.Matching)
        {
            UpdateMatchingUI();
        }
    }

    public async void GameHasEnded()
    {
        if(_isGameEnded) return;
        GameState = GameState.Ending;
        _isGameEnded = true;
        Debug.Log("Game終了");
        BGMManager.Instance.Play(BGMPath.INGAME_RESULT_BGM_);
        inGameUIController.gameObject.SetActive(false);
        var localInputPoller = FindAnyObjectByType<LocalInputPoller>();
        localInputPoller.InputEnabled = false;
        
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        var playerDataList = new List<PlayerDataNetworked>(10);
        foreach (var playerObject in playerObjects)
        {
            var playerDataNetworked = playerObject.GetComponent<PlayerDataNetworked>();
            playerDataList.Add(playerDataNetworked);
        }

        PlayerDataNetworked myPlayerDataNetworked = null;
        var dataList = new List<ResultCellData>();
        foreach (var data in playerDataList)
        {
            Debug.Log($"P{data.PlayerIndex}:{data.NickName} 獲得数 :{data.EnemyTypeACnt + data.EnemyTypeBCnt + data.EnemyTypeCCnt} playerId{data.PlayerId}");
            dataList.Add(new ResultCellData(data.PlayerIndex,data.NickName.ToString(),data.EnemyTypeACnt + data.EnemyTypeBCnt + data.EnemyTypeCCnt,Runner.LocalPlayer.PlayerId == data.PlayerId));
            if (Runner.LocalPlayer.PlayerId == data.PlayerId)
            {
                myPlayerDataNetworked = data;
            }
        }
        
        // データのアップロード
        await UploadScore(myPlayerDataNetworked);
        
        var dataArray = dataList.ToArray();
        for (var i = 0; i < dataArray.Length; i++)
        {
            for (var j = 0; j < dataArray.Length; j++)
            {
                if (i == j) continue;
                if (dataArray[i].Score > dataArray[j].Score)
                {
                    (dataArray[i], dataArray[j]) = (dataArray[j], dataArray[i]);
                }

                await UniTask.Yield();
            }
        }

        if (Runner.GameMode != GameMode.Single)
        {
            var localResultCell = dataList.FirstOrDefault(data => data.IsLocalPlayer);
            if (localResultCell != null)
            {
                var score = Preferences.GetHighScore();
                if (score < localResultCell.Score)
                {
                    Preferences.SetHighScore(localResultCell.Score);
                }
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        
        Preferences.SetRanking(true);
        
        PopupManager.ShowResultPopupAsync(dataArray,Disconnect);

    }

    private async UniTask UploadScore(PlayerDataNetworked myPlayerDataNetworked)
    {
        if (Runner.GameMode == GameMode.Single) return;
        var dict = new Dictionary<string, uint>();
        dict.Add(GrimyKey, (uint)myPlayerDataNetworked.EnemyTypeACnt);
        dict.Add(FangKey, (uint)myPlayerDataNetworked.EnemyTypeBCnt);
        dict.Add(MelogardiaKey, (uint)myPlayerDataNetworked.EnemyTypeCCnt);
        try
        {
            var user = await APIClient.ScoreRanking.UpdateScoreRanking(Preferences.GetPlayerId(), dict);
        }
        catch (APIErrorException e)
        {
            Debug.Log(e);
            PopupManager.ShowFooterErrorPopupAsync("通信中にエラーが発生しました");
        }
    }
    
    public void TrackNewPlayer(int playerId,Transform targetTransform)
    {
        Debug.Log("track");
        PlayerTransformDictionary.Add(playerId,targetTransform);
    }

    public int GetPlayerIndex(int photonId)
    {
        return UserNameDictionary.TakeWhile(userNamePair => userNamePair.Key != photonId).Count();
    }

    private void UpdateMatchingUI()
    {
        var remainTime = Timer.RemainingTime(Runner);
        if(remainTime != null) matchingUI.SetTimer((int)remainTime);
        var currentPlayersCnt = Runner.ActivePlayers.Count();
        var maxPlayer = _roomMaxPlayer;
        matchingUI.SetCurrentPlayer(currentPlayersCnt, maxPlayer);
    }

    private void UpdateRunningDisplay()
    {
        var remainTimer = Timer.RemainingTime(Runner);
        if (remainTimer != null)
        {
            inGameUIController.SetTime((int)remainTimer);
            if (remainTimer <= 4 && !_isCountDownFinish)
            {
                _isCountDownFinish = true;
                CountDownFinish().Forget();
            }
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_SynPlayerInfo(int photonId, string userName)
    {
        if(!Object.HasStateAuthority) return;
        Debug.Log($"Syn PlayerInfo photonId: {photonId} userName: {userName}");
        UserNameDictionary.Add(photonId, userName);
     
        RefreshMatchingUserName();
        var keys = new List<int>();
        var usernames = new List<string>();
        foreach (var userNamePair in UserNameDictionary)
        {
            keys.Add(userNamePair.Key);
            usernames.Add(userNamePair.Value);
        }
        RPC_AckPlayerInfo(keys.ToArray(),usernames.ToArray());
    }
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_AckPlayerInfo(int[] photonIds, string[] userNames)
    {
        if(Object.HasStateAuthority) return;
        UserNameDictionary = new Dictionary<int, string>();
        for (var i = 0; i < photonIds.Length; i++)
        {
            UserNameDictionary.Add(photonIds[i],userNames[i]);
        }

        var playerIndex = 0;     
        foreach (var keypair in UserNameDictionary)
        {
            if (keypair.Key == Runner.LocalPlayer.PlayerId)
            {
                inGameUIController.SetPlayerIndex(playerIndex);
            }

            playerIndex++;
        }
        
        RefreshMatchingUserName();
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_PlayerGenerateDone()
    {
        _playerGenerateDone = true;
    }

    private void RefreshMatchingUserName()
    {
        var index = 0;
        foreach (var userNamePair in UserNameDictionary)
        {
            matchingUI.Setup(index,userNamePair.Value);
            index++;
        }
    }

    public async void Disconnect()
    {
        if (GameState == GameState.InGame)
        {
            var errorPopup = await PopupManager.ShowFooterErrorPopupAsync("通信が切断されました");
            await UniTask.Delay(TimeSpan.FromSeconds(1f),cancellationToken:_cts.Token);
            if (_cts.IsCancellationRequested) SceneManager.LoadScene("OutGame");
        }
        
        var popup = await PopupManager.ShowPopupAsync(PopupKey.LOADING_POPUP);
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f),cancellationToken:_cts.Token);
        }
        catch (OperationCanceledException e)
        { }
        _cts?.Cancel();
        await Runner.Shutdown();
    }

    private void OnDestroy()
    {
        if (Runner != null)
        {
            Runner.Shutdown();
        }
        _cts.Cancel();
    }
    
    public async UniTask CountDownStart()
    {
        BGMManager.Instance.Stop();
        await CountDown();
        inGameUIController.ShowNumberEffect(NumberEffectType.Start);
        BGMManager.Instance.Play(BGMPath.INGAME_BGM_FIRSTHALF);
        SEManager.Instance.Play(SEPath.INGAME_GO);
    }
    
    public async UniTask CountDownFinish()
    {
        await CountDown();
        inGameUIController.ShowNumberEffect(NumberEffectType.Finish);
        BGMManager.Instance.Play(BGMPath.INGAME_BGM_FIRSTHALF);
        SEManager.Instance.Play(SEPath.INGAME_TIMEISUP);
    }

    public async UniTask CountDown()
    {
        inGameUIController.ShowNumberEffect(NumberEffectType.Three);
        SEManager.Instance.Play(SEPath.INGAME_COUNTDOWN_SINGLE);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        inGameUIController.ShowNumberEffect(NumberEffectType.Two);
        SEManager.Instance.Play(SEPath.INGAME_COUNTDOWN_SINGLE);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        inGameUIController.ShowNumberEffect(NumberEffectType.One);
        SEManager.Instance.Play(SEPath.INGAME_COUNTDOWN_SINGLE);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    public async UniTask Amount30()
    {
        inGameUIController.ShowNumberEffect(NumberEffectType.Three);
        SEManager.Instance.Play(SEPath.INGAME_COUNTDOWN_SINGLE);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        var index = 0;
        foreach (var keypair in UserNameDictionary)
        {
            if(keypair.Key == player.PlayerId) return;
            index++;
        }
        Debug.Log($"playerIndex:{index}");
        UserNameDictionary.Remove(player.PlayerId);
        matchingUI.DisablePlayerInfo(index);
    }
    
}
