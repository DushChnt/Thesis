using UnityEngine;
using System.Collections;

public class DetailPanel : MonoBehaviour {

    public dfButton TestButton;
    public dfLabel MissionTextLabel, TitleLabel;
    dfPanel panel;

    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    string mission1Text = @"Train your robot to move around. 

Do so by adjusting the correct sliders for the brain that you are training.

In order to complete the mission you must create two brains; a brain that can approach the target and a brain that can flee from the target.";
    string mission2Text = "22224 your robot to move around.\n\n" +
        "Do so by adjusting the correct sliders for the brain that you are training. Something shorter now.\n\n";
        

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        TestButton.Click += new MouseEventHandler(TestButton_Click);
        
	}

    void TestButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel(string.Format("Mission {0}", PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1)));
    }

   

    public void DisplayMission(int mission, string title)
    {       
        if (mission == 1)
        {
            MissionTextLabel.Text = mission1Text;
        }
        else
        {
            MissionTextLabel.Text = mission2Text;
        }
        TitleLabel.Text = title;     
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
