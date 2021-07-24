using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    static NetworkManager _instance;

    [SerializeField]
    string _gameVersion = "1";
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.")]
    [SerializeField]
    private byte _maxPlayersPerRoom = 4;

    bool _isConnecting;


    #region Lobby Variables
    TypedLobby _customLobby = new TypedLobby("customLobby", LobbyType.Default);

    Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
    #endregion


    //#region Room Variables
    //List<Player> _players = new List<Player>();
    //#endregion


    #region Events
    public UnityEvent<List<RoomInfo>> onRoomListRefresh;
    #endregion



    static public NetworkManager Instance
    {
        get
        {
            if (!_instance)
            {
                GameObject gameManagerObj = GameObject.Find("Game Manager");

                if (!gameManagerObj)
                {
                    Debug.LogError("Scene does not possess a game manager!");
                    return null;
                }

                _instance = gameManagerObj.GetComponent<NetworkManager>();

                // Add missing game manager to the game manager object.
                if (!_instance)
                {
                    _instance = gameManagerObj.AddComponent<NetworkManager>();
                }
            }

            return _instance;
        }
    }

    //public List<Player> Players
    //{
    //    get
    //    {
    //        return _players;
    //    }
    //}

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        connect();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void connect()
    {
        //if (PhotonNetwork.IsConnected)
        //{
        //    PhotonNetwork.JoinRandomRoom();
        //}
        //else
        //{
        //    _isConnecting = PhotonNetwork.ConnectUsingSettings();
        //    PhotonNetwork.GameVersion = _gameVersion;
        //}

        if (!PhotonNetwork.IsConnected)
        {
            //_isConnecting = PhotonNetwork.ConnectToRegion("cae");
            _isConnecting = PhotonNetwork.ConnectUsingSettings();   // TO-DO Change fixed region, in Photon Server Settings, to a button of choice.
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }

    public void joinLobby()
    {
        //PhotonNetwork.JoinLobby(_customLobby);
        PhotonNetwork.JoinLobby();
    }

    public void createRoom(string roomName, RoomOptions roomOptions)
    {
        if (!PhotonNetwork.IsConnected)
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void joinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    void updateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int roomIndex = 0; roomIndex < roomList.Count; ++roomIndex)
        {
            RoomInfo info = roomList[roomIndex];

            if (info.RemovedFromList)
            {
                _cachedRoomList.Remove(info.Name);
            }
            else
            {
                _cachedRoomList[info.Name] = info;
            }
        }

        onRoomListRefresh.Invoke(roomList);
    }


    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedMaster() was called by PUN!");

        // Don't do anything if we are not attempting to join a room.
        if (_isConnecting)
        {
            //PhotonNetwork.JoinRandomRoom();

            joinLobby();

            _isConnecting = false;
        }

        Debug.LogError("Region: " + PhotonNetwork.CloudRegion);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}!", cause);

        _cachedRoomList.Clear();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("OnCreateRoomFailed: {0}, with return code {1}", message, returnCode);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby() was called by PUN!");

        _cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate() was called by PUN!");

        updateCachedRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        _cachedRoomList.Clear();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom() was called!");

        _isConnecting = true;

        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            PhotonNetwork.LoadLevel("Main Menu");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinedRoomFailed() was called by PUN. No random room was available, so we'll create one.");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN! Now this client is in a room!");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            // Load the Room level.
            //PhotonNetwork.LoadLevel("Room for 1");

            PhotonNetwork.LoadLevel("Room");
        }
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        _players.Add(newPlayer);
    //    }
    //}

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        _players.Remove(otherPlayer);
    //    }
    //}

    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        int playerIndex = _players.FindIndex((Player currPlayer) => currPlayer == targetPlayer);

    //        _players[playerIndex] = targetPlayer;
    //    }
    //}

    #endregion
}
