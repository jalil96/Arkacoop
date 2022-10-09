using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private TimerController _timerController;

    private List<CharacterModel> _characters = new List<CharacterModel>();
    private List<BrickModel> _bricks = new List<BrickModel>();

    private int _activePlayers;
    private bool _win;
    private bool _gameStarted;
    private bool _changeScene;
    private bool _readyToPlay;

    private void Start()
    {
        // _startScreen.SetActive(false);
        if (!PhotonNetwork.IsMasterClient) return;
        OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
        _bricks = FindObjectsOfType<BrickModel>().ToList();

        foreach (var brick in _bricks)
        {
            brick.OnBrickDestroyed += BrickDestroyed;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _activePlayers++;
        _readyToPlay = _activePlayers >= PhotonNetwork.CurrentRoom.PlayerCount;
        if (_readyToPlay)
        {
            photonView.RPC(nameof(ShowStartScreen), RpcTarget.All, true);
        }
        Debug.Log("Player entered!!");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Leave because the master leaved
        PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.LoadLevel("MainMenu");
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Starting game!");
            StartGame();
            Debug.Log("Game started!");
        }
    }

    private void StartGame()
    {
        Debug.Log($"Ready to player: {_readyToPlay}");
        if (!_readyToPlay) return;
        var obj = PhotonNetwork.Instantiate("Ball", Vector3.zero, Quaternion.identity);
        var ball = obj.GetComponent<BallModel>();

        if (ball != null)
        {
            // Hacer algo?
        }

        if (_gameStarted) return;
        
        _gameStarted = true;
        
        _characters = FindObjectsOfType<CharacterModel>().ToList();
        foreach (var character in _characters)
        {
            SetNewCharacter(character);
        }
        _timerController.StartTimer();
        
        photonView.RPC(nameof(ShowStartScreen), RpcTarget.All, false);
    }

    private void CharacterDied()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_characters.All(character => character.Dead))
        {
            EndGame(false);
        }
    }

    private void BrickDestroyed()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_bricks.All(brick => brick.Destroyed))
        {
            EndGame(true);
        }
    }

    private void EndGame(bool win)
    {
        Debug.Log($"We win? {win}");
        photonView.RPC(win ? nameof(ShowWinScreen) : nameof(ShowLoseScreen), RpcTarget.All);
        _timerController.StopTimer();
        var balls = FindObjectsOfType<BallModel>();
        var bricks = FindObjectsOfType<BrickModel>();
        foreach (var ball in balls)
        {
            ball.gameObject.SetActive(false);
        }
        foreach (var brick in bricks)
        {
            brick.gameObject.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient && !_changeScene)
        {
            Debug.Log("Calling ChangeScene");
            _changeScene = true;
            Invoke(nameof(ChangeScene), 2f);
        }
    }
    
    [PunRPC]
    private void ShowLoseScreen()
    {
        _loseScreen.SetActive(true);
        _win = false;
        SaveScoreData(_win);
    }

    [PunRPC]
    private void ShowWinScreen()
    {
        _winScreen.SetActive(true);
        _win = true;
        SaveScoreData(_win);
    }

    [PunRPC]
    private void ShowStartScreen(bool show)
    {
        _startScreen.SetActive(show);
    }

    private void ChangeScene()
    {
        PhotonNetwork.LoadLevel("ScoreScreen");
    }

    private void SaveScoreData(bool win)
    {
        var playersData = new List<PlayerData>();
        var players = PhotonNetwork.PlayerList;
        
        foreach (var player in players)
        {
            playersData.Add(new PlayerData(player.GetScore(), player.NickName));
            Debug.Log($"Saving {player.NickName} score: {player.GetScore()}");
        }

        PersistScoreData.Instance.win = win;
        PersistScoreData.Instance.playersData = playersData;
    }

    private void SetNewCharacter(CharacterModel characterModel)
    {
        characterModel.OnDied += CharacterDied;
    }
    
}
