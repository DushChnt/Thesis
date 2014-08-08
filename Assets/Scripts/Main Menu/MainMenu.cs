using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public dfButton BootcampButton, ChampionsButton, LogoutButton, ExitButton;
    dfPanel panel;
    public dfPanel WaitPanel;
    public dfButton ContinueButton, BackButton;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        panel = this.GetComponent<dfPanel>();
        BootcampButton.Click += new MouseEventHandler(BootcampButton_Click);
        ChampionsButton.Click += new MouseEventHandler(ChampionsButton_Click);        
        LogoutButton.Click += new MouseEventHandler(LogoutButton_Click);
        ExitButton.Click += new MouseEventHandler(ExitButton_Click);

        ContinueButton.Click += new MouseEventHandler(ContinueButton_Click);
        BackButton.Click += new MouseEventHandler(BackButton_Click);
	}

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        WaitPanel.Hide();
        panel.Enable();
    }

    void ContinueButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Champions arena");
    }

    void LogoutButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Parse.ParseUser.LogOut();
        Application.LoadLevel("DFGUI Login");
    }

    void ExitButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        print("Quit clicked");
        if (!Application.isEditor)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();

        }
    }

   

    

    void ChampionsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (Player.Level < 4)
        {
            WaitPanel.Show();
            panel.Disable();
        }
        else
        {
            Application.LoadLevel("Champions arena");
        }
    }

    void BootcampButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Bootcamp");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
