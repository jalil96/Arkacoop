using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterController : MonoBehaviourPun
{
    private CharacterModel _model;

    private void Awake()
    {
        if (!photonView.IsMine) Destroy(this);
        _model = GetComponent<CharacterModel>();
    }

    private void Update()
    {

        var horizontal = Input.GetAxisRaw("Horizontal");
        _model.Move(new Vector2(horizontal, 0));

    }
}
