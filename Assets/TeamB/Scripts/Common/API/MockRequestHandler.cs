using System;
using System.Collections.Generic;
using System.Web;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.Scripts.Common.API
{
    public class MockRequestHandler : IAPIRequestHandler
    {
        public async UniTask<string> Get(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            Debug.Log($"MockRequestHandler: GET {path}");
            var mockKey = ConstructMockKey("GET", path, queries);
            var filepath = $"mock/{Uri.EscapeDataString(mockKey)}";

            var textAsset = await Resources.LoadAsync<TextAsset>(filepath) as TextAsset;
            if (textAsset == null)
                throw new NotImplementedException($"Missing mock for ${filepath}");

            return textAsset.ToString();
        }

        public async UniTask<string> Post(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            Debug.Log($"MockRequestHandler: POST {path}");
            var mockKey = ConstructMockKey("POST", path, queries);
            var filepath = $"mock/{Uri.EscapeDataString(mockKey)}";

            var textAsset = await Resources.LoadAsync<TextAsset>(filepath) as TextAsset;
            if (textAsset == null)
                throw new NotImplementedException($"Missing mock for ${filepath}");

            return textAsset.ToString();
        }

        public async UniTask<string> Put(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            Debug.Log($"MockRequestHandler: PUT {path}");
            var mockKey = ConstructMockKey("PUT", path, queries);
            var filepath = $"mock/{Uri.EscapeDataString(mockKey)}";

            var textAsset = await Resources.LoadAsync<TextAsset>(filepath) as TextAsset;
            if (textAsset == null)
                throw new NotImplementedException($"Missing mock for ${filepath}");

            return textAsset.ToString();
        }

        public async UniTask<string> Delete(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            Debug.Log($"MockRequestHandler: DELETE {path}");
            var mockKey = ConstructMockKey("DELETE", path, queries);
            var filepath = $"mock/{Uri.EscapeDataString(mockKey)}";

            var textAsset = await Resources.LoadAsync<TextAsset>(filepath) as TextAsset;
            if (textAsset == null)
                throw new NotImplementedException($"Missing mock for ${filepath}");

            return textAsset.ToString();
        }

        private string ConstructMockKey(string method, string path, Dictionary<string, string> queries = null)
        {
            // Query parameters
            var query = "";

            if (queries != null && queries.Count > 0)
            {
                var queryCollection = HttpUtility.ParseQueryString("");
                foreach (var param in queries)
                    queryCollection.Add(param.Key, param.Value);
                query = queryCollection.ToString();
            }

            var mockKey = $"{method}_{path}";

            if (query.Length > 0)
                mockKey += $"?{query}";

            return mockKey;
        }
    }
}