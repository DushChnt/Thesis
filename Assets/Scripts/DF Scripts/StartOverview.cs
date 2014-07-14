using UnityEngine;
using System.Collections;
using Parse;
using System.IO;

public class StartOverview : MonoBehaviour {

    public dfLabel WelcomeLabel;
    public dfButton LogoutButton;
    public dfButton LobbyButton;
    public dfButton StatisticsButton;

    private DialogShow _dialog;

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
        _dialog = GameObject.Find("Dialog").GetComponent<DialogShow>();
        WelcomeLabel.Text = "Welcome, " + ParseUser.CurrentUser.Username;
        LogoutButton.Click += LogoutButton_Click;        
        LobbyButton.Click += new MouseEventHandler(LobbyButton_Click);
        StatisticsButton.Click += new MouseEventHandler(StatisticsButton_Click);
	}

    void StatisticsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
    //    Application.LoadLevel("Statistics Web scene");
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
        if (Slot1.GetComponent<BrainPanelState>().InUse || Slot2.GetComponent<BrainPanelState>().InUse
            || Slot3.GetComponent<BrainPanelState>().InUse || Slot4.GetComponent<BrainPanelState>().InUse)
        {
            Application.LoadLevel("Lobby scene");
        }
        else
        {
            _dialog.ShowDialog("You must select at least one brain to use in battle before you can start fighting!" +
                "\n\nTo select a brain, simply drag it down to an empty slot in the 'Selected brains' panel.");
        }
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
