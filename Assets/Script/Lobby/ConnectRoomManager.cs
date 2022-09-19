using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectRoomManager : MonoBehaviourPunCallbacks
{
    private const string DEFAULT_ROOM_NAME = "TestRoom";
    private const string DEFAULT_NICK_NAME = "TestUser";
    private const int DEFAULT_MAX_PLAYERS = 4;
    
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField nickNameInput;

    private int _maxPlayers = DEFAULT_MAX_PLAYERS;
    
    private void Start()
    {
        roomNameInput.text = DEFAULT_ROOM_NAME;
        nickNameInput.text = DEFAULT_NICK_NAME;

        connectButton.interactable = false;
        statusText.text = "Connecting to Master";

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connecting to Lobby";
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        connectButton.interactable = true;
        statusText.text = "Connected to Lobby";
    }

    public void Connect()
    {
        if (string.IsNullOrEmpty(roomNameInput.text) || string.IsNullOrWhiteSpace(roomNameInput.text)) return;
        if (string.IsNullOrEmpty(nickNameInput.text) || string.IsNullOrWhiteSpace(nickNameInput.text)) return;

        PhotonNetwork.NickName = nickNameInput.text;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)_maxPlayers;
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, options, TypedLobby.Default);
        connectButton.interactable = false;
    }
    
    public override void OnCreatedRoom()
    {
        statusText.text = "Created Room";
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Created Room failed";
        connectButton.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room";
        PhotonNetwork.LoadLevel("Level"); // Change to RoomMenu?
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Joined Room failed";
        connectButton.interactable = true;
    }
}
