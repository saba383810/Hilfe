using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/// <summary>
/// インゲームにおける他のプレイヤーの情報を動かす
/// </summary>
public class IngamePlayerInfoMove : MonoBehaviour
{
    [SerializeField, Header("カーソルのRoot")] private Transform _cursorRoot;
    [SerializeField, Header("カーソル")] private RectTransform _cursorRectTransform;

    [SerializeField, Header("中心に合わせるための調整用")] private Vector2 _fixMisalignment;
    [SerializeField, Header("ターゲットの(Rect)Transformから上方向にずらす距離")] private Vector2 _topMisalignment;
    [SerializeField, Header("ターゲットの(Rect)Transformから左方向にずらす距離")] private Vector2 _leftMisalignment;
    [SerializeField, Header("ターゲットの(Rect)Transformから右方向にずらす距離")] private Vector2 _rightMisalignment;
    [SerializeField, Header("ターゲットの(Rect)Transformから下方向にずらす距離")] private Vector2 _bottomMisalignment;
    [SerializeField, Header("HeaderUI用の境界線バッファ")] private float _topBuffer;
    [SerializeField, Header("FooterUI用の境界線バッファ")] private float _bottomBuffer;
    [SerializeField, Header("左の境界線バッファ")] private float _leftBuffer;
    [SerializeField, Header("右の境界線バッファ")] private float _rightBuffer;

    // 追従対象
    private Transform _targetTransform;
    private Camera _camera;
    
    [SerializeField, Header("キャンバスのルート")]
    private Canvas _canvas;
    private RectTransform _canvasRectTransform;
    // セーフエリア
    [SerializeField, Header("セーフエリア")]
    private RectTransform _safeAreaRectTransform;
    private RectTransform _thisRectTransform;
    
    //　カメラからキャンバスにするための倍率
    private float _cameraToCanvasRatio;
    
    // セーフエリアからセーフラインを設定
    private float _topSafeLine;
    private float _leftSafeLine;
    private float _rightSafeLine;
    private float _bottomSafeLine;

    private Vector3 _originalTargetRectTransformPosition;

    private bool isInitialized = false;
    
    private void Update()
    {
        if (isInitialized)
        {
            Move();
        }
    }

    public void Initialization(Transform targetTransform)//, Camera camera, GameObject canvas, RectTransform safeArea)
    {
        _camera = Camera.main;
        
        _thisRectTransform = gameObject.GetComponent<RectTransform>();
        SetTargetTransform(targetTransform);
        var canvasScaler = _canvas.GetComponent<CanvasScaler>();
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        _cameraToCanvasRatio = (canvasScaler.referenceResolution.y * _canvasRectTransform.localScale.y) / (_camera.orthographicSize * 2);
        
        _topSafeLine = (_safeAreaRectTransform.rect.height / 2f) - _topBuffer - _cursorRectTransform.localPosition.x - (_cursorRectTransform.rect.height / 2f);
        _bottomSafeLine = (- _safeAreaRectTransform.rect.height / 2f) + _bottomBuffer + _cursorRectTransform.localPosition.x + (_cursorRectTransform.rect.height / 2f);
        _leftSafeLine = (-_safeAreaRectTransform.rect.width / 2f) + _leftBuffer + _cursorRectTransform.localPosition.x + (_cursorRectTransform.rect.height / 2f);
        _rightSafeLine = (_safeAreaRectTransform.rect.width / 2f) - _rightBuffer - _cursorRectTransform.localPosition.x - (_cursorRectTransform.rect.height / 2f);

        isInitialized = true;
    }

    public void Move()
    {
        // IngamePlayerInfoを移動させる
        MovePlayerInfo();
    }

    private void SetTargetTransform(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }

