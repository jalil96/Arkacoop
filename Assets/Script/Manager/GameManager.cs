using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered!!");
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetButtonDown("Submit"))
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        var obj = PhotonNetwork.Instantiate("Ball", Vector3.zero, Quaternion.identity);
        var ball = obj.GetComponent<BallModel>();

        if (ball != null)
        {
            // Hacer algo?
        }
    }
}
