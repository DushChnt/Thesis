using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public dfButton BootcampButton, ChampionsButton, StatisticsButton, OptionsButton, LogoutButton, ExitButton;

	// Use this for initialization
	void Start () {
        BootcampButton.Click += new MouseEventHandler(BootcampButton_Click);
        ChampionsButton.Click += new MouseEventHandler(ChampionsButton_Click);
        StatisticsButton.Click += new MouseEventHandler(StatisticsButton_Click);
        OptionsButton.Click += new MouseEventHandler(OptionsButton_Click);
        LogoutButton.Click += new MouseEventHandler(LogoutButton_Click);
        ExitButton.Click += new MouseEventHandler(ExitButton_Click);
	}

    void LogoutButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Parse.ParseUser.LogOut();
        Application.LoadLevel("DFGUI Login");
    }

    void ExitButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.Quit();
    }

    void OptionsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        throw new System.NotImplementedException();
    }

    void StatisticsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        throw new System.NotImplementedException();
    }

    void ChampionsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        throw new System.NotImplementedException();
    }

    void BootcampButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Bootcamp");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
