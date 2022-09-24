using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class CharacterInstantiator : MonoBehaviourPunCallbacks
{
    // [SerializeField] private SpawnPoint _spawn;
    [SerializeField] private List<SpawnPoint> _spawns; // Temp to TEST
    
    
    
    private void Start()
    {

        var spawn = _spawns.First(point => !point.Occupied);
        _spawns.ForEach(p => Debug.Log($"spawn: {p.photonView.ViewID} occupied: {p.Occupied} IsMine: {p.photonView.IsMine}"));
        var character = PhotonNetwork.Instantiate("Character", spawn.transform.position, spawn.transform.rotation).GetComponentInChildren<CharacterController>();
    
        character.SetFlip(spawn.Flip);
        character.SetVertical(spawn.Vertical);

        spawn.SetOccupied(true);
        
    }
}
