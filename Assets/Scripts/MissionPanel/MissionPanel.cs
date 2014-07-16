using UnityEngine;
using System.Collections;

public class MissionPanel : MonoBehaviour {

    public dfButton Mission1, Mission2, Mission3, Mission4, Mission5;
    public DetailPanel DetailPanel;
    public const string CURRENT_MISSION = "CurrentMission";

	// Use this for initialization
	void Start () {        
        Mission1.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 1));
        Mission2.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 2));
        Mission3.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 3));
        Mission4.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 4));
        Mission5.Click += new MouseEventHandler((sender, e) => MissionClick(sender, e, 5));
	}

    void MissionClick(dfControl control, dfMouseEventArgs mouseEvent, int mission)
    {
        print("Clicked on mission " + mission);
        print("Control: " + control.RelativePosition);
        PlayerPrefs.SetInt(CURRENT_MISSION, mission);
        DetailPanel.DisplayMission(mission, control.RelativePosition.y + control.Height);
    }

    
	
	// Update is called once per frame
	void Update () {
	
	}
}
