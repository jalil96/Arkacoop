using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class BrickView : MonoBehaviourPunCallbacks
{
    [SerializeField] private BrickModel _model;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        _text.text = _model.Hits.ToString();
        if (!photonView.IsMine) return;
        _model.OnBrickHited += BrickHited;
    }

    private void BrickHited()
    {
        Debug.Log("Brick hited");
        _text.text = _model.Hits.ToString();
        photonView.RPC(nameof(UpdateHitsText), RpcTarget.Others, _model.Hits);
    }

    [PunRPC]
    private void UpdateHitsText(int hits)
    {
        _text.text = hits.ToString();
    }
    
}
