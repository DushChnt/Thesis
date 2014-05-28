using UnityEngine;
using System.Collections;
using Parse;

public class StartOverview : MonoBehaviour {

    public dfLabel WelcomeLabel;
    public dfButton LogoutButton;
    public dfButton BattleButton;

	// Use this for initialization
	void Start () {
        WelcomeLabel.Text = "Welcome, " + ParseUser.CurrentUser.Username;
        LogoutButton.Click += LogoutButton_Click;
        BattleButton.Click += BattleButton_Click;
	}

    void BattleButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PhotonNetwork.offlineMode = false;
        Application.LoadLevel("Network Test");
    }

    void LogoutButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ParseUser.LogOut();
        Application.LoadLevel("Login scene");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
