using UnityEngine;
using System.Collections;

public class DetailPanel : MonoBehaviour {

    public dfButton TestButton;
    public dfLabel MissionTextLabel, TitleLabel;
    dfPanel panel;

    

    string mission1Text = "Train your robot to move around.\n\n" + 
        "Do so by adjusting the correct sliders for the brain that you are training.\n\n" + 
        "I can fill quite a lot of text in here can't I?\n\nThat is pretty good I guess.";
    string mission2Text = "22224 your robot to move around.\n\n" +
        "Do so by adjusting the correct sliders for the brain that you are training. Something shorter now.\n\n";
        

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();    
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
