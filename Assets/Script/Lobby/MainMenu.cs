using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
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
    [SerializeField] private Panel loggingPanel;
    [SerializeField] private Panel choosePanels;
    [SerializeField] private Panel roomSettingPanel;
    [SerializeField] private Panel waitignLobbyPanel;
    [SerializeField] private Panel kickedPanel;
    [SerializeField] private Panel loadingSymbolPanel;
    [SerializeField] private Panel joiningRoomsWaitPanel;

    [Header("WaitingToJoinRoom")]
    [SerializeField] private float timeOutSearch = 10f;

    [Header("Logging")]
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private Button logInButton;

    [Header("Choose")]
    [SerializeField] private Button newRoomButton;
    [SerializeField] private Button joinRoomButton;

    [Header("Room Settings")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Text txtMaxPlayersValue;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button goBackToChooseButton;

    [Header("WaitingL Room")]
    [SerializeField] private string roomPrefix = "Room: ";
    [SerializeField] private PlayerWaitigList playerListPrefab;
    [SerializeField] private Text roomNameWaitingLobbyTxt;
    [SerializeField] private GameObject playerListContainer;
    [SerializeField] private GameObject waitingHostSpecialOptions;
    [SerializeField] private GameObject waitingNonHostOptions;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private Text currentNumberPlayersTxt;

    [Header("Prompts")]
    [SerializeField] private Button kickedOutConfirmButton;

    private int _maxPlayers;
    private List<Panel> allPanels = new List<Panel>();
    private List<PlayerWaitigList> playerButtons = new List<PlayerWaitigList>();
    private Dictionary<string, int> playerNames = new Dictionary<string, int>(); 

    private Player[] currentPlayerList;
    private MenuPlayerView playerView;

    private bool wasKicked;
    private bool skipEverything; //for cheating the login
    private bool searchingRandomRoom;
    private float timeOutSearchTimer;

    public void Awake()
    {
        _maxPlayers = DEFAULT_MAX_PLAYERS;
        playerView = GetComponent<MenuPlayerView>();
        txtNickname.gameObject.SetActive(false);
        quitButton.onClick.AddListener(OnQuitButton);

        //GeneratePanels
        GenerateWaitingPanel();
        GenerateChoosingPanel();
        GenerateCreateRoomPanel();
        GenerateWaitJoinningRoomPanel();
        logInButton.onClick.AddListener(LogInUser);
        kickedOutConfirmButton.onClick.AddListener(() => { ChangePanel(choosePanels); wasKicked = false; });

        //Set all panels
        allPanels.Add(loggingPanel);
        allPanels.Add(choosePanels);
        allPanels.Add(roomSettingPanel);
        allPanels.Add(waitignLobbyPanel);
        allPanels.Add(kickedPanel);
        allPanels.Add(loadingSymbolPanel);
        allPanels.Add(joiningRoomsWaitPanel);

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
    private void GenerateWaitingPanel()
    {
        waitignLobbyPanel.OnOpen += OnOpen;

        leaveRoomButton.onClick.AddListener(LeaveTheRoom);

        waitingHostSpecialOptions.SetActive(false);
        waitingNonHostOptions.SetActive(false);

        //Let´s create the max of buttons needed in the game
        playerListPrefab.gameObject.SetActive(false);
        for (int i = 0; i < _maxPlayers; i++)
        {
            PlayerWaitigList aux = Instantiate(playerListPrefab, playerListContainer.transform);
            aux.gameObject.SetActive(false);
            playerButtons.Add(aux);
        }


        void OnOpen()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                waitingHostSpecialOptions.SetActive(true);
                waitingNonHostOptions.SetActive(false);
                startGameButton.onClick.AddListener(StartGame);
                startGameButton.interactable = false;
            }
            else
            {
                waitingHostSpecialOptions.SetActive(false);
                waitingNonHostOptions.SetActive(true);
                startGameButton.gameObject.SetActive(false);
            }

            roomNameWaitingLobbyTxt.text = roomPrefix + PhotonNetwork.CurrentRoom.Name;
            RefreshPlayerList();
        }
    }

    private void GenerateCreateRoomPanel()
    {
        maxPlayersSlider.maxValue = DEFAULT_MAX_PLAYERS;
        maxPlayersSlider.minValue = MINIMUM_PLAYERS_FOR_GAME;

        goBackToChooseButton.onClick.AddListener(() => ChangePanel(choosePanels));
        maxPlayersSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        createRoomButton.onClick.AddListener(CreateRoom);
    }

    private void GenerateChoosingPanel()
    {
        joinRoomButton.onClick.AddListener(JoinRoom);
        newRoomButton.onClick.AddListener(() => ChangePanel(roomSettingPanel));
    }

    private void GenerateWaitJoinningRoomPanel()
    {
        joiningRoomsWaitPanel.OnOpen += OnOpen;
        joiningRoomsWaitPanel.OnClose += OnClose;

        void OnOpen()
        {
            SetStatus("Searching for rooms");

            searchingRandomRoom = true;
            timeOutSearchTimer = timeOutSearch;
        }

        void OnClose()
        {
            SetStatus("No rooms found");

            searchingRandomRoom = false;
            timeOutSearchTimer = 0f;
        }
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            QuickMatchCheat();

        if (joiningRoomsWaitPanel.IsOpen)
        {
            if (searchingRandomRoom)
            {
                timeOutSearchTimer -= Time.deltaTime;
                if (timeOutSearchTimer <= 0)
                {
                    searchingRandomRoom = false;
                    if (PhotonNetwork.InRoom) return;
                    ChangePanel(choosePanels);
                }
            }
        }
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

    private void SetStatus(string message)
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


    #region Choose & Create
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text) || string.IsNullOrWhiteSpace(roomNameInput.text)) return;

        BaseCreateRoom(roomNameInput.text, (byte)maxPlayersSlider.value);
        ChangePanel(loadingSymbolPanel);
    }

    private void BaseCreateRoom(string roomName = "", byte maxPlayers = DEFAULT_MAX_PLAYERS)
    {
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
            roomName = DEFAULT_ROOM_NAME;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = maxPlayers;
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }


    public void ValueChangeCheck()
    {
        txtMaxPlayersValue.text = maxPlayersSlider.value.ToString();
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        ChangePanel(joiningRoomsWaitPanel);
    }

    public void CheckNicknames(Player[] players)
    {
        playerNames.Clear();
        for (int i = 0; i < players.Length; i++)
        {
            if (playerNames.TryGetValue(players[i].NickName, out int number))
            {
                players[i].NickName = $"{players[i].NickName}({number})";
                number++;
                playerNames[players[i].NickName] = number;
            }
            else
                playerNames.Add(players[i].NickName, 1);
        }
    }

    #endregion

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
            BaseCreateRoom();
            SetStatus("Getting Room");
            return;
        }

        if (!wasKicked)
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
        searchingRandomRoom = false;
        timeOutSearchTimer = 0f;
        SetStatus("Joined Room");
        ChangePanel(waitignLobbyPanel);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetStatus("Joined Room failed");
        ChangePanel(choosePanels);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnLeftRoom()
    {
        if(wasKicked)
            ChangePanel(kickedPanel);
        else
            ChangePanel(choosePanels);
    }
    #endregion

    public void RefreshPlayerList()
    {
        currentNumberPlayersTxt.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        currentPlayerList = PhotonNetwork.PlayerList;

        CheckNicknames(currentPlayerList);

        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = HasEnoughPlayers();

        for (int i = 0; i < playerButtons.Count; i++)
        {
            bool overflow = (currentPlayerList.Length - 1) < i;

            playerButtons[i].gameObject.SetActive(!overflow);

            if (overflow) continue;

            Player currentPlayer = currentPlayerList[i];

            playerButtons[i].gameObject.SetActive(true);
            playerButtons[i].nameTxt.text = currentPlayer.NickName;
            playerButtons[i].numberTxt.text = $"{i + 1} - ";

            if (PhotonNetwork.IsMasterClient && !currentPlayer.IsMasterClient)
            {
                playerButtons[i].kickButton.gameObject.SetActive(true);
                if (PhotonNetwork.IsMasterClient)
                    playerButtons[i].kickButton.onClick.AddListener(() => OnKickPlayer(currentPlayer));
            }
            else
            {
                playerButtons[i].kickButton.gameObject.SetActive(false);
            }
        }
    }

    private bool HasEnoughPlayers()
    {
        bool hasEnough = PhotonNetwork.CurrentRoom.PlayerCount >= MINIMUM_PLAYERS_FOR_GAME;

        if(hasEnough)
            SetStatus("Ready to start");
        else
            SetStatus("Waiting for more players");
        return hasEnough;
    }

    private void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.AutomaticallySyncScene = true;

        currentPlayerList = PhotonNetwork.PlayerList;

        for (int i = currentPlayerList.Length - 1; i >= 0; i--)
        {
            if (!currentPlayerList[i].IsMasterClient)
                LoadScene(currentPlayerList[i]);
        }

        PhotonNetwork.LoadLevel(Level);
    }

    private void LoadScene(Player newPlayer)
    {
        playerView.OnLoadScene(newPlayer);
    }

    private void OnKickPlayer(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerView.OnKickPlayer(newPlayer);
        SetStatus("Being kicked");
    }

    public void KickedPlayer()
    {
        wasKicked = true;
        PhotonNetwork.LeaveRoom(false);
    }

    private void LeaveTheRoom()
    {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.IsMasterClient)
        {
            currentPlayerList = PhotonNetwork.PlayerList;

            for (int i = currentPlayerList.Length - 1; i >= 0; i--)
            {
                if (!currentPlayerList[i].IsMasterClient)
                    OnKickPlayer(currentPlayerList[i]);
            }
        }

        PhotonNetwork.LeaveRoom(false);
        ChangePanel(choosePanels);
    }

    private void OnQuitButton()
    {
        if (PhotonNetwork.InRoom)
            LeaveTheRoom();

        SetStatus("Disconnecting");
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    private void ClearData()
    {
        playerNames.Clear();
        skipEverything = false;
        wasKicked = false;
        timeOutSearchTimer = 0f;
        searchingRandomRoom = false;

    }

}
