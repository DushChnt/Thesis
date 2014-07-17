using UnityEngine;
using System.Collections;

public class MissionPanel : MonoBehaviour {

    public dfButton Mission1, Mission2, Mission3, Mission4, Mission5;
    public DetailPanel DetailPanel;
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
    }

    
	
	// Update is called once per frame
	void Update () {
	
	}
}
