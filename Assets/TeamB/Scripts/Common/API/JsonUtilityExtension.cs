using System.Collections.Generic;
using UnityEngine;

namespace TeamB.Scripts.Common.API
{
    /// <summary>
    ///     JSON ⇔ オブジェクト間の変換へのショートカットを提供します。
    /// </summary>
    public static class JsonUtilityExtension
    {
        /// <summary>
        ///     <see cref="string" /> を JSON 形式の文字列と仮定し {T} 型への変換を試行します。
        /// </summary>
        /// <remarks>
        ///     内部実装に <see cref="JsonUtility.ToJson(object)" /> を使用しているので
        ///     変換可能な規則は Unity の <see cref="UnityEngine.JsonUtility" /> に準拠します。
        /// </remarks>
        public static T FromJson<T>(this string self)
        {
            return JsonUtility.FromJson<T>(self);
        }

        /// <summary>
        ///     <see cref="string" /> を Root 要素を持たない JSON 配列形式の文字列と仮定し T[] 型への変換を試行します。
        /// </summary>
        public static T[] FromJsonArray<T>(this string self)
        {
            return JsonHelper.FromJson<T>(self);
        }

        /// <summary>
        ///     指定したオブジェクトを JSON 文字列に変換します。
        /// </summary>
        /// <remarks>
        ///     内部実装に <see cref="JsonUtility.ToJson(object)" /> を使用しているので
        ///     変換可能な規則は Unity の <see cref="UnityEngine.JsonUtility" /> に準拠します。
        /// </remarks>
        public static string FromJson<T>(this object self)
        {
            return JsonUtility.ToJson(self);
        }

        /// <summary>
        ///     指定したオブジェクトを Root 要素を持たない JSON 配列に変換します。
        /// </summary>
        public static string FromJsonArray<T>(this IEnumerable<T> self)
        {
            return JsonHelper.ToJson(self);
        }
    }
}