    private void MovePlayerInfo()
    {
        // ターゲットの位置（RectTransformを取ってくる）算出
        var targetRectTransformPosition = GetTargetRectPosition();
        
        // キャンバスサイズに応じて調整
        _originalTargetRectTransformPosition = targetRectTransformPosition;

        // ズレを修正
        _originalTargetRectTransformPosition =
            CalculateMisalignment(_originalTargetRectTransformPosition, _fixMisalignment);
        
        // ターゲットの頭上の位置（頭上ターゲット）算出
        var currentTargetRectTransformPosition =
            CalculateMisalignment(_originalTargetRectTransformPosition, _topMisalignment);

        //　頭上ターゲットがセーフエリア内か判別し、セーフエリア内ならターゲットの頭上の位置に移動し、セーフエリア外ならターゲットの左の位置（左ターゲット）を算出
        if (InSafeArea(currentTargetRectTransformPosition))
        {
            _thisRectTransform.localPosition = currentTargetRectTransformPosition;
            // カーソルルートを回転させる
            _cursorRoot.rotation = Quaternion.Euler(0f, 0f, -90f);
            return;
        }
            
        currentTargetRectTransformPosition =
            CalculateMisalignment(_originalTargetRectTransformPosition, _leftMisalignment);

        //　左ターゲットがセーフエリア内か判別し、セーフエリア内ならターゲットの頭上の位置に移動し、セーフエリア外ならターゲットの右の位置（右ターゲット）を算出
        if (InSafeArea(currentTargetRectTransformPosition))
        {
            _thisRectTransform.localPosition = currentTargetRectTransformPosition;
            // カーソルルートを回転させる
            _cursorRoot.rotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }
        
        currentTargetRectTransformPosition =
            CalculateMisalignment(_originalTargetRectTransformPosition, _rightMisalignment);
        
        //　右ターゲットがセーフエリア内か判別し、セーフエリア内ならターゲットの頭上の位置に移動し、セーフエリア外ならターゲットの足元の位置（足元ターゲット）を算出
        if (InSafeArea(currentTargetRectTransformPosition))
        {
            _thisRectTransform.localPosition = currentTargetRectTransformPosition;
            // カーソルルートを回転させる
            _cursorRoot.rotation = Quaternion.Euler(0f, 0f, 180f);
            return;
        }
        
        currentTargetRectTransformPosition =
            CalculateMisalignment(_originalTargetRectTransformPosition, _bottomMisalignment);

        //　足元ターゲットがセーフエリア内か判別し、セーフエリア内ならターゲットの頭上の位置に移動し
        if (InSafeArea(currentTargetRectTransformPosition))
        {
            _thisRectTransform.localPosition = currentTargetRectTransformPosition;
            // カーソルルートを回転させる
            _cursorRoot.rotation = Quaternion.Euler(0f, 0f, 90f);
            return;
        }
        
        //セーフエリア外なら画面中心とターゲットを結ぶ直線とセーフエリアの交点を算出し、そこに移動
        if (_originalTargetRectTransformPosition.y > 0)
        {
            // 上
            var intersection = CalculateIntersectionTop(_originalTargetRectTransformPosition);
            if (InSafeArea(intersection))
            {
                _thisRectTransform.localPosition = intersection;
                // カーソルルートを回転させる
                var angle = Vector2Extensions.Vector2ToAngle(_originalTargetRectTransformPosition);
                _cursorRoot.rotation = Quaternion.Euler(0f, 0f, angle);
                return;
            }
        }
        else
        {
            // 下
            var intersection = CalculateIntersectionBottom(_originalTargetRectTransformPosition);
            if (InSafeArea(intersection))
            {
                _thisRectTransform.localPosition = intersection;
                // カーソルルートを回転させる
                var angle = Vector2Extensions.Vector2ToAngle(_originalTargetRectTransformPosition);
                _cursorRoot.rotation = Quaternion.Euler(0f, 0f, angle);
                return;
            }
        }

        if (_originalTargetRectTransformPosition.x < 0)
        {
            // 左
            var intersection = CalculateIntersectionLeft(_originalTargetRectTransformPosition);
            if (InSafeArea(intersection))
            {
                _thisRectTransform.localPosition = intersection;
                // カーソルルートを回転させる
                var angle = Vector2Extensions.Vector2ToAngle(_originalTargetRectTransformPosition);
                _cursorRoot.rotation = Quaternion.Euler(0f, 0f, angle);
                return;
            }
        }
        else
        {
            // 右
            var intersection = CalculateIntersectionRight(_originalTargetRectTransformPosition);
            if (InSafeArea(intersection))
            {
                _thisRectTransform.localPosition = intersection;
                // カーソルルートを回転させる
                var angle = Vector2Extensions.Vector2ToAngle(_originalTargetRectTransformPosition);
                _cursorRoot.rotation = Quaternion.Euler(0f, 0f, angle);
                return;
            }
        }
        
        Debug.LogError("交点が全てセーフエリア外");
    }

    // ターゲット位置算出
    private Vector3 GetTargetRectPosition()
    {
        var targetRectPosition = Vector2.zero;

        var rect = _canvasRectTransform.rect;
        var screenPosition =
            RectTransformUtility.WorldToScreenPoint(_camera, _targetTransform.position - _camera.transform.position);// + new Vector3(rect.position.x / 2f, rect.position.y / 2f, 0f));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPosition, _camera,
            out targetRectPosition);

        return (targetRectPosition - new Vector2(rect.position.x, rect.position.y)) * _cameraToCanvasRatio;
    }
    
    // ずらす計算
    private Vector3 CalculateMisalignment(Vector3 _originalTargetRectTransformPosition, Vector2 misalignment)
    {
        return _originalTargetRectTransformPosition + new Vector3(misalignment.x, misalignment.y, 0f);
    }
    
    // セーフエリア内かの判定
    private bool InSafeArea(Vector3 currentTargetRectTransformPosition)
    {
        // 上方向の判定
        if (currentTargetRectTransformPosition.y > _topSafeLine) return false;
        
        // 下方向の判定
        if (currentTargetRectTransformPosition.y < _bottomSafeLine) return false;
        
        //　左方向の判定
        if (currentTargetRectTransformPosition.x < _leftSafeLine) return false;
        
        //　右方向の判定
        if (currentTargetRectTransformPosition.x > _rightSafeLine) return false;
        
        return true;
    }
    
    // 左のセーフエリアとの交点
    private Vector3 CalculateIntersectionLeft(Vector3 _originalTargetTransform)
    {
        var intersectionY = (_originalTargetTransform.y / _originalTargetTransform.x) * _leftSafeLine;
        return new Vector3(_leftSafeLine, intersectionY, _originalTargetTransform.z);
    }
    
    // 右のセーフエリアとの交点
    private Vector3 CalculateIntersectionRight(Vector3 _originalTargetTransform)
    {
        var intersectionY = (_originalTargetTransform.y / _originalTargetTransform.x) * _rightSafeLine;
        return new Vector3(_rightSafeLine, intersectionY, _originalTargetTransform.z);
    }
    
    // 上のセーフエリアとの交点
    private Vector3 CalculateIntersectionTop(Vector3 _originalTargetTransform)
    {
        var intersectionX = (_originalTargetTransform.x / _originalTargetTransform.y) * _topSafeLine;
        return new Vector3(intersectionX, _topSafeLine, _originalTargetTransform.z);
    }
    
    // 下のセーフエリアとの交点
    private Vector3 CalculateIntersectionBottom(Vector3 _originalTargetTransform)
    {
        var intersectionX = (_originalTargetTransform.x / _originalTargetTransform.y) * _bottomSafeLine;
        return new Vector3(intersectionX, _bottomSafeLine, _originalTargetTransform.z);
    }
}
