using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    const string _playerNamePrefKey = "PlayerName";



    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        InputField inputField = GetComponent<InputField>();

        if (inputField)
        {
            if (PlayerPrefs.HasKey(_playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(_playerNamePrefKey);
                inputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void setPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty!");
            return;
        }

        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(_playerNamePrefKey, value);
    }
}
