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
    [SerializeField] private TimerController _timerController; 
    
    private List<CharacterModel> _characters = new List<CharacterModel>();
    private List<BrickModel> _bricks = new List<BrickModel>();

    private int _activePlayers;
    private bool _win;
    
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        _bricks = FindObjectsOfType<BrickModel>().ToList();

        foreach (var brick in _bricks)
        {
            brick.OnBrickDestroyed += BrickDestroyed;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
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
            StartGame();
        }
    }

    private void StartGame()
    {
        var obj = PhotonNetwork.Instantiate("Ball", Vector3.zero, Quaternion.identity);
        var ball = obj.GetComponent<BallModel>();

        if (ball != null)
        {
            // Hacer algo?
        }

        _characters = FindObjectsOfType<CharacterModel>().ToList();
        foreach (var character in _characters)
        {
            SetNewCharacter(character);
        }
        _activePlayers = _characters.Count();
        
        _timerController.StartTimer();
    }

    private void CharacterDied()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_characters.All(character => character.Dead))
        {
            photonView.RPC(nameof(ShowLoseScreen), RpcTarget.All); // ShowLoseScreen();
            _timerController.StopTimer();
        }
    }

    private void BrickDestroyed()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_bricks.All(brick => brick.Destroyed))
        {
            photonView.RPC(nameof(ShowWinScreen), RpcTarget.All);
            _timerController.StopTimer();
        }
    }
    
    [PunRPC]
    private void ShowLoseScreen()
    {
        _loseScreen.SetActive(true);
        _win = false;
        Invoke(nameof(ChangeScene), 2f);
    }

    [PunRPC]
    private void ShowWinScreen()
    {
        _winScreen.SetActive(true);
        _win = true;
        Invoke(nameof(ChangeScene), 2f);
    }

    private void ChangeScene()
    {
        SaveScoreData(_win);
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
