using UniRx;
using UnityEngine;

namespace TeamB.Scripts.Common.Event
{
    public class GlobalEventHelper : MonoBehaviour
    {
        public static Subject<int> Player1EngelCntGlobalEvent { get; set; }
        public static Subject<int> Player2EngelCntGlobalEvent { get; set; }
        public static Subject<int> Player3EngelCntGlobalEvent { get; set; }
        public static Subject<int> Player4EngelCntGlobalEvent { get; set; }
    }
}
