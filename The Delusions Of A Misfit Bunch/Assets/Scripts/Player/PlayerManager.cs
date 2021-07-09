using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;



public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("The Beams GameObject to control.")]
    [SerializeField]
    GameObject _beams;
    bool _isFiring;
    [Tooltip("The current health of our player.")]
    [SerializeField]
    float _health = 1.0f;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene.")]
    public static GameObject localPlayerInstance;
    [Tooltip("The Player's UI GameObject Prefab.")]
    [SerializeField]
    public GameObject _playerUIPrefab;



    public float Health
    {
        get
        {
            return _health;
        }
    }

    private void Awake()
    {
        if (!_beams)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
        }
        else
        {
            _beams.SetActive(false);
        }


        if (photonView.IsMine)
        {
            localPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        CameraWork cameraWork = gameObject.GetComponent<CameraWork>();

        if (cameraWork)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }


#if UNITY_5_4_OR_NEWER
        SceneManager.sceneLoaded += onSceneLoaded;
#endif


        if (_playerUIPrefab)
        {
            GameObject _uiObj = Instantiate(_playerUIPrefab);
            _uiObj.SendMessage("setTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

#if UNITY_5_4_OR_NEWER
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= onSceneLoaded;
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            processInputs();

            if (_health <= 0.0f)
            {
                GameManager._instance.leaveRoom();
            }

        }

        if (_beams && _isFiring != _beams.activeInHierarchy)
        {
            _beams.SetActive(_isFiring);
        }
    }

    void processInputs()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            if (!_isFiring)
            {
                _isFiring = true;
            }
        }
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (_isFiring)
            {
                _isFiring = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        _health -= 0.1f;
        Debug.Log(PhotonNetwork.NickName + " Health: " + _health);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!other.name.Contains("Beam"))
        {
            return;
        }

        _health -= 0.1f * Time.deltaTime;
        Debug.Log(PhotonNetwork.NickName + " Health: " + _health);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data.
            stream.SendNext(_isFiring);
            stream.SendNext(_health);
        }
        else
        {
            // Network player, recieve data.
            _isFiring = (bool)stream.ReceiveNext();
            _health = (float)stream.ReceiveNext();
        }
    }

# if UNITY_5_4_OR_NEWER
    void onSceneLoaded(Scene scene, LoadSceneMode loadingMode)
    {
        calledOnLevelWasLoaded(scene.buildIndex);
    }
#endif

#if !UNITY_5_4_OR_NEWER
    void OnLevelWasLoaded(int level)
    {
        calledOnLevelWasLoaded(level);
    }
#endif

    void calledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5.0f))
        {
            transform.position = new Vector3(0.0f, 5.0f, 0.0f);
        }


        GameObject uiObj = Instantiate(_playerUIPrefab);
        uiObj.SendMessage("setTarget", this, SendMessageOptions.RequireReceiver);
    }
}
