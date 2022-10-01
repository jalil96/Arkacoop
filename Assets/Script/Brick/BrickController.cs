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

    private void OnCollisionEnter2D(Collision2D col)
    {
        
        var ball = col.gameObject.GetComponent<BallModel>();
        
        if (ball != null)
        {
            Debug.Log("Collisioned with ball");
            _brickModel.Damage(1); // Change to be the ball giving the damage?
        }
    }
}
