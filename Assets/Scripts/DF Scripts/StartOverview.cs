using UnityEngine;
using System.Collections;
using Parse;

public class StartOverview : MonoBehaviour {

    public dfLabel WelcomeLabel;
    public dfButton LogoutButton;

	// Use this for initialization
	void Start () {
        WelcomeLabel.Text = "Welcome, " + ParseUser.CurrentUser.Username;
        LogoutButton.Click += LogoutButton_Click;
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
