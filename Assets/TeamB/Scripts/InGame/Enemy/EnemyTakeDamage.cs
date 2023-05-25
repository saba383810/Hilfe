using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// エネミーが被弾した時の挙動を制御する
/// </summary>
public class EnemyTakeDamage : MonoBehaviour
{
    [SerializeField, Header("ダメージテキストのポジションZ")]
    private float _damageTextPositionZ = -1f;

    [SerializeField, Header("ハートエフェクト")] private DamageText _hartEffect;
    
    // ダメージ数
    private int _damageNum;
    
    // ダメージテキストのAddressableKey
    //private const string _damageTextAddressableKye = "EnemyDamageText.prefab";
    private const string _damageTextAddressableKye = "NormalDamageEffect.prefab";

    public async UniTask CallDamageText()
    {
        //var damageTextPrefab = await GetDamageText(_damageTextAddressableKye);
        //var damageText = Instantiate(damageTextPrefab, transform);
        //damageText.GetComponent<DamageText>().Initialization(Vector3.zero);
        _hartEffect.DamageTextAnimation();
    }

    private Vector3 SetDamagePosition()
    {
        var positionX = 0f;//Random.Range(-2.5f, 2.5f);
        var positionY = 0f;//Random.Range(0f, 2.5f);
        var positionZ = _damageTextPositionZ;
        var localPosition = new Vector3(positionX, positionY, positionZ);
        return localPosition;
    }
    
    private static async UniTask<GameObject> GetDamageText(string damageTextName) 
    {
        var bulletPrefab = await Addressables.LoadAssetAsync<GameObject>(damageTextName);
        Debug.Assert(bulletPrefab != null, $"指定したPopupが見つかりませんでした。Addressable Groupsを確認してください。\nbulletName : {damageTextName}"); 
        return bulletPrefab;
}
}
