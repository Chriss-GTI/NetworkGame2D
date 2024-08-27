using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkmanager;

    private void OnGUI()
    {

        if (GUILayout.Button("Host"))
        {
            networkmanager.StartHost();
        }

        if (GUILayout.Button("Join"))
        {
            networkmanager.StartClient();
        }


        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
    }


}