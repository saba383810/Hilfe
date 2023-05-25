using System;
using UnityEngine.Networking;

namespace TeamB.Scripts.Common.API
{
    public enum ErrorType
    {
        ConnectionError = UnityWebRequest.Result.ConnectionError,
        ProtocolError = UnityWebRequest.Result.ProtocolError,
        DataProcessingError = UnityWebRequest.Result.DataProcessingError,
        AuthorizationError,
        JsonError
    }

    public class APIErrorException : Exception
    {
        public APIErrorException(ErrorType type, string msg) : base(msg)
        {
            Type = type;
        }

        public APIErrorException(ErrorType type, string msg, Exception inner) : base(msg, inner)
        {
            Type = type;
        }

        public APIErrorException(UnityWebRequest.Result requestResult, string msg) : base(msg)
        {
            Type = (ErrorType)Enum.ToObject(typeof(ErrorType), (int)requestResult);
        }

        public ErrorType Type { get; }
    }
}