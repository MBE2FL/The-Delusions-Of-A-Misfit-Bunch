using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;



public class RoomManager : MonoBehaviourPunCallbacks
{
    Transform _playersPanel;
    List<UIPlayerPanel> _playerPanels = new List<UIPlayerPanel>();
    UIPlayerPanel _playerPanelPrefab;
    Button _startButton;
    Button _leaveButton;



    public Transform PlayersPanel
    {
        get
        {
            return _playersPanel;
        }
    }

    private void Awake()
    {
        GameObject canvas = GameObject.Find("Canvas");
        Debug.Assert(canvas, "RoomManager: Failed to find the canvas!");

        _playersPanel = canvas.transform.Find("Players Panel");
        Debug.Assert(_playersPanel, "RoomManager: Failed to find Players Panel!");

        Transform leaveTransform = canvas.transform.Find("Leave Button");
        Debug.Assert(leaveTransform, "RoomManager: Failed to find Leave Button's transform!");
        _leaveButton = leaveTransform.GetComponent<Button>();
        Debug.Assert(_leaveButton, "RoomManager: Failed to find Leave Button!");
        _leaveButton.onClick.AddListener(() => 
        {
            PhotonNetwork.LeaveRoom();
        });

        if (PhotonNetwork.IsMasterClient)
        {

            _playerPanelPrefab = Resources.Load<UIPlayerPanel>("Networking/Room/Player Panel");
            Debug.Assert(_playerPanelPrefab, "RoomManager: Failed to load Player Panel prefab!");

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                //UIPlayerPanel playerPanel = Instantiate(_playerPanelPrefab, _playersPanel);
                object[] initData = { player };
                UIPlayerPanel playerPanel = PhotonNetwork.Instantiate("Networking/Room/Player Panel", Vector3.zero, Quaternion.identity, 0, initData).GetComponent<UIPlayerPanel>();
                //playerPanel.transform.SetParent(_playersPanel);

                Debug.Log("Player Panel Created!");

                //playerPanel.init();
                //playerPanel.Player = player;


                _playerPanels.Add(playerPanel);
            }


            Transform startTransform = canvas.transform.Find("Start Button");
            Debug.Assert(startTransform, "RoomManager: Failed to find Start Button's transform!");
            _startButton = startTransform.GetComponent<Button>();
            Debug.Assert(_startButton, "RoomManager: Failed to find Start Button!");
            _startButton.onClick.AddListener(() => PhotonNetwork.LoadLevel("Room for 4"));
            _startButton.interactable = false;
            _startButton.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            bool allReady = true;
            foreach (UIPlayerPanel playerPanel in _playerPanels)
            {
                if (!playerPanel.IsReady)
                {
                    allReady = false;
                    break;
                }
            }

            _startButton.interactable = allReady;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AddCallbackTarget(this);

            Debug.LogError("RoomManager: ADDED CALLBACKS!");
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    base.OnDisable();

        //    PhotonNetwork.RemoveCallbackTarget(this);
        //}

        PhotonNetwork.RemoveCallbackTarget(this);

        Debug.LogError("RoomManager: REMOVED CALLBACKS!");
    }


    #region Photon Callbacks
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("RoomManager: OnJoinedRoom Called.");

    //    //UIPlayerPanel playerPanel = Instantiate(_playerPanelPrefab, _playersPanel);
    //    object[] initData = { PhotonNetwork.LocalPlayer };
    //    GameObject playerPanelObj = PhotonNetwork.Instantiate("Networking/Room/Player Panel", Vector3.zero, Quaternion.identity, 0, initData);
    //    Debug.Assert(playerPanelObj, "Failed to instantiate UI Player Panel Prefab!", this);
    //    UIPlayerPanel playerPanel = playerPanelObj.GetComponent<UIPlayerPanel>();
    //    Debug.Assert(playerPanel, "Failed to get UI Player Panel component!", this);

    //    //playerPanel.init();
    //    //playerPanel.transform.SetParent(_playersPanel);
    //    //playerPanel.Player = newPlayer;

    //    Debug.Log("Player Panel Created!");
    //}

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("RoomManager: OnPlayerEnteredRoom Called. New Player: " + newPlayer.NickName);

            //UIPlayerPanel playerPanel = Instantiate(_playerPanelPrefab, _playersPanel);
            object[] initData = { newPlayer };
            GameObject playerPanelObj = PhotonNetwork.Instantiate("Networking/Room/Player Panel", Vector3.zero, Quaternion.identity, 0, initData);
            Debug.Assert(playerPanelObj, "Failed to instantiate UI Player Panel Prefab!", this);
            UIPlayerPanel playerPanel = playerPanelObj.GetComponent<UIPlayerPanel>();
            Debug.Assert(playerPanel, "Failed to get UI Player Panel component!", this);

            playerPanel.photonView.TransferOwnership(newPlayer);

            //playerPanel.init();
            //playerPanel.transform.SetParent(_playersPanel);
            //playerPanel.Player = newPlayer;

            Debug.Log("Player Panel Created!");

            _playerPanels.Add(playerPanel);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("RoomManager: OnPlayerLeftRoom Called.");

            for (int playerPanelIndex = 0; playerPanelIndex < _playerPanels.Count; ++playerPanelIndex)
            {
                UIPlayerPanel playerPanel = _playerPanels[playerPanelIndex];

                if (playerPanel.Player == otherPlayer)
                {
                    PhotonNetwork.Destroy(playerPanel.photonView);
                    _playerPanels.RemoveAt(playerPanelIndex);
                }
            }
        }
    }
    #endregion
}
