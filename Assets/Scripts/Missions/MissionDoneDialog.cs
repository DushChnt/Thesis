using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class MissionDoneDialog : MonoBehaviour {

    public dfButton LevelUpButton;
    public bool LoseDialog;
    int mission;
    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
    void Start()
    {
        mission = PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1);
        LevelUpButton.Click += new MouseEventHandler(LevelUpButton_Click);
        if (Player.Level > mission)
        {
            LevelUpButton.Text = "Back";
        }
    }

    void LevelUpButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (!LoseDialog)
        {
            LevelUp();
        }
       
        Application.LoadLevel("Bootcamp");
    }

    void LevelUp()
    {
        if (Player.Level <= mission)
        {
            Player.Level = mission + 1;
            Player.SaveAsync();
            var dimensions = new Dictionary<string, string> 
            {
                { "level", "Level " + Player.Level }            
            };

            ParseAnalytics.TrackEventAsync("Level Up", dimensions);
        }
        if (mission < 5)
        {
            PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, mission + 1);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
