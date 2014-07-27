﻿using UnityEngine;
using System.Collections;

public class MissionDoneDialog : MonoBehaviour {

    public dfButton LevelUpButton;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        LevelUpButton.Click += new MouseEventHandler(LevelUpButton_Click);
	}

    void LevelUpButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        LevelUp();
        Application.LoadLevel("Bootcamp");
    }

    void LevelUp()
    {
        int mission = PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1);
        if (Player.Level <= mission)
        {
            Player.Level = mission + 1;
            Player.SaveAsync();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
