using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;



public class ServerBrowser : MonoBehaviour
{
    NetworkManager _networkManager;

    #region Room Browser Variables
    [SerializeField]
    GameObject _roomPanelPrefab;
    RectTransform roomScrollContent;
    //Button _refreshButton;
    //Button _backButton;
    Dictionary<string, GameObject> _roomPanels = new Dictionary<string, GameObject>();
    #endregion

    #region Create Room Variables
    InputField _roomNameInputField;
    Toggle _isPublicToggle;
    Toggle _isVisibleToggle;
    Slider _maxPlayersSlider;
    Button _roomCreateButton;
    #endregion



    private void Awake()
    {
        _roomPanelPrefab = Resources.Load<GameObject>("Networking/Server Browser/Room Panel");

        // Find the canvas.
        GameObject canvas = GameObject.Find("Server Browser Canvas");
        Debug.Assert(canvas, "ServerBrowser: Failed to find canvas!");
        canvas.SetActive(false);

        ScrollRect roomScrollRect = canvas.transform.Find("Room Scroll View").GetComponent<ScrollRect>();
        Debug.Assert(roomScrollRect, "ServerBrowser: Failed to find Room Scroll View's rect!");

        roomScrollContent = roomScrollRect.content;



        Transform mainPanel = canvas.transform.Find("Panel");
        Debug.Assert(mainPanel, "ServerBrowser: Failed to find Panel's transform!");

        //Transform tempTransform = mainPanel.Find("Back Button");
        //Debug.Assert(tempTransform, "ServerBrowser: Failed to find Back Button's transform!");
        //_backButton = tempTransform.GetComponent<Button>();
        //Debug.Assert(_backButton, "ServerBrowser: Failed to find Back Button!");
        //_backButton.onClick.AddListener(() => {  })




        Transform roomOptionsPanel = canvas.transform.Find("Room Options Panel");
        Debug.Assert(roomOptionsPanel, "ServerBrowser: Failed to find Room Options Panel's transform!");

        Transform tempTransform = roomOptionsPanel.Find("Name InputField");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Name InputField's transform!");
        _roomNameInputField = tempTransform.GetComponent<InputField>();
        Debug.Assert(_roomNameInputField, "ServerBrowser: Failed to find Name InputField!");

        tempTransform = roomOptionsPanel.Find("Is Public Toggle");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Is Public Toggle's transform!");
        _isPublicToggle = tempTransform.GetComponent<Toggle>();
        Debug.Assert(_isPublicToggle, "ServerBrowser: Failed to find Is Public Toggle!");

        tempTransform = roomOptionsPanel.Find("Is Visible Toggle");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Is Visible Toggle's transform!");
        _isVisibleToggle = tempTransform.GetComponent<Toggle>();
        Debug.Assert(_isVisibleToggle, "ServerBrowser: Failed to find Is Visible Toggle!");

        tempTransform = roomOptionsPanel.Find("Max Players Slider");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Max Players Slider's transform!");
        _maxPlayersSlider = tempTransform.GetComponent<Slider>();
        Debug.Assert(_maxPlayersSlider, "ServerBrowser: Failed to find Max Players Slider!");

        tempTransform = tempTransform.Find("Text");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Max Players Slider's text's transform!");
        Text _maxPlayersSliderText = tempTransform.GetComponent<Text>();
        Debug.Assert(_maxPlayersSliderText, "ServerBrowser: Failed to find Max Players Slider's Text!");
        _maxPlayersSliderText.text = "Max Players: " + (int)_maxPlayersSlider.value;
        _maxPlayersSlider.onValueChanged.AddListener((float value) => _maxPlayersSliderText.text = "Max Players: " + (int)value);

        tempTransform = roomOptionsPanel.Find("Create Button");
        Debug.Assert(tempTransform, "ServerBrowser: Failed to find Create Button's transform!");
        _roomCreateButton = tempTransform.GetComponent<Button>();
        Debug.Assert(_roomCreateButton, "ServerBrowser: Failed to find Create Button!");
        _roomCreateButton.onClick.AddListener(createRoom);
    }

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = NetworkManager.Instance;

        _networkManager.onRoomListRefresh.AddListener(onRoomListRefresh);
    }

    private void OnDestroy()
    {
        //NetworkManager networkManager = NetworkManager.Instance;

        _networkManager.onRoomListRefresh.RemoveListener(onRoomListRefresh);

        _roomCreateButton.onClick.RemoveListener(createRoom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onRoomListRefresh(List<RoomInfo> roomList)
    {
        Debug.LogError("Server Browser: Room List Updated!");


        //for (int roomPanelIndex = 0; roomPanelIndex < _roomPanels.Count; ++roomPanelIndex)
        //{
        //    Destroy(_roomPanels[roomPanelIndex]);
        //}
        //_roomPanels.Clear();

        //foreach (GameObject roomPanel in _roomPanels)
        //{
        //    Destroy(roomPanel);
        //}
        //_roomPanels.Clear();


        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                GameObject roomPanel;
                if (_roomPanels.TryGetValue(info.Name, out roomPanel))
                {
                    Destroy(roomPanel);
                    _roomPanels.Remove(info.Name);
                }
            }
            else
            {
                if (_roomPanels.ContainsKey(info.Name))
                {
                    continue;
                }

                GameObject roomPanel = Instantiate(_roomPanelPrefab, roomScrollContent);

                roomPanel.transform.Find("Name").GetComponent<Text>().text = info.Name;
                roomPanel.transform.Find("Players").GetComponent<Text>().text = "Players: " + info.PlayerCount + "/" + info.MaxPlayers;
                Button joinButton = roomPanel.transform.Find("Join Button").GetComponent<Button>();
                joinButton.interactable = info.PlayerCount <= info.MaxPlayers;
                string roomName = info.Name;
                joinButton.onClick.AddListener(() => _networkManager.joinRoom(roomName));

                _roomPanels.Add(info.Name, roomPanel);
            }
        }
    }

    //public void display(bool shouldDisplay)
    //{
    //    if (shouldDisplay)
    //    {
    //        gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    public void createRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = _isPublicToggle.isOn;
        roomOptions.IsVisible = _isVisibleToggle.isOn;
        roomOptions.MaxPlayers = (byte)_maxPlayersSlider.value;

        _networkManager.createRoom(_roomNameInputField.text, roomOptions);
    }
}
