using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _winScreen;
    
    private List<CharacterModel> _characters = new List<CharacterModel>();
    private List<BrickModel> _bricks = new List<BrickModel>();

    private int _activePlayers;
    
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
    }

    private void CharacterDied()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_characters.All(character => character.Dead))
        {
            photonView.RPC(nameof(ShowLoseScreen), RpcTarget.All); // ShowLoseScreen();
        }
    }

    private void BrickDestroyed()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_bricks.All(brick => brick.Destroyed))
        {
            photonView.RPC(nameof(ShowWinScreen), RpcTarget.All);
        }
    }
    
    [PunRPC]
    private void ShowLoseScreen()
    {
        _loseScreen.SetActive(true);
    }

    [PunRPC]
    private void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }

    private void SetNewCharacter(CharacterModel characterModel)
    {
        characterModel.OnDied += CharacterDied;
    }
    
}
