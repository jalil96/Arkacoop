using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BallManager : MonoBehaviour
{

    // [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private List<Transform> _ballSpawnPoints;
    
    
    public void SpawnBall()
    {
        var spawn = _ballSpawnPoints[Random.Range(0, _ballSpawnPoints.Count)];
        var position = spawn.position;
        var rotation = spawn.rotation;
        PhotonNetwork.Instantiate("Ball", position, rotation);
        
    }
    
}
