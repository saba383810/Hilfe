using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.Scripts.Common.API
{
    public class APIClient
    {
        private static IAPIRequestHandler _requestHandler;

        private static string _userId;

        // Lazy init
        private static IAPIRequestHandler RequestHandler
        {
            get
            {
                if (_requestHandler == null)
                {
                    if (Config.UseMock)
                        _requestHandler = new MockRequestHandler();
                    _requestHandler = new WebRequestHandler();
                }

                return _requestHandler;
            }
        }


        // ログイン済みかどうかを判断する関数
        public static bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(Preferences.GetPlayerId()) || Config.UseMock;
        }

        // ログイン認証を行う。パスワード不正確などの場合は ErrorType.AuthorizationError の APIErrorException が投げられる。
        public static async UniTask<Models.User> DoLogin(string userId)
        {
            var authResponse = await Auth.Authenticate(userId);
            Preferences.SetPlayerId(authResponse.user_id);

            return authResponse;
        }

        // ログアウトを行う
        public static void DoLogout()
        {
            Preferences.SetPlayerId(null);
        }


        public static async UniTask<Models.GeoIP> GetGeoIP()
        {
            var headers = new Dictionary<string, string>
            {
                { "User-Agent", GetCustomUserAgent() }
            };

            var responseJson = await RequestHandler.Get("https://api.ip.sb", "/geoip", null, headers);

            try
            {
                return JsonUtility.FromJson<Models.GeoIP>(responseJson);
            }
            catch (Exception ex)
            {
                throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
            }
        }

        private static string GetCustomUserAgent()
        {
            var appName = Application.productName;
            var appVersion = Application.version;
            var os = SystemInfo.operatingSystem;
            var deviceModel = SystemInfo.deviceModel;
            var unityVersion = Application.unityVersion;

            var customUserAgent = $"{appName}/{appVersion} ({os}; {deviceModel}) Unity/{unityVersion}";
            return customUserAgent;
        }

        /* /api/auth/
         * 認証関連
         * 非公開。基本的には公開関数 IsLoggedIn(), DoLogin(), DoLogout() を使う。
         */
        private static class Auth
        {
            // ログイン認証API。ユーザのID・Name・最高スコアが返される。
            public static async UniTask<Models.User> Authenticate(string userId)
            {
                var path = "/api/auth/login";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() },
                    { "Content-Type", "application/json" }
                };

                var body = new Models.LoginRequestBody
                {
                    id = userId
                };
                var bodyJson = JsonUtility.ToJson(body);

                var responseJson = await RequestHandler.Post(Config.ServerHost, path, bodyJson, null, headers);

                try
                {
                    return JsonUtility.FromJson<Models.User>(responseJson);
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }
        }

        /* /api/users/
         * ユーザー関連
         */
        public static class Users
        {
            // ユーザー登録（会員登録）
            public static async UniTask<Models.User> SignUp(string name)
            {
                const string path = "/api/users";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() },
                    { "Content-Type", "application/json" }
                };

                var body = new Models.SignUpRequestBody
                {
                    user_name = name
                };
                var bodyJson = JsonUtility.ToJson(body);

                var responseJson = await RequestHandler.Post(Config.ServerHost, path, bodyJson, null, headers);

                try
                {
                    var user = JsonUtility.FromJson<Models.User>(responseJson);
                    return user;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }

            // ユーザー情報更新（ログイン状態が必要）
            public static async UniTask<Models.User> UpdateUserInfo(
                string userId, string name)
            {
                if (!IsLoggedIn()) throw new InvalidOperationException("Must be in logged in state");

                var path = $"/api/users/{userId}";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() },
                    { "Content-Type", "application/json" }
                };

                var body = new Models.SignUpRequestBody
                {
                    user_name = name
                };
                var bodyJson = JsonUtility.ToJson(body);

                string responseJson = null;

                try
                {
                    responseJson = await RequestHandler.Put(Config.ServerHost, path, bodyJson, null, headers);
                }
                catch (APIErrorException ex)
                {
                    if (ex.Type == ErrorType.AuthorizationError)
                        responseJson = await RequestHandler.Put(Config.ServerHost, path, bodyJson, null, headers);
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                try
                {
                    var user = JsonUtility.FromJson<Models.User>(responseJson);
                    return user;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }

            // ユーザー情報の取得。ログイン状態が必要
            public static async UniTask<Models.User> GetUser(string userId)
            {
                if (!IsLoggedIn()) throw new InvalidOperationException("Must be in logged in state");

                var path = $"/api/users/{userId}";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() }
                };

                string responseJson = null;

                try
                {
                    responseJson = await RequestHandler.Get(Config.ServerHost, path, null, headers);
                }
                catch (APIErrorException ex)
                {
                    if (ex.Type == ErrorType.AuthorizationError)
                        responseJson = await RequestHandler.Get(Config.ServerHost, path, null, headers);
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                try
                {
                    var profile = JsonUtility.FromJson<Models.User>(responseJson);
                    return profile;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }
        }

        /* /api/votes
         * 人気投票API
         * 主に人気投票の結果を取得してゲームに反映する
         * ログイン状態必要
         */
        public static class VoteRanking
        {
            // 人気投票の結果を取得する
            public static async UniTask<Models.VoteRanking[]> GetVoteRankings()
            {
                if (!IsLoggedIn()) throw new InvalidOperationException("Must be in logged in state");

                var path = "/api/votes";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() }
                };

                string responseJson = null;

                try
                {
                    responseJson = await RequestHandler.Get(Config.ServerHost, path, null, headers);
                }
                catch (APIErrorException ex)
                {
                    if (ex.Type == ErrorType.AuthorizationError)
                        responseJson = await RequestHandler.Get(Config.ServerHost, path, null, headers);
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                try
                {
                    var voteRankings = responseJson.FromJsonArray<Models.VoteRanking>();
                    return voteRankings;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }
        }

        /* /api/rankings
         * スコアランキング関連
         * すべてログイン状態が必要
         */
        public static class ScoreRanking
        {
            // ランキングを取得する
            public static async UniTask<Models.ScoreRanking[]> GetScoreRanking(string userId)
            {
                if (!IsLoggedIn()) throw new InvalidOperationException("Must be in logged in state");

                const string path = "/api/rankings";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() }
                };
                var queries = new Dictionary<string, string>
                {
                    {"user_id", userId}
                };

                string responseJson = null;

                try
                {
                    responseJson = await RequestHandler.Get(Config.ServerHost, path, queries, headers);
                }
                catch (APIErrorException ex)
                {
                    if (ex.Type == ErrorType.AuthorizationError)
                        responseJson = await RequestHandler.Get(Config.ServerHost, path, queries, headers);
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                try
                {
                    var communities = responseJson.FromJsonArray<Models.ScoreRanking>();
                    return communities;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }

            /// <summary>
            ///     ランキングを更新する
            /// </summary>
            /// <param name="scoreMap">キャラクターIDとスコアのマップ</param>
            /// <returns></returns>
            public static async UniTask<Models.User> UpdateScoreRanking(string userId, Dictionary<string, uint> scoreMap)
            {
                if (!IsLoggedIn()) throw new InvalidOperationException("Must be in logged in state");

                var path = $"/api/rankings/{userId}";
                var headers = new Dictionary<string, string>
                {
                    { "User-Agent", GetCustomUserAgent() },
                    { "Content-Type", "application/json" }
                };

                var body = new Models.UpdateScoreRankingBody[scoreMap.Count];
                var i = 0;
                foreach (var score in scoreMap)
                {
                    body[i++] = new Models.UpdateScoreRankingBody
                    {
                        character_id = score.Key,
                        score = score.Value
                    };
                }
                var bodyJson = JsonHelper.ToJson(body);
                Debug.Log(bodyJson);

                string responseJson = null;

                try
                {
                    responseJson = await RequestHandler.Put(Config.ServerHost, path, bodyJson, null, headers);
                }
                catch (APIErrorException ex)
                {
                    if (ex.Type == ErrorType.AuthorizationError)
                        responseJson = await RequestHandler.Put(Config.ServerHost, path, bodyJson, null, headers);
                    else
                        ExceptionDispatchInfo.Capture(ex).Throw();
                }

                try
                {
                    var community = JsonUtility.FromJson<Models.User>(responseJson);
                    return community;
                }
                catch (Exception ex)
                {
                    throw new APIErrorException(ErrorType.JsonError, ex.Message, ex);
                }
            }
        }
    }
}