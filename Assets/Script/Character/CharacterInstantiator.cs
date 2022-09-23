using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterInstantiator : MonoBehaviourPun
{
    // [SerializeField] private SpawnPoint _spawn;
    [SerializeField] private List<SpawnPoint> _spawns; // Temp to TEST
    private void Start()
    {
        foreach (var _spawn in _spawns)
        {
            var character = PhotonNetwork.Instantiate("Character", _spawn.transform.position, _spawn.transform.rotation).GetComponentInChildren<CharacterController>();
        
            character.SetFlip(_spawn.Flip);
            character.SetVertical(_spawn.Vertical);
        }
    }
}
