using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamB.Scripts.Common.API
{
    public class WebRequestHandler : IAPIRequestHandler
    {
        public async UniTask<string> Get(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            var finalUri = ConstructFullUri(host, path, queries);
            using var request = UnityWebRequest.Get(finalUri);
            request.timeout = Config.RequestTimeout;

            // Custom request headers
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

            Debug.Log($"WebRequestHandler: GET {finalUri.OriginalString}");
            try
            {
                await request.SendWebRequest();
            }
            catch (UnityWebRequestException ex)
            {
                if (ex.Result != UnityWebRequest.Result.Success)
                {
                    if (ex.ResponseCode == 401)
                        throw new APIErrorException(ErrorType.AuthorizationError, ex.Message);
                    throw new APIErrorException(ex.Result, ex.Message);
                }
            }

            var responseText = request.downloadHandler.text;
            if (string.IsNullOrEmpty(responseText))
                throw new APIErrorException(ErrorType.JsonError, "Response is empty");

            return responseText;
        }

        public async UniTask<string> Post(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            var finalUri = ConstructFullUri(host, path, queries);
            var payload = Encoding.UTF8.GetBytes(body);

            using var request = new UnityWebRequest(finalUri, UnityWebRequest.kHttpVerbPOST);
            var downloader = new DownloadHandlerBuffer();
            UploadHandler uploader = new UploadHandlerRaw(payload);
            uploader.contentType = "text/plain";
            request.downloadHandler = downloader;
            request.uploadHandler = uploader;

            request.timeout = Config.RequestTimeout;

            // Custom request headers
            if (headers != null)
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                    if (header.Key.ToLower() == "content-type")
                        uploader.contentType = header.Value;
                }

            Debug.Log($"WebRequestHandler: POST {finalUri.OriginalString}");
            try
            {
                await request.SendWebRequest();
            }
            catch (UnityWebRequestException ex)
            {
                if (ex.Result != UnityWebRequest.Result.Success)
                {
                    if (ex.ResponseCode == 401)
                        throw new APIErrorException(ErrorType.AuthorizationError, ex.Message);
                    throw new APIErrorException(ex.Result, ex.Message);
                }
            }

            var responseText = request.downloadHandler.text;
            if (string.IsNullOrEmpty(responseText))
                throw new APIErrorException(ErrorType.JsonError, "Response is empty");

            return responseText;
        }

        public async UniTask<string> Put(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            var finalUri = ConstructFullUri(host, path, queries);
            using var request = UnityWebRequest.Put(finalUri, body);
            request.timeout = Config.RequestTimeout;

            // Custom request headers
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

            Debug.Log($"WebRequestHandler: PUT {finalUri.OriginalString}");
            try
            {
                await request.SendWebRequest();
            }
            catch (UnityWebRequestException ex)
            {
                if (ex.Result != UnityWebRequest.Result.Success)
                {
                    if (ex.ResponseCode == 401)
                        throw new APIErrorException(ErrorType.AuthorizationError, ex.Message);
                    throw new APIErrorException(ex.Result, ex.Message);
                }
            }

            var responseText = request.downloadHandler.text;
            if (string.IsNullOrEmpty(responseText))
                throw new APIErrorException(ErrorType.JsonError, "Response is empty");

            return responseText;
        }

        public async UniTask<string> Delete(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null)
        {
            var finalUri = ConstructFullUri(host, path, queries);
            using var request = UnityWebRequest.Delete(finalUri);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = Config.RequestTimeout;

            // Custom request headers
            if (headers != null)
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);

            Debug.Log($"WebRequestHandler: DELETE {finalUri.OriginalString}");
            try
            {
                await request.SendWebRequest();
            }
            catch (UnityWebRequestException ex)
            {
                if (ex.Result != UnityWebRequest.Result.Success)
                {
                    if (ex.ResponseCode == 401)
                        throw new APIErrorException(ErrorType.AuthorizationError, ex.Message);
                    throw new APIErrorException(ex.Result, ex.Message);
                }
            }

            var responseText = request.downloadHandler.text;
            if (string.IsNullOrEmpty(responseText))
                throw new APIErrorException(ErrorType.JsonError, "Response is empty");

            return responseText;
        }

        private Uri ConstructFullUri(string host, string path, Dictionary<string, string> queries = null)
        {
            // Query parameters
            var queryString = HttpUtility.ParseQueryString("");
            if (queries != null)
                foreach (var param in queries)
                    queryString.Add(param.Key, param.Value);

            var uriBuilder = new UriBuilder(host + path)
            {
                Query = queryString.ToString()
            };

            return uriBuilder.Uri;
        }
    }
}