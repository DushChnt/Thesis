using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyScript : Photon.MonoBehaviour {

    public dfButton CreateButton, JoinRandomButton, JoinButton, InviteButton, BackButton;
    public dfScrollPanel ActiveRoomsPanel;
    public GameObject ListItem;
    dfPanel currentlySelected;
    public dfPanel RoomInfoPanel;
    public dfLabel UsernameLabel, MapTypeLabel, GameModeLabel, OpenLabel;
    public dfDropdown ArenaDropdown, GameModeDropdown;

    // Use this for i14itialization
    void Start()
    {
        Connect();
        RoomInfoPanel.Disable();
        BackButton.Click += new MouseEventHandler(BackButton_Click);
        Time.timeScale = 1;
    }

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Start Menu");
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

                    dfLabel mapName = listItem.Find("Map Type").GetComponent<dfLabel>();
                    mapName.Text = "Map: " + game.customProperties["map"].ToString();

                    dfLabel modeName = listItem.Find("Game Mode").GetComponent<dfLabel>();
                    modeName.Text = game.customProperties["mode"].ToString();

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
     //   Application.LoadLevel("Network Battle");
     //   Application.LoadLevel("Arena1");

        //string arena = "Arena1";
        //switch (ArenaDropdown.SelectedValue)
        //{
        //    case "Arena 1":
        //        arena = "Arena1";
        //        break;
        //    case "Arena 2":
        //        arena = "Arena2";
        //        print("Arena2");
        //        break;
        //    case "Random":
        //        if (Random.Range(0, 1) == 0)
        //        {
        //            arena = "Arena2";
        //        }
        //        break;
        //    default:
        //        arena = "Arena1";
        //        break;
        //}
        //
        string arena = PhotonNetwork.room.customProperties["map"].ToString();
        print("Value of arena: " + arena);
        Application.LoadLevel(arena);
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
      //  Application.LoadLevel("Network Battle");
       
    //    Application.LoadLevel("Arena2");
    }

    void CreateButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        string arena = ArenaDropdown.SelectedValue;
        if (arena.Equals("Random"))
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    arena = "Arena 1";
                    break;
                case 1:
                    arena = "Arena 2";
                    break;
            }
        }
        //switch (ArenaDropdown.SelectedValue)
        //{
        //    case "Arena 1":
        //        arena = "Arena1";
        //        break;
        //    case "Arena 2":
        //        arena = "Arena2";
        //        print("Arena2");
        //        break;
        //    case "Random":
        //        if (Random.Range(0, 1) == 0)
        //        {
        //            arena = "Arena2";
        //        }
        //        break;
        //    default:
        //        arena = "Arena1";
        //        break;
        //}

        string mode = GameModeDropdown.SelectedValue;       

        string[] roomPropsInLobby = { "map", "mode" };
        Hashtable customRoomProperties = new Hashtable() { { "map", arena }, { "mode", mode } };

        PhotonNetwork.CreateRoom(Player.CurrentUser.Username, true, true, 2, customRoomProperties, roomPropsInLobby);    
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
