using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;



public class PlayerAnimatorManager : MonoBehaviourPun
{
    Animator _animator;
    [SerializeField]
    InputAction _horizontalAxis;
    [SerializeField]
    InputAction _verticalAxis;
    [SerializeField]
    float _directionDampTime = 0.25f;



    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (!_animator)
        {
            Debug.LogError("PlayerAnimatorManager is missing an animator component!", this);
        }

        _horizontalAxis.Enable();
        _verticalAxis.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (!_animator)
        {
            return;
        }


        // Jumping
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        
        // Only allow jumping if we are running.
        if (stateInfo.IsName("Base Layer.Run"))
        {
            Mouse mouse = Mouse.current;

            if (mouse.rightButton.wasPressedThisFrame)
            {
                _animator.SetTrigger("Jump");
            }
        }


        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        float h = _horizontalAxis.ReadValue<float>();
        float v = _verticalAxis.ReadValue<float>();

        //Mouse mouse = Mouse.current;
        //Vector2 mouseDelta = mouse.delta.ReadValue();
        //float h = mouseDelta.x;
        //float v = mouseDelta.y;

        //if (v < 0.0f)
        //{
        //    v = 0.0f;
        //}
  
        //_animator.SetFloat("Speed", (h * h) + (v * v));
        _animator.SetFloat("Speed", v);
        _animator.SetFloat("Direction", h, _directionDampTime, Time.deltaTime);
    }
}
