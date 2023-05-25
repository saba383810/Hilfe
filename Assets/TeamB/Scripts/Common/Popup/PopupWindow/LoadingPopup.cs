using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class LoadingPopup : Popup
{
    [SerializeField] private LoadingSceneController _loadingSceneController;

    public override void Setup()
    {
        base.Setup();
        _loadingSceneController.Show();
    }
}
