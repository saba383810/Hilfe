using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイアブルキャラパラメータアセット
/// </summary>
[CreateAssetMenu(fileName = "PlayableCharacterStatus", menuName = "ScriptableObjects/CreatePlayableCharacterParameterAsset")]
public class PlayableCharacterParameterAsset : ScriptableObject
{
    [SerializeField, Header("最大速度")]
    private float _maxVelocity = 10f;

    public float GetMaxVelocity()
    {
        return _maxVelocity;
    }
}
