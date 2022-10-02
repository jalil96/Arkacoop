using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BrickController : MonoBehaviourPun
{
    [SerializeField] private BrickModel _brickModel;

    private void Start()
    {
        if (!photonView.IsMine) Destroy(this);
    }

}
