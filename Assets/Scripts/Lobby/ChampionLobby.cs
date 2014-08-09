using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ChampionLobby : MonoBehaviour {

    public dfButton CreateButton, JoinRandomButton;
    public dfScrollPanel ActiveRoomsPanel;
    public GameObject ListItem;
    dfPanel currentlySelected;   
    public dfDropdown ArenaDropdown, GameModeDropdown;
    public TrainingDialogScript TrainingDialog;
    public dfPanel MainPanel;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    // Use this for i14itialization
    void Start()
    {
        Connect();
        Time.timeScale = 1;
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
                foreach (RoomInfo game in PhotonNetwork.GetRoomList().OrderBy(t => Mathf.Abs((int)t.customProperties["level"] - Player.Level)))
                {
                    dfPanel listItem = ActiveRoomsPanel.AddPrefab(ListItem) as dfPanel; // as UserListItem;
                    listItem.Width = ActiveRoomsPanel.Width - ActiveRoomsPanel.FlowPadding.left - ActiveRoomsPanel.FlowPadding.right;
                    listItem.Click += new MouseEventHandler(listItem_Click);
                    listItem.DoubleClick += new MouseEventHandler(listItem_DoubleClick);

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
                       
                    }

                    dfLabel roomName = listItem.Find("Room Name").GetComponent<dfLabel>();
                    roomName.Text = string.Format("{0} ({1})", game.customProperties["name"].ToString(), game.customProperties["level"].ToString());

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

    bool TestWeapons()
    {
        bool ok = true;
        // Test for selected weapons
        if (Player.Level > 1)
        {
            if (Player.MeleeWeapon == null || Player.MeleeWeapon.Equals(""))
            {
                ok = false;
            }
            if (Player.Level > 2)
            {
                if (Player.RangedWeapon == null || Player.RangedWeapon.Equals(""))
                {
                    ok = false;
                }
                if (Player.Level > 3)
                {
                    if (Player.MortarWeapon == null || Player.MortarWeapon.Equals(""))
                    {
                        ok = false;
                    }
                }
            }
        }
        return ok;
    }

    bool TestBrains()
    {
        bool ok = Player.Brain1 != null || Player.Brain2 != null || Player.Brain3 != null || Player.Brain4 != null;
        return ok;
    }

    bool TestOK()
    {
        if (TestBrains())
        {
            if (TestWeapons())
            {
                return true;
            }
            else
            {
                MainPanel.Disable();
                TrainingDialog.ShowDialog("You have to choose a weapon before continuing. Click on the flashing slot to the right to assign.");
                TrainingDialog.panel.Click += new MouseEventHandler(panel_Click);
            }
        }
        else
        {
            MainPanel.Disable();
            TrainingDialog.ShowDialog("You haven't chosen any brains to use for battle! Drag the brains you wish to use down to the selected brains panel.");
            TrainingDialog.panel.Click += new MouseEventHandler(panel_Click);
        }
        return false;
    }

    void panel_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        MainPanel.Enable();
    }

    void listItem_DoubleClick(dfControl control, dfMouseEventArgs mouseEvent)
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
        if (TestOK())
        {
            if (data.Game.playerCount < data.Game.maxPlayers)
            {
                PhotonNetwork.JoinOrCreateRoom(data.Game.name, null, null);
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
       
    }   
    
    void OnJoinedLobby()
    {
        print("Joined lobby");
        CreateButton.Click += new MouseEventHandler(CreateButton_Click);
        JoinRandomButton.Click += new MouseEventHandler(JoinRandomButton_Click);
    }

    void JoinRandomButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (TestOK())
        {
            PhotonNetwork.JoinRandomRoom();
        }
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

        string[] roomPropsInLobby = { "map", "mode", "name", "level" };
        Hashtable customRoomProperties = new Hashtable() { { "map", arena }, { "mode", mode }, { "name", Player.CurrentUser.Username }, { "level", Player.Level } };
        if (TestOK())
        {
            PhotonNetwork.CreateRoom(Player.CurrentUser.Username + GenerateRandomTag(), true, true, 2, customRoomProperties, roomPropsInLobby);
        }
    }

    private string GenerateRandomTag()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new System.String(stringChars);
        return finalString;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
