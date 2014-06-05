using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class LobbyScript : Photon.MonoBehaviour {

    public dfButton CreateButton, JoinRandomButton, JoinButton, InviteButton;
    public dfScrollPanel ActiveRoomsPanel;
    public GameObject ListItem;

    // Use this for i14itialization
    void Start()
    {
        Connect();        
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("TEST 0.1");

    }

    void GenerateRoomList()
    {
        var children = new List<GameObject>();

        foreach (Transform child in ActiveRoomsPanel.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));
        if (PhotonNetwork.connected)
        {
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                dfPanel listItem = ActiveRoomsPanel.AddPrefab(ListItem) as dfPanel; // as UserListItem;
                listItem.Width = ActiveRoomsPanel.Width - ActiveRoomsPanel.FlowPadding.left - ActiveRoomsPanel.FlowPadding.right;

                dfLabel roomName = listItem.Find("Room Name").GetComponent<dfLabel>();
                roomName.Text = game.name;

                dfLabel numberPlayers = listItem.Find("Number Players").GetComponent<dfLabel>();
                numberPlayers.Text = string.Format("{0}/{1}", game.playerCount, game.maxPlayers);
            }
        }
    }

    void OnJoinedLobby()
    {
        print("Joined lobby");
        CreateButton.Click += new MouseEventHandler(CreateButton_Click);
        JoinRandomButton.Click += new MouseEventHandler(JoinRandomButton_Click);
    }

    void JoinRandomButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnJoinedRoom()
    {
        Application.LoadLevel("Network Battle");
    }

    void OnReceivedRoomListUpdate()
    {
        print("Updated room list");
        GenerateRoomList();
    }

    void OnPhotonRandomJoinFailed()
    {
        print("Failed join random");    
        
    }

    void OnCreatedRoom()
    {
        print("Joe?");
        Application.LoadLevel("Network Battle");
    }

    void CreateButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        RoomOptions rOpt = new RoomOptions();
        rOpt.isOpen = true;
        rOpt.isVisible = true;
        rOpt.maxPlayers = 2;
        TypedLobby tl = new TypedLobby();

        PhotonNetwork.CreateRoom(Player.CurrentUser.Username, rOpt, tl);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
