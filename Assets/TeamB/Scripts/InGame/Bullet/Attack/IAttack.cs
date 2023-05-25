using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IAttack
{
    #region PublicMethod

    public UniTask Attack();

    #endregion
}
