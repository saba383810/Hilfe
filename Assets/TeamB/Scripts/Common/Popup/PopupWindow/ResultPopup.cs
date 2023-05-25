using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class ResultPopup : Popup
    {
        [SerializeField] private CommonButton nextButton;
        [SerializeField] private ResultCellView[] resultCellViewArray;

        public void Setup(ResultCellData[] resultCellDataArray, Action disconnect = null)
        {
            base.Setup();
            for (var i = 0; i < resultCellDataArray.Length; i++)
            {
                resultCellViewArray[i].Setup(resultCellDataArray[i]);
            }
            
            nextButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe( async =>
            {
                disconnect?.Invoke();
                Hide();
            });
        }
        
    }
}