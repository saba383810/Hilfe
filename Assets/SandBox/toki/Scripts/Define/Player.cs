using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤータイプ
/// </summary>
public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        TypeA,
        TypeB,
        TypeC,
        TypeD,
    }
    
    public enum PlayerStatus 
    {
        Attack,
        StandBy,
        Stan,
    }
}
