using System.Collections;
using System.Collections.Generic;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

public class EnterButton : MonoBehaviour
{
    [SerializeField, Header("アニメータ")] private Animator _animator;

    public void Enter()
    {
        _animator.SetBool("Enter", true);
    }

    public void Exit()
    {
        _animator.SetBool("Enter", false);
    }
}
