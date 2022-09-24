using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterController : MonoBehaviourPun
{
    [SerializeField] private bool _vertical;
    [SerializeField] private bool _flip;
    
    private CharacterModel _model;

    private void Awake()
    {
        if (!photonView.IsMine) Destroy(this);
        _model = GetComponent<CharacterModel>();
        _model.Init(_vertical, _flip);
    }

    private void Update()
    {

        var inputDirection = _vertical ? Input.GetAxisRaw("Vertical") : Input.GetAxisRaw("Horizontal");
        inputDirection *= _flip ? -1 : 1;
        _model.Move(new Vector2(inputDirection, 0));
        // _model.Move(vertical ? new Vector2(0, inputDirection) : new Vector2(inputDirection, 0), vertical);

    }

    public void SetFlip(bool flip)
    {
        _flip = flip;
    }

    public void SetVertical(bool vertical)
    {
        _vertical = vertical;
    }
}
