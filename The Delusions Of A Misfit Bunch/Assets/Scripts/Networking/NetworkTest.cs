using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;



public class NetworkTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    string _gameVersion = "1";
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.")]
    [SerializeField]
    private byte _maxPlayersPerRoom = 4;

    bool _isConnecting;

    [Tooltip("The UI Panel to let the user enter their name, connect and play.")]
    [SerializeField]
    GameObject _controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress.")]
    [SerializeField]
    GameObject _progressLabel;



    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _controlPanel.SetActive(true);
        _progressLabel.SetActive(false);
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
        _controlPanel.SetActive(false);
        _progressLabel.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            _isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = _gameVersion;
        }
    }


    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedMaster() was called by PUN!");

        // Don't do anything if we are not attempting to join a room.
        if (_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}!", cause);

        _controlPanel.SetActive(true);
        _progressLabel.SetActive(false);
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
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }

    #endregion

}
