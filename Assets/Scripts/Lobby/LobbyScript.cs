using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class LobbyScript : Photon.MonoBehaviour {

    public dfButton CreateButton, JoinRandomButton, JoinButton, InviteButton;
    public dfScrollPanel ActiveRoomsPanel;
    public GameObject ListItem;
    dfPanel currentlySelected;
    public dfPanel RoomInfoPanel;
    public dfLabel UsernameLabel, MapTypeLabel, GameModeLabel, OpenLabel;

    // Use this for i14itialization
    void Start()
    {
        Connect();
        RoomInfoPanel.Disable();
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
            int sel_idx = -1;
            if (currentlySelected != null)
            {
                sel_idx = currentlySelected.GetComponent<RoomData>().Index;
            }
            int idx = 0;
            for (int i = 0; i < 1; i++)
            {
                foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                {
                    dfPanel listItem = ActiveRoomsPanel.AddPrefab(ListItem) as dfPanel; // as UserListItem;
                    listItem.Width = ActiveRoomsPanel.Width - ActiveRoomsPanel.FlowPadding.left - ActiveRoomsPanel.FlowPadding.right;
                    listItem.Click += new MouseEventHandler(listItem_Click);

                    RoomData data = listItem.GetComponent<RoomData>();
                    data.Game = game;
                    data.Index = idx++;

                    if (data.Index == sel_idx)
                    {
                        print("They match!");
                        data.Selected = true;
                        listItem.BackgroundColor = new Color32(0, 0, 0, 255);
                        currentlySelected = listItem;
                        currentlySelected.Find("Selected Indicator").GetComponent<dfSprite>().Show();
                        UpdateInfoBox();
                    }

                    dfLabel roomName = listItem.Find("Room Name").GetComponent<dfLabel>();
                    roomName.Text = game.name;

                    dfLabel numberPlayers = listItem.Find("Number Players").GetComponent<dfLabel>();
                    numberPlayers.Text = string.Format("{0}/{1}", game.playerCount, game.maxPlayers);

                    if (game.playerCount < game.maxPlayers)
                    {
                        numberPlayers.Color = new Color32(0, 255, 0, 255);
                    }
                    else
                    {
                        numberPlayers.Color = new Color32(255, 0, 0, 255);
                    }
                }
            }
        }
    }

    void listItem_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (currentlySelected != null)
        {
            currentlySelected.BackgroundColor = new Color32(255, 255, 255, 255);
            currentlySelected.Find("Selected Indicator").GetComponent<dfSprite>().Hide();
        }

        dfPanel item = control as dfPanel;
        RoomData data = item.GetComponent<RoomData>();
        data.Selected = true;
        item.BackgroundColor = new Color32(0, 0, 0, 255);
        currentlySelected = item;
        currentlySelected.Find("Selected Indicator").GetComponent<dfSprite>().Show();

        UpdateInfoBox();
    }

    void UpdateInfoBox()
    {
        if (currentlySelected != null)
        {
            RoomData data = currentlySelected.GetComponent<RoomData>();
            RoomInfoPanel.Enable();
            UsernameLabel.Text = data.Game.name;
            MapTypeLabel.Text = "Map 1";
            GameModeLabel.Text = "Showdown";
            if (data.Game.playerCount < data.Game.maxPlayers)
            {
                OpenLabel.Text = "Open";
                OpenLabel.Color = new Color32(0, 255, 0, 255);
                JoinButton.Enable();
                JoinButton.Click += new MouseEventHandler(JoinButton_Click);
            }
            else
            {
                OpenLabel.Text = "Closed";
                OpenLabel.Color = new Color32(255, 0, 0, 255);
                JoinButton.Disable();
            }
        }
        else
        {
            RoomInfoPanel.Disable();
        }
    }

    void JoinButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        RoomData data = currentlySelected.GetComponent<RoomData>();
        PhotonNetwork.JoinOrCreateRoom(data.Game.name, null, null);
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
