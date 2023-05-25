using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// プレイヤーに紐づいた画像のアセット
/// </summary>
[CreateAssetMenu(fileName = "ImageAssociatedPlayerAsset", menuName = "ScriptableObjects/CreateImageAssociatedPlayerAsset")]
public class ImageAssociatedPlayerAsset : ScriptableObject
{
    // プレイヤータイプ
    [FormerlySerializedAs("_playerType")] [SerializeField, Header("プレイヤータイプ")] private Player.PlayerType playerTypeType;
    
    // ピクミン画像（エネミー3種類 * アニメーション差分）
    // TODO: アニメーションでもつか画像でもつか
    [SerializeField, Header("エネミー画像")] private List<EnemySpriteAssociatedPlayer> _enemyImageAssociatedPlayersLIst;

    // プレイヤー画像（プレイヤー1種類 * アニメーション差分）
    [SerializeField, Header("プレイヤ画像")] private List<Sprite> _playerSpriteList;

    // プレイヤースタン画像
    [SerializeField, Header("プレイヤスタン画像")] private Sprite _playStanSprite;

    // レティクル（照準）画像
    [SerializeField, Header("照準画像")] private Sprite _aimingSprite;
    
    // 矢印キー画像
    [SerializeField, Header("Arrow")] private Sprite _arrowSprite;

    // プレイヤータイプ取得
    public Player.PlayerType GetPlayerType()
    {
        return playerTypeType;
    }

    // あるエネミーの画像リストのカウント取得
    public int GetEnemyImageListCount(EnemyType.Enemy enemyType)
    {
        foreach (var enemyImageAssociatedPlayer in _enemyImageAssociatedPlayersLIst)
        {
            if (enemyImageAssociatedPlayer.GetEnemyType() == enemyType)
            {
                return enemyImageAssociatedPlayer.GetListCount();
            }
        }

        Debug.LogError("設定されていないEnemyTypeが指定された");
        return 0;
    }
    
    // プレイヤの画像リストのカウント取得
    public int GetPlayerSpriteListCount()
    {
        return _playerSpriteList.Count;
    }
    
    // エネミー画像リストを取得
    public List<Sprite> GetEnemySpriteList(EnemyType.Enemy enemyType)
    {
        
        foreach (var enemyImageAssociatedPlayer in _enemyImageAssociatedPlayersLIst)
        {
            if (enemyImageAssociatedPlayer.GetEnemyType() == enemyType)
            {
                return enemyImageAssociatedPlayer.GetEnemySpriteList();
            }
        }

        Debug.LogError("設定されていないEnemyTypeが指定された");
        return new List<Sprite>();
    }
    
    // プレイヤー画像リスト取得
    public List<Sprite> GetPlayerSpriteList()
    {
        return _playerSpriteList;
    }
    
    // プレイヤースタン画像取得
    public Sprite GetPlayerSprite()
    {
        return _playStanSprite;
    }
    
    // 照準の画像取得
    public Sprite GetAimingSprite()
    {
        return _aimingSprite;
    }
    
    // 矢印の画像取得
    public Sprite GetArrowSprite()
    {
        return _arrowSprite;
    }

    [Serializable]
    public class EnemySpriteAssociatedPlayer
    {
        [SerializeField, Header("エネミータイプ")] private EnemyType.Enemy _enemyType;
        [SerializeField, Header("エネミー画像リスト")] private List<Sprite> _enemyImageList;

        public EnemyType.Enemy GetEnemyType()
        {
            return _enemyType;
        }

        public List<Sprite> GetEnemySpriteList()
        {
            return _enemyImageList;
        }

        public int GetListCount()
        {
            return _enemyImageList.Count;
        }
    }
}
