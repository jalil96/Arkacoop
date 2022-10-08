using System;
using System.Runtime.InteropServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPoint : MonoBehaviourPun
{
    [SerializeField] private bool _flip;
    [SerializeField] private bool _vertical;
    [SerializeField] private bool _occupied;
    [SerializeField] private WallController _wallController;
    [SerializeField] private Player _playerReference;
    [SerializeField] private CharacterUI _characterInformation;


    private void Awake()
    {
        _characterInformation.gameObject.SetActive(false);
    }

    private void Start()
    {
        // Debug.Log($"Spawnpoint: {photonView.ViewID} is mine: {photonView.IsMine}");
        if (photonView.IsMine) return;
        photonView.RPC(nameof(RequestOccupied), photonView.Owner, PhotonNetwork.LocalPlayer);
    }

    public bool Flip => _flip;
    public bool Vertical => _vertical;
    public bool Occupied => _occupied;
    public Player PlayerReference => _playerReference;
    public CharacterUI CanvasInfo => _characterInformation;

    public void SetOccupied(bool occupied)
    {
        _occupied = occupied;
        _wallController.Activate(!_occupied);
        photonView.RPC(nameof(UpdateOccupied), RpcTarget.OthersBuffered, _occupied);
    }

    public void SetPlayerReference(Player player)
    {
        _playerReference = player;
        _characterInformation.playerNickName.text = player.NickName;
        CanvasInfo.Setup(true, player);
        photonView.RPC(nameof(UpdatePlayerReference), RpcTarget.OthersBuffered, _playerReference);
    }

    [PunRPC]
    public void UpdatePlayerReference(Player player)
    {
        _playerReference = player;
        _characterInformation.playerNickName.text = player.NickName;
    }

    [PunRPC]
    public void UpdateOccupied(bool occupied)
    {
        //Debug.Log($"Updating spawnpoint {photonView.ViewID} for player {PhotonNetwork.LocalPlayer.NickName}, sending value {occupied}");
        _occupied = occupied;
    }

    [PunRPC]
    public void RequestOccupied(Player player)
    {
        //Debug.Log($"Requested update spawnpoint {photonView.ViewID} for player {player.NickName}, sending value {_occupied}");
        photonView.RPC(nameof(UpdateOccupied), player, _occupied);
    }
}