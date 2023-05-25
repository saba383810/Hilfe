using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : MonoBehaviour
{
    // シングルトン参照
    //[SerializeField, Header("スピードの倍率")] private float _speedMultiplier = 1.5f;
    private float _speedMultiplier;
    
    // タグ定義
    private const string Player_TAG = "Player";

    private void Awake()
    {
        _speedMultiplier = LevelDesignSingleton.Instance.GetSpeedUpItemMultiplier();
    }

    public float GetSpeedMultiplier()
    {
        return _speedMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D enterCollider)
    {
        var isPlayer = enterCollider.CompareTag(Player_TAG);
        if (isPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
