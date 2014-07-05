using UnityEngine;
using System.Collections;
using Parse;
using System.IO;

public class StartOverview : MonoBehaviour {

    public dfLabel WelcomeLabel;
    public dfButton LogoutButton;
    public dfButton LobbyButton;
    public dfButton StatisticsButton;

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
        LobbyButton.Click += new MouseEventHandler(LobbyButton_Click);
        StatisticsButton.Click += new MouseEventHandler(StatisticsButton_Click);
	}

    void StatisticsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Statistics Web scene");
     //   DeactivateChildren(GameObject.Find("UI Root"), false);
       // GameObject.Find("WebView Plane").transform.position = new Vector3(-7, 0, 2);
    }

    void DeactivateChildren(GameObject g, bool a)
    {
        g.SetActive(a);

        foreach (Transform child in g.transform)
        {
            DeactivateChildren(child.gameObject, a);
        }
    }

    void LobbyButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Lobby scene");
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
