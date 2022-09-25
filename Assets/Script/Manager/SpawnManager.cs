using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
     [SerializeField] private List<SpawnPoint> _spawns;
     
     
     
     public override void OnPlayerEnteredRoom(Player newPlayer)
     {
          Debug.Log($"NewPlayer entered: {newPlayer.NickName}");
          if (!PhotonNetwork.IsMasterClient) return;
          var spawn = _spawns.First(point => !point.Occupied);
          var spawnIndex = _spawns.IndexOf(spawn);
          Debug.Log($"Executing {nameof(SpawnCharacter)} for player {newPlayer.NickName} spawn index {spawnIndex}");
          
          photonView.RPC(nameof(SpawnCharacter), newPlayer, spawnIndex);
     }

     [PunRPC]
     private void SpawnCharacter(int spawnIndex)
     {
          var spawn = _spawns[spawnIndex];
          
          var character = PhotonNetwork.Instantiate("Character", spawn.transform.position, spawn.transform.rotation).GetComponentInChildren<CharacterController>();
    
          character.SetFlip(spawn.Flip);
          character.SetVertical(spawn.Vertical);

          spawn.SetOccupied(true);
     }

}