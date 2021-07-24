using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class UIPlayerPanel : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange
{
    InputField _nameInputField;
    Button _readyButton;
    Button _kickButton;
    bool _isReady = false;
    Player _player;

    RoomManager _roomManager;



    public Player Player
    {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
            _nameInputField.text = _player.NickName;
        }
    }

    public bool IsReady
    {
        get
        {
            return _isReady;
        }
    }

    private void Awake()
    {
        // Find the name input field.
        Transform tempTransform = transform.Find("Name InputField");
        Debug.Assert(tempTransform, "UIPlayerPanel: Failed to find Name InputField's transform!");
        _nameInputField = tempTransform.GetComponent<InputField>();
        Debug.Assert(_nameInputField, "UIPlayerPanel: Failed to find Name InputField!");

        // Find the Buttons transform.
        tempTransform = transform.Find("Buttons");
        Debug.Assert(tempTransform, "UIPlayerPanel: Failed to find Button's transform!");

        // Find the ready button.
        Transform tempChildTransform = tempTransform.Find("Ready Button");
        Debug.Assert(tempChildTransform, "UIPlayerPanel: Failed to find Ready Button's transform!");
        _readyButton = tempChildTransform.GetComponent<Button>();
        Debug.Assert(_readyButton, "UIPlayerPanel: Failed to find Ready Button!");

        // Find the room manager.
        GameObject roomManagerObj = GameObject.Find("Room Manager");
        Debug.Assert(roomManagerObj, "UIPlayerPanel: Failed to find Room Manager's game object!");
        _roomManager = roomManagerObj.GetComponent<RoomManager>();
        Debug.Assert(_roomManager, "UIPlayerPanel: Failed to find Room Manager!");
    }

    // Start is called before the first frame update
    void Start()
    {
        //init();
    }

    public void init()
    {
        ColorBlock colours = _readyButton.colors;
        colours.normalColor = Color.red;
        colours.highlightedColor = colours.normalColor * 0.96f;
        colours.pressedColor = colours.normalColor * 0.78f;
        colours.selectedColor = colours.normalColor * 0.96f;
        Color disabledColour = colours.disabledColor;
        disabledColour = colours.normalColor * 0.78f;
        disabledColour.a = 0.5f;
        colours.disabledColor = disabledColour;
        _readyButton.colors = colours;

        //if (photonView.IsMine)
        if (_player == PhotonNetwork.LocalPlayer)
        {
            _readyButton.interactable = true;

            _readyButton.onClick.AddListener(() =>
            {
                _isReady = !_isReady;

                Color colour = _isReady ? Color.green : Color.red;
                colours = _readyButton.colors;
                colours.normalColor = colour;
                colours.highlightedColor = colour * 0.96f;
                colours.pressedColor = colour * 0.78f;
                colours.selectedColor = colour * 0.96f;
                Color disabledColour = colours.disabledColor;
                disabledColour = colour * 0.78f;
                disabledColour.a = 0.5f;
                colours.disabledColor = disabledColour;
                _readyButton.colors = colours;
            });
        }
        else
        {
            _readyButton.interactable = false;

            // Find the kick button.
            if (PhotonNetwork.IsMasterClient)
            {
                // Find the Buttons transform.
                Transform tempTransform = transform.Find("Buttons");
                Debug.Assert(tempTransform, "UIPlayerPanel: Failed to find Button's transform!");

                Transform tempChildTransform = tempTransform.Find("Kick Button");
                Debug.Assert(tempChildTransform, "UIPlayerPanel: Failed to find Kick Button's transform!");
                _kickButton = tempChildTransform.GetComponent<Button>();
                Debug.Assert(_kickButton, "UIPlayerPanel: Failed to find Kick Button!");
                _kickButton.onClick.AddListener(() => PhotonNetwork.CloseConnection(_player));
                _kickButton.gameObject.SetActive(true);
            }
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data.

            //stream.SendNext(_readyButton.colors.normalColor);
            Color colour = _readyButton.colors.normalColor;
            Vector3 colourVector = new Vector3(colour.r, colour.g, colour.b);
            stream.SendNext(colourVector);
            stream.SendNext(_isReady);
        }
        else
        {
            // Network player, recieve data.

            ColorBlock colours = _readyButton.colors;
            //colours.normalColor = (Color)stream.ReceiveNext();
            Vector3 colourVector = (Vector3)stream.ReceiveNext();
            colours.normalColor = new Color(colourVector.x, colourVector.y, colourVector.z, 1.0f);
            colours.highlightedColor = colours.normalColor * 0.96f;
            colours.pressedColor = colours.normalColor * 0.78f;
            colours.selectedColor = colours.normalColor * 0.96f;
            Color disabledColour = colours.disabledColor;
            disabledColour = colours.normalColor * 0.78f;
            disabledColour.a = 0.5f;
            colours.disabledColor = disabledColour;
            _readyButton.colors = colours;

            _isReady = (bool)stream.ReceiveNext();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //GameObject canvas = GameObject.Find("Canvas");
        //Debug.Assert(canvas, "UIPlayerPanel: Failed to find the canvas!");

        //Transform playersPanel = canvas.transform.Find("Players Panel");
        //Debug.Assert(playersPanel, "UIPlayerPanel: Failed to find Players Panel!");

        transform.SetParent(_roomManager.PlayersPanel);
        object[] initData = info.photonView.InstantiationData;

        Debug.Assert(initData.Length > 0, "Instantiation data was invalid!", this);

        Player player = initData[0] as Player;

        Debug.Assert(player != null, "A Player was not the first piece of instantiation data!", this);

        Player = player;

        init();
        //Debug.LogError("Player Name Is " + _player.NickName);
    }

    public void OnOwnerChange(Player newOwner, Player previousOwner)
    {
        Debug.LogError("Hey There");

        //_readyButton.interactable = true;

        //_readyButton.onClick.AddListener(() =>
        //{
        //    _ready = !_ready;

        //    ColorBlock colours = _readyButton.colors;
        //    Color colour = _ready ? Color.green : Color.red;
        //    colours = _readyButton.colors;
        //    colours.normalColor = colour;
        //    colours.highlightedColor = colour * 0.96f;
        //    colours.pressedColor = colour * 0.78f;
        //    colours.selectedColor = colour * 0.96f;
        //    Color disabledColour = colours.disabledColor;
        //    disabledColour = colour * 0.78f;
        //    disabledColour.a = 0.5f;
        //    colours.disabledColor = disabledColour;
        //    _readyButton.colors = colours;
        //});
    }
}
