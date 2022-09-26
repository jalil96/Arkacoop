using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CharacterInstantiator : MonoBehaviourPunCallbacks
{
    // [SerializeField] private SpawnPoint _spawn;
    [SerializeField] private List<SpawnPoint> _spawns; // Temp to TEST
    
    
    
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; // Only instantiate character if I'm the MasterClient
        var spawn = GetAvailableSpawnPoint();
        InstantiateCharacter(spawn);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"NewPlayer entered: {newPlayer.NickName}");
        if (!PhotonNetwork.IsMasterClient) return; // Only the master sends where the new player is going to spawn
        var spawn = GetAvailableSpawnPoint();
        var spawnIndex = _spawns.IndexOf(spawn);
        Debug.Log($"Executing {nameof(SpawnCharacter)} for player {newPlayer.NickName} spawn index {spawnIndex}");
        photonView.RPC(nameof(SpawnCharacter), newPlayer, spawnIndex);
    }

    [PunRPC]
    private void SpawnCharacter(int spawnIndex)
    {
        var spawn = _spawns[spawnIndex];
        InstantiateCharacter(spawn);
    }

    private SpawnPoint GetAvailableSpawnPoint()
    {
        return _spawns.First(point => !point.Occupied);
    }

    private void InstantiateCharacter(SpawnPoint spawn)
    {
        var character = PhotonNetwork.Instantiate("Character", spawn.transform.position, spawn.transform.rotation).GetComponentInChildren<CharacterController>();
    
        character.SetFlip(spawn.Flip);
        character.SetVertical(spawn.Vertical);

        spawn.SetOccupied(true);
    }
}
