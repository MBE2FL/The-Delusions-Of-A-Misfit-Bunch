using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Shooting : MonoBehaviour
{
    [SerializeField]
    private PhotonView photonView;

    private GameObject[] bulletBank = new GameObject[10];
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject bulletSpawnPoint;

    private float shootTimer = 0.0f;
    private float bulletLifeTimer = 5.0f;

    private bool _isFiring;


    // Start is called before the first frame update
    void Start()
    {
        if (!bullet)
        {
            Debug.LogError("there is no bullet prefab");
        }
        else
        {
            for (int i = 0; i < bulletBank.Length; i++)//instantiates the bullets at one go to form an object pool
            {
                bulletBank[i] = Instantiate(bullet);
                bulletBank[i].transform.position = bulletSpawnPoint.transform.position;
                //DontDestroyOnLoad(bulletBank[i]);
                bulletBank[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            processInputs();
        }

        shootTimer += Time.deltaTime;

        //this is for activating the bullet and firing if there is any in the bullet bank
        if (_isFiring && shootTimer >= 0.5f)
        {
            for (int i = 0; i < bulletBank.Length; i++)
            {
                if (!bulletBank[i].activeInHierarchy)
                {
                    photonView.RPC("ShotFiredCall", RpcTarget.All, i);//calls the rpc function for shooting the bullet
                    break;
                }
            }
        }

        //this checks to see if the life span of the bullet has ended and will put it back in the bullet bank and reset the life span
        for (int i = 0; i < bulletBank.Length; i++)
        {
            if (bulletBank[i].activeInHierarchy && bulletBank[i].GetComponent<Bullet>().getLifeTime() >= bulletLifeTimer)
            {
                photonView.RPC("ShotDeadCall", RpcTarget.All, i);//calls the rpc function to reset the bullet
            }
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

    [PunRPC]
    void ShotFiredCall(int bulletNum)
    {
        bulletBank[bulletNum].SetActive(true);
        bulletBank[bulletNum].transform.position = bulletSpawnPoint.transform.position;//sets postion of the bullet
        bulletBank[bulletNum].GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.transform.GetComponentInParent<Transform>().forward * 500);//adds force to the bullet
        shootTimer = 0.0f;//resets the shoot timer
    }

    [PunRPC]
    void ShotDeadCall(int bulletNum)
    {
        bulletBank[bulletNum].GetComponent<Bullet>().resetLifeTime();//resets bullet lifespan
        bulletBank[bulletNum].GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);//resets bullet velocity
        bulletBank[bulletNum].transform.position = bulletSpawnPoint.transform.position;//restes position
        bulletBank[bulletNum].SetActive(false);
    }
}
