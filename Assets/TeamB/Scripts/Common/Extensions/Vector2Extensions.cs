using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Extensions : MonoBehaviour
{
    /// <summary>
    /// 角度から単位ベクトルを取得
    /// </summary>
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }
    
    public static float Vector2ToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
    
    /// <summary>
    ///   単位ベクトルを取得
    /// </summary>
    public static Vector2 DeltaToDirection(Vector2 delta)
    {
        var angle = Vector2ToAngle(delta);
        var direction = AngleToVector2(angle);
        return direction;
    }
}
