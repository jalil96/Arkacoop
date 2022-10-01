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
    //[SerializeField] private int maxPlayers = 4;
    [SerializeField] private Text txtNickname;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string Level = "Level";

    [Header("Logging")]
    [SerializeField] private Panel loggingPanel;
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private Button logInButton;
    [SerializeField] private Panel loadingSymbolPanel;

    [Header("Choose")]
    [SerializeField] private Panel choosePanels;
    [SerializeField] private Button newRoomButton;
    [SerializeField] private Button joinRoomButton;

    [Header("Room Settings")]
    [SerializeField] private Panel roomSettingPanel;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Text txtMaxPlayersValue;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button goBackToChooseButton;

    [Header("WaitingL Room")]
    [SerializeField] private string roomPrefix = "Room: ";
    [SerializeField] private Panel waitignLobbyPanel;
    [SerializeField] private Text roomNameWaitingLobbyTxt;
    [SerializeField] private GameObject playerListContainer;
    [SerializeField] private PlayerWaitigList playerListPrefab;
    [SerializeField] private GameObject waitingHostSpecialOptions;
    [SerializeField] private GameObject waitingNonHostOptions;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private Text currentNumberPlayersTxt;

    [Header("Prompts")]
    [SerializeField] private Panel kickedPanel;
    [SerializeField] private Button kickedOutConfirmButton;

    private int _maxPlayers;
    private List<Panel> allPanels = new List<Panel>();
    private List<PlayerWaitigList> playerButtons = new List<PlayerWaitigList>();

    private Player[] currentPlayerList;


    public void Awake()
    {
        _maxPlayers = DEFAULT_MAX_PLAYERS;

        //Start settings
        txtNickname.gameObject.SetActive(false);
        
        //GeneratePanels
        GenerateWaitingPanel();
        GenerateChoosingPanel();
        GenerateCreateRoomPanel();
        logInButton.onClick.AddListener(LogInUser);
        kickedOutConfirmButton.onClick.AddListener(() => ChangePanel(choosePanels));

        //Set all panels
        allPanels.Add(loggingPanel);
        allPanels.Add(choosePanels);
        allPanels.Add(roomSettingPanel);
        allPanels.Add(waitignLobbyPanel);
        allPanels.Add(kickedPanel);
        allPanels.Add(loadingSymbolPanel);

        //if (PhotonNetwork.NickName != null)
        //    ChangePanel(choosePanels);
        //else
            ChangePanel(loggingPanel);
    }

    #region GeneratingPanels
    public void GenerateWaitingPanel()
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

    public void GenerateCreateRoomPanel()
    {
        goBackToChooseButton.onClick.AddListener(() => ChangePanel(choosePanels));
        maxPlayersSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        createRoomButton.onClick.AddListener(CreateRoom);
    }

    public void GenerateChoosingPanel()
    {
        joinRoomButton.onClick.AddListener(JoinRoom);
        newRoomButton.onClick.AddListener(() => ChangePanel(roomSettingPanel));
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            roomNameInput.text = DEFAULT_ROOM_NAME;
            nickNameInput.text = DEFAULT_NICK_NAME;

            statusText.text = "Connecting to Master";
            PhotonNetwork.ConnectUsingSettings();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            nickNameInput.text = DEFAULT_NICK_NAME;
            PhotonNetwork.ConnectUsingSettings();
        }
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

    public void LogInUser()
    {
        if (string.IsNullOrEmpty(nickNameInput.text) || string.IsNullOrWhiteSpace(nickNameInput.text)) return;


        PhotonNetwork.NickName = nickNameInput.text;
        txtNickname.text = nickNameInput.text;

        txtNickname.gameObject.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        ChangePanel(loadingSymbolPanel);
        statusText.text = "Status: Trying to Connect";
    }


    #region Choose & Create
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text) || string.IsNullOrWhiteSpace(roomNameInput.text)) return;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)maxPlayersSlider.value;
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, options, TypedLobby.Default);
        ChangePanel(loadingSymbolPanel);
    }

    public void ValueChangeCheck()
    {
        txtMaxPlayersValue.text = maxPlayersSlider.value.ToString();
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        ChangePanel(loadingSymbolPanel);
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        statusText.text = "Connecting to Lobby";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        ChangePanel(choosePanels);
        statusText.text = "Connected to Lobby";
    }

    public override void OnCreatedRoom()
    {
        ChangePanel(waitignLobbyPanel);
        statusText.text = "Created Room";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Created Room failed";
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room";
        ChangePanel(waitignLobbyPanel);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Joined Room failed";
        ChangePanel(choosePanels);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        print("otherPlayer left the room");
        print(otherPlayer.NickName);
        RefreshPlayerList();
    }

    public override void OnLeftRoom()
    {
        ChangePanel(choosePanels);
    }
    #endregion

    public void RefreshPlayerList()
    {
        currentNumberPlayersTxt.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        currentPlayerList = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerButtons.Count; i++)
        {
            bool overflow = (currentPlayerList.Length - 1) < i;

            playerButtons[i].gameObject.SetActive(!overflow);

            if (overflow) return;

            Player currentPlayer = currentPlayerList[i];

            playerButtons[i].gameObject.SetActive(true);
            playerButtons[i].nameTxt.text = currentPlayer.NickName;
            playerButtons[i].numberTxt.text = $"{currentPlayer.ActorNumber} - ";

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

        startGameButton.interactable = HasEnoughPlayers();
    }

    private bool HasEnoughPlayers()
    {
        print("Has enough players? " + (PhotonNetwork.CurrentRoom.PlayerCount >= MINIMUM_PLAYERS_FOR_GAME));
        return PhotonNetwork.CurrentRoom.PlayerCount >= MINIMUM_PLAYERS_FOR_GAME;
    }

    private void StartGame()
    {
        PhotonNetwork.LoadLevel(Level);
    }

    private void OnKickPlayer(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("KickPlayer", newPlayer);
        startGameButton.interactable = HasEnoughPlayers();
    }

    [PunRPC]
    private void KickPlayer()
    {
        PhotonNetwork.LeaveRoom(false);
        ChangePanel(kickedPanel);
    }

    private void LeaveTheRoom()
    {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.IsMasterClient)
        {
            currentPlayerList = PhotonNetwork.PlayerList;

            for (int i = currentPlayerList.Length - 1; i >= 0; i--)
            {
                if(!currentPlayerList[i].IsMasterClient)
                    OnKickPlayer(currentPlayerList[i]);
            }
        }

        PhotonNetwork.LeaveRoom(false);
        print(PhotonNetwork.NickName + " has left the room");
    }

}
