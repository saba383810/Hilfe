using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreConsumption : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public void ScoreConsumptionAnimation()
    {
        _animator.SetBool("ScoreConsumption", true);
        Invoke("ToFalse", 1f);
    }

    public void ToFalse()
    {
        _animator.SetBool("ScoreConsumption", false);
    }
}
