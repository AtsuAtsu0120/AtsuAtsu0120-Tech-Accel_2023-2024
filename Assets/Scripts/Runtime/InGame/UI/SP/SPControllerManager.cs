using System;
using System.Collections;
using System.Collections.Generic;
using InGame.Base;
using UnityEngine;

public class SPControllerManager : UIManagerBase
{
    [SerializeField] private MoveBase _moveBase;

    [SerializeField] private SPViewpointDrager _viewpoint;
    [SerializeField] private SPVirtualStick _spVirtualStick;
    private void Start()
    {
        #if PLATFORM_ANDROID || PLATFORM_IOS || PLATFORM_WEBGL
        gameObject.SetActive(true);
        #elif PLATFORM_STANDALONE
        gameObject.SetActive(false);
        #endif
    }

    private void Update()
    {
        OnLook();
        OnMove();
    }

    public void OnAction()
    {
        _moveBase.Interact();
    }

    public void OnSprint(bool isSprint)
    {
        _moveBase.IsSprint = isSprint;
    }

    public void OnMove()
    {
        // 直感とは逆方向の入力なので-1をかけて修正
        _moveBase.Move(_spVirtualStick.MoveInput * -1);
    }

    public void OnLook()
    {
        _moveBase.Look(_viewpoint.LookInput);
    }
}
