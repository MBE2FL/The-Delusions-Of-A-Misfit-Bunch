using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Camera camera;
    [SerializeField]
    Transform playerTransform;

    public Vector3 followDist;

    void Start()
    {
        camera = Camera.main;

        if(!camera)
        {
            Debug.Log("no camera found");
        }

        playerTransform = transform;

        if(!playerTransform)
        {
            Debug.Log("no playerfound");
        }
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.LookAt(playerTransform);

        camera.transform.position = playerTransform.position + followDist;
    }
}
