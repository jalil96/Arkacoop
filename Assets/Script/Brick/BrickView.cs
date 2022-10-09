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
    [SerializeField] private Animator _animator;
    private static readonly int Dying = Animator.StringToHash("Dying");
    private static readonly int Death = Animator.StringToHash("Death");

    private void Start()
    {
        _text.text = _model.Hits.ToString();
        if (!photonView.IsMine) return;
        _model.OnBrickHited += BrickHited;
        _model.OnBrickDestroyed += BrickDestroyed;
    }

    private void BrickHited()
    {
        Debug.Log("Brick hited");
        if (!photonView.IsMine) return;
        _text.text = _model.Hits.ToString();
        photonView.RPC(nameof(UpdateHitsText), RpcTarget.Others, _model.Hits);
        
        if (_model.Hits == 1)
        {
            _animator.SetBool(Dying, true);
        }
    }

    private void BrickDestroyed()
    {
        _animator.SetBool(Death, true);
    }

    [PunRPC]
    private void UpdateHitsText(int hits)
    {
        _text.text = hits.ToString();
    }
    
}
