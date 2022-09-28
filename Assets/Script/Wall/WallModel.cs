using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WallModel : MonoBehaviourPun, ICollisionable
{

    [SerializeField] private bool _active;


    private void Start()
    {
        if (!photonView.IsMine) return;
        photonView.RPC(nameof(RequestActive), photonView.Owner, PhotonNetwork.LocalPlayer);
    }

    public void Activate(bool active)
    {
        _active = active;
        gameObject.SetActive(_active);
        Debug.Log($"Setting active wall: {_active}");
        photonView.RPC(nameof(UpdateActive), RpcTarget.OthersBuffered, _active);
    }

    [PunRPC]
    public void UpdateActive(bool active)
    {
        Debug.Log($"Receive update active: {_active}");
        _active = active;
        gameObject.SetActive(_active);
    }

    [PunRPC]
    public void RequestActive(Player player)
    {
        photonView.RPC(nameof(UpdateActive), player, _active);
    }
    
}