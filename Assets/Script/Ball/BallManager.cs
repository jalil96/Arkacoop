using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallManager : MonoBehaviourPun
{
    [SerializeField] private List<Transform> _ballSpawnPoints;
    [Range(1,12)][SerializeField] private int _maxBallsInScreen;
    [SerializeField] private float _timeBetweenSpawning;
    
    [SerializeField] private List<BallModel> _activeBalls;
    [SerializeField] private GameManager _gameManager;

    private List<CharacterModel> _characters;

    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient) Destroy(this);
        _gameManager.OnGameStarted += Init;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            var spawn = _ballSpawnPoints[Random.Range(0, _ballSpawnPoints.Count)];
            var position = spawn.position;
            var rotation = spawn.rotation;
            var ball = PhotonNetwork.Instantiate("Ball", position, rotation);
            _activeBalls.Add(ball.GetComponent<BallModel>());
        }
    }

    private void Init()
    {
        _characters = FindObjectsOfType<CharacterModel>().ToList();
        InvokeRepeating(nameof(SpawnBall), 0, _timeBetweenSpawning);
    }

    public void SpawnBall()
    {
        _activeBalls = _activeBalls.FindAll(ball => ball != null);
        _characters = _characters.FindAll(character => !character.Dead);
        _maxBallsInScreen = _characters.Count;
        if (_activeBalls.Count >= _maxBallsInScreen) return;
        
        var spawn = _ballSpawnPoints[Random.Range(0, _ballSpawnPoints.Count)];
        var position = spawn.position;
        var rotation = spawn.rotation;
        var ball = PhotonNetwork.Instantiate("Ball", position, rotation);
        _activeBalls.Add(ball.GetComponent<BallModel>());
    }
    
}
