using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Action OnGameStarted = delegate {  };
    public Action OnGameFinished = delegate { };
    
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private TextMeshProUGUI _startMessage;
    [SerializeField] private TimerController _timerController;
    [SerializeField] private int _countDownTime = 3;

    //Start screen messages
    private string _hostReadyMessage = "Press Spacebar to Start";
    private string _playersReadyMessage = "Ready to Start";
    private string _waitingMessage = "Waiting for all players";

    private List<CharacterModel> _characters = new List<CharacterModel>();
    private List<BrickModel> _bricks = new List<BrickModel>();

    private int _activePlayers;
    private bool _win;
    private bool _gameStarted;
    private bool _changeScene;
    private bool _readyToPlay;
    private bool _countingDown;

    public bool Finished { get; private set; }

    private void Start()
    {
         _startScreen.SetActive(true);
        _startMessage.text = _waitingMessage;

        photonView.RPC(nameof(SendEnteredPlayer), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        
        if (!PhotonNetwork.IsMasterClient) return;
        _bricks = FindObjectsOfType<BrickModel>().ToList();

        foreach (var brick in _bricks)
        {
            brick.OnBrickDestroyed += BrickDestroyed;
        }
    }

    [PunRPC]
    private void SendEnteredPlayer(Player player)
    {
        OnPlayerEnteredRoom(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"New player entered: {newPlayer.NickName}");
        
        _activePlayers++;
        _readyToPlay = (_activePlayers + 1) >= PhotonNetwork.CurrentRoom.PlayerCount; //we add one because the host doesn't add itself

        if (!PhotonNetwork.IsMasterClient) return;

        if (_readyToPlay)
        {
            _startMessage.text = _hostReadyMessage;
            photonView.RPC(nameof(UpdateStartScreenMessage), RpcTarget.Others, _playersReadyMessage);
        }

        Debug.Log("Player entered!!");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Leave because the master leaved
        PhotonNetwork.LeaveRoom(false);
        PhotonNetwork.LoadLevel("MainMenu_v2");
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (Input.GetButtonDown("Submit"))
            StartCountDown();
        
    }

    private void StartGame()
    {
        Debug.Log($"Ready to play: {_readyToPlay}");
        if (!_readyToPlay) return;
        if (_gameStarted) return;
        
        _gameStarted = true;
        
        _characters = FindObjectsOfType<CharacterModel>().ToList();
        foreach (var character in _characters)
        {
            SetNewCharacter(character);
        }
        _timerController.StartTimer();
        
        OnGameStarted.Invoke();
        
        photonView.RPC(nameof(ShowStartScreen), RpcTarget.All, false);
    }

    private void StartCountDown()
    {
        if (_countingDown) return;
        _countingDown = true;
        StartCoroutine(TimerCountDown());
    }

    private IEnumerator TimerCountDown()
    {
        for (int i = _countDownTime - 1; i >= 0; i--)
        {
            SendStartTimeMessage((i + 1).ToString());
            yield return new WaitForSeconds(1f);
        }

        StartGame();
    }

    private void SendStartTimeMessage(string message)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _startMessage.text = message;
        photonView.RPC(nameof(UpdateStartScreenMessage), RpcTarget.All, message);
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
        OnGameFinished.Invoke();
        Finished = true;
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
        _win = false;
        SaveScoreData(_win);
    }

    [PunRPC]
    private void ShowWinScreen()
    {
        _win = true;
        SaveScoreData(_win);
    }

    [PunRPC]
    private void ShowStartScreen(bool show)
    {
        _startScreen.SetActive(show);
    }

    [PunRPC]
    private void UpdateStartScreenMessage(string message)
    {
        _startMessage.text = message;
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
