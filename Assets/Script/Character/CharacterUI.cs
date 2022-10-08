using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviourPun
{
    public TextMeshProUGUI playerNickName;
    public TextMeshProUGUI score;

    private bool _active;
    
    private void Start()
    {
        photonView.RPC(nameof(RequestSetup), photonView.Owner, PhotonNetwork.LocalPlayer);
        photonView.RPC(nameof(RequestScore), photonView.Owner, PhotonNetwork.LocalPlayer);
    }

    public void Setup(bool active, Player player)
    {
        photonView.TransferOwnership(player);
        photonView.RPC(nameof(UpdateSetup), RpcTarget.AllBuffered, active);
    }

    public void SetScore(string scoreValue)
    {
        photonView.RPC(nameof(UpdateScore), RpcTarget.AllBuffered, scoreValue);
    }

    [PunRPC]
    private void UpdateSetup(bool active)
    {
        _active = active;
        gameObject.SetActive(active);
    }

    [PunRPC]
    private void RequestSetup(Player player)
    {
        photonView.RPC(nameof(UpdateSetup), player, _active);
    }

    [PunRPC]
    private void UpdateScore(string scoreValue)
    {
        score.text = scoreValue;
    }

    [PunRPC]
    private void RequestScore(Player player)
    {
        photonView.RPC(nameof(UpdateScore), player, score.text);
    }
}
