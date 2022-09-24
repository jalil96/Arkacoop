using System;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    private BallModel _ballModel;
    private CharacterModel _lastCollision;
    
    private void Start()
    {
        if (!photonView.IsMine) Destroy(this);
        _ballModel = GetComponent<BallModel>();
        _ballModel.InitDirection();
    }

    private void Update()
    {
        _ballModel.Move();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var character = col.gameObject.GetComponent<CharacterModel>();
        if (character != null && character != _lastCollision)
        {
            _ballModel.ChangeDirection(character.Vertical, character.Flip);
            _lastCollision = character;
        }
    }

}