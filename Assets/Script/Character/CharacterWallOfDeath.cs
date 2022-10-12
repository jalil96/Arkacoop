using System;
using Photon.Pun;
using UnityEngine;


public class CharacterWallOfDeath : MonoBehaviourPun
{
    public Action OnBallEnter = delegate {  };
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var ball = col.gameObject.GetComponent<BallModel>();
        if (ball != null)
        {
            photonView.RPC(nameof(BallEnter), photonView.Owner);
        }
    }

    [PunRPC]
    private void BallEnter()
    {
        OnBallEnter.Invoke();
    }
    
}