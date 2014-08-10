using UnityEngine;
using System.Collections;

public class MissionPanel : MonoBehaviour {

    public dfButton Mission1, Mission2, Mission3, Mission4, Mission5;
    public DetailPanel DetailPanel;
    public dfSprite SelectedSprite;
    public const string CURRENT_MISSION = "CurrentMission";
    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {        
        Mission1.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 1));
        Mission2.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 2));
        Mission3.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 3));
        Mission4.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 4));
        Mission5.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 5));

        DisableLockedMissions();
     //   MissionClick(null, null, Player.Level);
        // print("Clicking missions");
        // print("Player level: " + Player.Level);

        int mission = PlayerPrefs.GetInt(CURRENT_MISSION, 1);

        if (mission > Player.Level)
        {
            mission = Player.Level;
            PlayerPrefs.SetInt(CURRENT_MISSION, mission);
        }
        switch (mission)
        {
            case 1:
                Mission1.DoClick();
                // print("Clicking mission 1");
                break;
            case 2:
                Mission2.DoClick();  
                // print("Clicking mission 2");              
                break;
            case 3:
                Mission3.DoClick();
                // print("Clicking missions 3");
                break;
            case 4:
                Mission4.DoClick();
                // print("Clicking mission 4");
                break;
            case 5:
                // print("Clicking mission 5");
                Mission5.DoClick();
                break;
        }

	}

    void DisableLockedMissions()
    {
        if (Player.Level < 5)
        {
            Mission5.Disable();
            if (Player.Level < 4)
            {
                Mission4.Disable();
                if (Player.Level < 3)
                {
                    Mission3.Disable();
                    if (Player.Level < 2)
                    {
                        Mission2.Disable();
                    }
                }
            }
        }
    }

    void MissionClick(dfControl control, dfMouseEventArgs mouseEvent, int mission)
    {        
        PlayerPrefs.SetInt(CURRENT_MISSION, mission);
        DetailPanel.DisplayMission(mission, ((dfButton)control).Text);
        dfButton but = control as dfButton;
        SelectedSprite.RelativePosition = new Vector2(SelectedSprite.RelativePosition.x, but.RelativePosition.y + (but.Height - SelectedSprite.Height) / 2);
    }

    
	
	// Update is called once per frame
	void Update () {
	
	}
}
