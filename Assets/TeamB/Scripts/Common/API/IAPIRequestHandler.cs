using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace TeamB.Scripts.Common.API
{
    public interface IAPIRequestHandler
    {
        UniTask<string> Get(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null);

        UniTask<string> Post(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null);

        UniTask<string> Put(
            string host,
            string path,
            string body,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null);

        UniTask<string> Delete(
            string host,
            string path,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> headers = null);
    }
}