using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerUI : MonoBehaviour
{
    [Tooltip("UI Text to display Player's Name.")]
    [SerializeField]
    Text _playerNameText;

    [Tooltip("UI Slider to display Player's Health.")]
    [SerializeField]
    Slider _playerHealthSlider;

    PlayerManager _target;

    [Tooltip("Pixel offset from the player target.")]
    [SerializeField]
    Vector3 _screenOffset = new Vector3(0.0f, 30.0f, 0.0f);
    float _characterControllerHeight = 0.0f;
    Transform _targetTransform;
    Renderer _targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 _targetPosition;



    private void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_target)
        {
            Destroy(gameObject);
            return;
        }


        if (_playerHealthSlider)
        {
            _playerHealthSlider.value = _target.Health;
        }
    }

    private void LateUpdate()
    {
        if (_targetRenderer)
        {
            _canvasGroup.alpha = _targetRenderer.isVisible ? 1.0f : 0.0f;
        }

        if (_targetTransform)
        {
            _targetPosition = _targetTransform.position;
            _targetPosition.y += _characterControllerHeight;
            transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + _screenOffset;
        }
    }

    public void setTarget(PlayerManager target)
    {
        if (!target)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        _target = target;

        _targetTransform = _target.GetComponent<Transform>();
        _targetRenderer = _target.GetComponent<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();

        if (characterController)
        {
            _characterControllerHeight = characterController.height;
        }

        if (_playerNameText)
        {
            _playerNameText.text = target.photonView.Owner.NickName;
        }
    }
}
