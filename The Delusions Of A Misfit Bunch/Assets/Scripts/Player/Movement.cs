using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField]
    InputAction _horizontalAxis;
    [SerializeField]
    InputAction _verticalAxis;
    [SerializeField]
    float velocity = 20.0f;

    Vector3 mouseScreenPos;
    Vector3 mouse3DPos;

    // Start is called before the first frame update
    void Start()
    {
        _horizontalAxis.Enable();
        _verticalAxis.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMoveValue = _horizontalAxis.ReadValue<float>();//gets the horizontal key values between 1 - -1 or w and s (idk hori and vert are flipped)
        float verticalMoveValue = _verticalAxis.ReadValue<float>(); //gets the vertical key values between 1 - -1 or a and d

        MouseMovement();
        PlayerMovement(Time.deltaTime, verticalMoveValue, horizontalMoveValue);
    }


    void MouseMovement()
    {
        //this gets the mouse pixel coordinates
        mouseScreenPos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), Mouse.current.position.y.ReadValue());

        //this gets a ray to shoot from the mouses pixel coords
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        // we then cast the ray
        if (Physics.Raycast(ray, out RaycastHit rayCastHit))
        {
            mouse3DPos = new Vector3(rayCastHit.point.x, transform.position.y, rayCastHit.point.z); // we get the point of hit from the ray
            transform.LookAt(mouse3DPos);// look at the position with the character
        }
    }

    void PlayerMovement(float deltatime, float vertical, float horizontal)
    {
        Vector3 verticalVectorVel = new Vector3(0, 0.0f, velocity * deltatime * vertical);// creates the vertical movement vector with deltatime 
        Vector3 horizontalVectorVel = new Vector3(velocity * deltatime * horizontal, 0.0f, 0);// creates the horizontal movement vector with deltatime

        Vector3 newPos = transform.position + verticalVectorVel + horizontalVectorVel;//adds all the vectors up to make the new position

        transform.position = newPos;//transfor becomes the new position
    }

    
}
