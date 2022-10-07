using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    private const string DEFAULT_ROOM_NAME = "TestRoom";
    private const string DEFAULT_NICK_NAME = "TestUser";
    private const int DEFAULT_MAX_PLAYERS = 4;
    private const int MINIMUM_PLAYERS_FOR_GAME = 2;

    [Header("Main Settings")]
    [SerializeField] private Text txtNickname;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button quitButton;
    [SerializeField] private string statusPrefix = "Status: ";
    public string Level = "Level";

    [Header("All Panels")]
    public Panel loggingPanel;
    public Panel choosePanels;
    public Panel roomSettingPanel;
    public Panel kickedPanel;
    public Panel loadingSymbolPanel;
    public Panel joiningRoomsWaitPanel;
    public Panel chooseRoomPanel;
    public Panel roomLobbyPanel;

    [Header("WaitingToJoinRoom")]
    [SerializeField] private float timeOutSearch = 10f;

    [Header("Logging")]
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private Button logInButton;

    [Header("Choose")]
    [SerializeField] private Button newRoomButton;
    [SerializeField] private Button joinRoomButton;

    [Header("Prompts")]
    [SerializeField] private Button kickedOutConfirmButton;

    private List<Panel> allPanels = new List<Panel>();
    private bool skipEverything; //for cheating the login

    //PROPIERTIES
    public MainMenuView PlayerView { get; private set; }
    public bool Kicked { get; set; }
    public int MaxPlayers { get; private set; }
    public int MinPlayers => MINIMUM_PLAYERS_FOR_GAME;
    public string DefaultRoom => DEFAULT_ROOM_NAME;
    public string DefaultNickname => DEFAULT_NICK_NAME;

    //EVENTS 
    public Action<RoomInfo> OnBannedRoom = delegate { };
    public Action OnClearData = delegate { };
    public Action<string, byte> OnBaseCreateRoom = delegate { };

    public void Awake()
    {
        PlayerView = GetComponent<MainMenuView>();
        MaxPlayers = DEFAULT_MAX_PLAYERS;

        PhotonNetwork.AutomaticallySyncScene = true;

        //GeneratePanels
        GenerateChoosingPanel();
        GenerateWaitJoinningRoomPanel();
        txtNickname.gameObject.SetActive(false);
        quitButton.onClick.AddListener(OnQuitButton);
        logInButton.onClick.AddListener(LogInUser);
        kickedOutConfirmButton.onClick.AddListener(() => { ChangePanel(choosePanels); Kicked = false; });

        //Set all panels
        allPanels.Add(loggingPanel);
        allPanels.Add(choosePanels);
        allPanels.Add(roomSettingPanel);
        allPanels.Add(roomLobbyPanel);
        allPanels.Add(kickedPanel);
        allPanels.Add(loadingSymbolPanel);
        allPanels.Add(joiningRoomsWaitPanel);
        allPanels.Add(chooseRoomPanel);

        RestartMenu();
    }

    public void RestartMenu()
    {
        if (PhotonNetwork.IsConnectedAndReady)
            ChangePanel(choosePanels);
        else
            ChangePanel(loggingPanel);
    }

    #region GeneratingPanels
    private void GenerateChoosingPanel()
    {
        joinRoomButton.onClick.AddListener(() => ChangePanel(chooseRoomPanel));
        newRoomButton.onClick.AddListener(() => ChangePanel(roomSettingPanel));
    }

    private void GenerateWaitJoinningRoomPanel()
    {
        joiningRoomsWaitPanel.OnOpen += OnOpen;
        joiningRoomsWaitPanel.OnClose += OnClose;

        void OnOpen()
        {
            SetStatus("Searching for rooms");
            StartCoroutine(JoinRandomRoomTimer(timeOutSearch));
        }

        void OnClose()
        {
            if(!PhotonNetwork.InRoom)
                SetStatus("No rooms found");
        }
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            QuickMatchCheat();
    }

    private IEnumerator JoinRandomRoomTimer(float timer)
    {
        yield return new WaitForSeconds(timer);

        if (!PhotonNetwork.InRoom)
            ChangePanel(choosePanels);
    }

    private void QuickMatchCheat()
    {
        ChangePanel(loadingSymbolPanel);
        PhotonNetwork.NickName = DEFAULT_NICK_NAME;

        PhotonNetwork.ConnectUsingSettings();

        skipEverything = true;
    }

    public void ChangePanel(Panel panelToOpen)
    {
        for (int i = 0; i < allPanels.Count; i++)
        {
            if (allPanels[i] == panelToOpen)
                allPanels[i].OpenPanel();
            else
                allPanels[i].ClosePanel();
        }
    }

    public void SetStatus(string message)
    {
        statusText.text = statusPrefix + message;
    }

    public void LogInUser()
    {
        if (string.IsNullOrEmpty(nickNameInput.text) || string.IsNullOrWhiteSpace(nickNameInput.text)) return;

        PhotonNetwork.NickName = nickNameInput.text;
        txtNickname.text = nickNameInput.text;

        txtNickname.gameObject.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        ChangePanel(loadingSymbolPanel);
        SetStatus("Trying to Connect");
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        ChangePanel(joiningRoomsWaitPanel);
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        SetStatus("Connecting to Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (skipEverything)
        {
            OnBaseCreateRoom.Invoke("", DEFAULT_MAX_PLAYERS);
            SetStatus("Getting Room");
            return;
        }

        if (!Kicked)
            ChangePanel(choosePanels);

        SetStatus("Connected to Lobby");
    }

    public override void OnCreatedRoom()
    {
        SetStatus("Created Room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetStatus("Created Room failed");
    }

    public override void OnJoinedRoom()
    {
        SetStatus("Joined Room");
        ChangePanel(roomLobbyPanel);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetStatus("Joined Room failed");
        ChangePanel(choosePanels);
    }

    public override void OnLeftRoom()
    {
        if(Kicked)
            ChangePanel(kickedPanel);
        else
            ChangePanel(choosePanels);
    }

    public void KickedPlayer()
    {
        Kicked = true;
        OnBannedRoom.Invoke(PhotonNetwork.CurrentRoom);
        PhotonNetwork.LeaveRoom(false);
    }
    #endregion

    private void OnQuitButton()
    {
        SetStatus("Disconnecting");
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    public void ClearData()
    {
        OnClearData.Invoke();
        skipEverything = false;
        Kicked = false;
    }
}
