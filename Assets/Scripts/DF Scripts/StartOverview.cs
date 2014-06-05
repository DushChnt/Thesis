using UnityEngine;
using System.Collections;
using Parse;
using System.IO;

public class StartOverview : MonoBehaviour {

    public dfLabel WelcomeLabel;
    public dfButton LogoutButton, FriendsButton;
    public dfButton BattleButton;

    public dfPanel Slot1, Slot2, Slot3, Slot4;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        WelcomeLabel.Text = "Welcome, " + ParseUser.CurrentUser.Username;
        LogoutButton.Click += LogoutButton_Click;
        BattleButton.Click += BattleButton_Click;
        FriendsButton.Click += new MouseEventHandler(FriendsButton_Click);
	}

    void FriendsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Friends scene");
    }

    void BattleButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PhotonNetwork.offlineMode = false;
        Application.LoadLevel("Network Test");
    }

    void LogoutButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        LogoutScript.Logout();
        ParseUser.LogOut();
        Application.LoadLevel("DFGUI Login");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
