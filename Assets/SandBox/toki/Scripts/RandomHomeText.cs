using System;
using System.Collections;
using System.Collections.Generic;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomHomeText : MonoBehaviour
{
    [SerializeField, Header("透明のボタン")] private CommonButton clearButton;
    [SerializeField, Header("立ち絵とテキスト")] private List<GameObject> _standImageAndTextObjects;

    private int _currentIndex;
    private int _pastIndex = 0;
    
    private void Start()
    {
        RandomActive();
        clearButton.OnClick.TakeUntilDestroy(gameObject).Subscribe(_ => RandomActive());
    }

    private void RandomActive()
    {
        _currentIndex = Random.Range(0, _standImageAndTextObjects.Count);
        _standImageAndTextObjects[_currentIndex].SetActive(true);
        if (_currentIndex != _pastIndex)
        {
            _standImageAndTextObjects[_pastIndex].SetActive(false);
        }
        _pastIndex = _currentIndex;
    }
}
