using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fresh23_N.Scripts.Common;
using TeamB.Scripts;
using TeamB.Scripts.Common;
using TeamB.Scripts.Common.API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common
{
    public class PopupManager : MonoBehaviour
    {
        private static CancellationTokenSource _cts;
        private static CancellationToken _token;

        public static void Init()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        public static async UniTask<Popup> ShowPopupAsync(string addressableKey)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(addressableKey);
            if (_token.IsCancellationRequested) return null;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{addressableKey} (Created by PopupManager)";
            var popup = obj.GetComponent<Popup>();
            popup.Setup();
            popup.Show();
            return popup;
        }


        /// <summary>
        ///     メッセージポップアップを表示
        /// </summary>
        public static async UniTask<Popup> ShowMessagePopupAsync(string caption, string body)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.MESSAGE_POPUP);
            if (_token.IsCancellationRequested) return null;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.MESSAGE_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<MessagePopup>();
            popup.Setup(caption, body);
            popup.Show();
            return popup;
        }

        /// <summary>
        ///     エラーポップアップを表示
        /// </summary>
        /// <param name="errorMessage">表示したいPopup名</param>
        public static async void ShowErrorPopupAsync(string errorMessage)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.ERROR_POPUP);
            if (_token.IsCancellationRequested) return;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.ERROR_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<ErrorPopup>();
            popup.Setup(errorMessage);
            popup.Show();
        }
        
        /// <summary>
        ///     メッセージポップアップを表示
        /// </summary>
        public static async UniTask<IngameFooterErrorPopup> ShowFooterErrorPopupAsync(string body)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.INGAME_FOOTER_ERROR_POPUP);
            if (_token.IsCancellationRequested) return null;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.INGAME_FOOTER_ERROR_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<IngameFooterErrorPopup>();
            popup.Setup(body);
            popup.Show();
            return popup;
        }

        /// <summary>
        ///     ランキングポップアップを表示
        /// </summary>
        /// <param name="errorMessage">表示したいPopup名</param>
        public static async void ShowRankingPopupAsync()
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.RANKING_POPUP);
            if (_token.IsCancellationRequested) return;
            var users = await APIClient.ScoreRanking.GetScoreRanking(Preferences.GetPlayerId());
            var myUser = await APIClient.Users.GetUser(Preferences.GetPlayerId());
            var list = new List<(string userId, string username, uint score)>();
            foreach (var user in users)
            {
                list.Add((user.user_id, user.user_name, user.score));
            }
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.RANKING_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<RankingPopup>();
            popup.Setup(list, myUser);
            popup.Show();
        }

        /// <summary>
        ///     リザルトポップアップを表示
        /// </summary>
        /// <param name="resultCellDataList"></param>
        public static async void ShowResultPopupAsync(ResultCellData[] resultCellDataArray, Action disconnect)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.RESULT_POPUP);
            if (_token.IsCancellationRequested) return;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.RESULT_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<ResultPopup>();
            popup.Setup(resultCellDataArray,disconnect);
            popup.Show();
        }
        
        /// <summary>
        ///     ADVポップアップを表示
        /// </summary>
        /// <param name="errorMessage">表示したいPopup名</param>
        public static async UniTask<Popup> ShowADVPopupAsync(List<ADVData> advDates)
        {
            var popupParent = GameObject.Find("PopupUI").transform;
            var popupPrefab = await GetPopup(PopupKey.ADV_POPUP);
            if (_token.IsCancellationRequested) return null;
            var obj = Instantiate(popupPrefab, popupParent);
            obj.name = $"{PopupKey.ADV_POPUP} (Created by PopupManager)";
            var popup = obj.GetComponent<ADVPopup>();
            popup.Setup(advDates, true);
            popup.Show();
            return popup;
        }

        /// <summary>
        ///     AddressableのPopupGroupからからGameObjectを取得
        /// </summary>
        /// <param name="popupName"></param>
        /// <returns></returns>
        private static async UniTask<GameObject> GetPopup(string popupName)
        {
            var popupPrefab = await Addressables.LoadAssetAsync<GameObject>(popupName);
            Debug.Assert(popupPrefab != null,
                $"指定したPopupが見つかりませんでした。Addressable Groupsを確認してください。\npopupName : {popupName}");
            return popupPrefab;
        }

        public static void CancelPopup()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}