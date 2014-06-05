using UnityEngine;
using System.Collections;

public class LobbyScript : Photon.MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Connect();        
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("TEST 0.1");

    }

    void OnJoinedLobby()
    {
        print("Joined lobby");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
