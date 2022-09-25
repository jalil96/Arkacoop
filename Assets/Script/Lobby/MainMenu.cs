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

    [Header("Main Settings")]
    [SerializeField] private bool skipMenu = false;
    //[SerializeField] private int maxPlayers = 4;
    [SerializeField] private Text txtNickname;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string Level = "Level";

    [Header("Logging")]
    [SerializeField] private Panel loggingPanel;
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private Button logInButton;

    [Header("Options")]
    [SerializeField] private Panel optionsPanels;
    [SerializeField] private Button newLobbyOptionsButton;
    [SerializeField] private Button joinLobbyOptionsButton;

    [Header("Room Settings")]
    [SerializeField] private Panel roomSettingPanel;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private Text txtMaxPlayersValue;
    [SerializeField] private Button createLobbyButton;

    [Header("WaitingLobby")]
    [SerializeField] private Panel waitignLobbyPanel;
    [SerializeField] private Text roomNameWaitingLobbyTxt;
    [SerializeField] private GameObject playerListContainer;
    [SerializeField] private PlayerWaitigList playerListPrefab;
    [SerializeField] private GameObject waitingHostSpecialOptions;
    [SerializeField] private GameObject waitingNonHostOptions;

    [Header("Prompts")]
    [SerializeField] private GameObject messagePrompt;
    [SerializeField] private GameObject twoOptionsPrompt;

    private int _maxPlayers = DEFAULT_MAX_PLAYERS;
    private List<Panel> allPanels = new List<Panel>();
    private bool isHost = false;

    public void Awake()
    {
        //Start settings
        txtNickname.gameObject.SetActive(false);

        //Set all panels
        allPanels.Add(loggingPanel);
        allPanels.Add(optionsPanels);
        allPanels.Add(roomSettingPanel);
        allPanels.Add(waitignLobbyPanel);


        waitingHostSpecialOptions.SetActive(false);
        waitingNonHostOptions.SetActive(false);
        ChangePanel(loggingPanel);

        //Setting All Buttons
        logInButton.onClick.AddListener(LogInUser);
        newLobbyOptionsButton.onClick.AddListener(ChangeToCreateARoomPanel);
        joinLobbyOptionsButton.onClick.AddListener(ChangeToCreateARoomPanel);
        createLobbyButton.onClick.AddListener(CreateLobby);
        maxPlayersSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void Start()
    {
        if (skipMenu)
        {
            roomNameInput.text = DEFAULT_ROOM_NAME;
            nickNameInput.text = DEFAULT_NICK_NAME;

            statusText.text = "Connecting to Master";

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
        ChangePanel(optionsPanels);
    }

    public void ChangeToCreateARoomPanel()
    {
        ChangePanel(roomSettingPanel);
    }



    #region WaitingLobby
    public void ChangeToWaitingPanel(bool host = false)
    { 
        ChangePanel(waitignLobbyPanel);
        if (host)
            waitingHostSpecialOptions.SetActive(true);
        else
            waitingNonHostOptions.SetActive(true);
    }

    public void AddPlayerToWaitingList(bool host = false)
    {
        PlayerWaitigList aux = Instantiate(playerListPrefab, playerListContainer.transform);
        //TODO: set player name, player number 
        aux.kickButton.gameObject.SetActive(host);
    }
    #endregion

    #region CreateLobby
    public void CreateLobby()
    {
        print(string.IsNullOrEmpty(roomNameInput.text));
        if (string.IsNullOrEmpty(roomNameInput.text) || string.IsNullOrWhiteSpace(roomNameInput.text)) return;

        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte) maxPlayersSlider.value;
        options.IsOpen = true;
        options.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, options, TypedLobby.Default);
        isHost = true;
    }

    public void ValueChangeCheck()
    {
        txtMaxPlayersValue.text = maxPlayersSlider.value.ToString();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connecting to Lobby";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        ChangeToWaitingPanel(isHost);
        createLobbyButton.interactable = true;
        statusText.text = "Connected to Lobby";
    }

    public override void OnCreatedRoom()
    {
        ChangeToWaitingPanel(isHost);
        statusText.text = "Created Room";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Created Room failed";
        createLobbyButton.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room";
        PhotonNetwork.LoadLevel(Level);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Joined Room failed";
        createLobbyButton.interactable = true;
    }
    #endregion
}
