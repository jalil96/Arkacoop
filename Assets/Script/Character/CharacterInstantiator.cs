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
        photonView.RPC(nameof(SendEnteredPlayer), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player: {otherPlayer.NickName} left the room!");
        if(!PhotonNetwork.IsMasterClient) return;
        var spawn = _spawns.Find((spawn) => spawn.PlayerReference.Equals(otherPlayer));
        spawn.SetOccupied(false);
    }
    
    [PunRPC]
    private void SendEnteredPlayer(Player player)
    {
        OnPlayerEnteredRoom(player);
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
        CharacterController character = PhotonNetwork.Instantiate("Character", spawn.transform.position, spawn.transform.rotation).GetComponentInChildren<CharacterController>();

        // CharacterUI canvasInfo = PhotonNetwork.Instantiate(spawn.CanvasInfoPrefab.gameObject.name, spawn.CanvasSpawnPoint.position, spawn.CanvasSpawnPoint.rotation).GetComponentInChildren<CharacterUI>();
        // canvasInfo.gameObject.transform.Rotate(0,0,0, Space.World);
        // canvasInfo.transform.SetParent(spawn.CanvasSpawnPoint);
        //TODO set character info reference in character model to update score and set name;

        character.SetFlip(spawn.Flip);
        character.SetVertical(spawn.Vertical);

        character.SetSpawnPoint(spawn);
        character.gameObject.GetComponent<CharacterView>().SetCharacterUI(spawn.CanvasInfo); // Esto se hace porque necesita de la variable Vertical del character y en el start del view esto no seria posible
        spawn.SetOccupied(true);
        spawn.SetPlayerReference(PhotonNetwork.LocalPlayer);
    }
}
