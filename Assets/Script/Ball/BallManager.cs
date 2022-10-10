using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
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
    private bool stopSpawning =  false;

    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient) Destroy(this);

        _gameManager.OnGameStarted += Init;
        _gameManager.OnGameFinished += OnGameEnded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            InstantiateBall();
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

        InstantiateBall();
    }

    public void InstantiateBall()
    {
        if (stopSpawning) return;
        var spawn = _ballSpawnPoints[Random.Range(0, _ballSpawnPoints.Count)];
        var position = spawn.position;
        var rotation = spawn.rotation;
        var ball = PhotonNetwork.Instantiate("Ball", position, rotation).GetComponent<BallModel>();
        ball.OnDie += CheckCurrentSituation;
        _activeBalls.Add(ball);
    }

    public void CheckCurrentSituation(BallModel ball)
    {
        ball.OnDie -= CheckCurrentSituation;
        _activeBalls.Remove(ball);

        if (_activeBalls.Count <= 0 && !_gameManager.Finished)
            InstantiateBall();
    }

    public void OnGameEnded()
    {
        stopSpawning = true;

        for (int i = _activeBalls.Count - 1; i >= 0; i--)
        {
            Destroy(_activeBalls[i]);
        }
    }
    
}
