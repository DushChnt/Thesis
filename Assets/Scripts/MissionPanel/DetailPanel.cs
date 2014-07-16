using UnityEngine;
using System.Collections;

public class DetailPanel : MonoBehaviour {

    public dfButton CloseButton, TestButton;
    public dfLabel MissionTextLabel;
    dfPanel panel;

    string mission1Text = "Train your robot to move around.\n\n" + 
        "Do so by adjusting the correct sliders for the brain that you are training.\n\n" + 
        "I can fill quite a lot of text in here can't I?\n\nThat is pretty good I guess.";
    string mission2Text = "22224 your robot to move around.\n\n" +
        "Do so by adjusting the correct sliders for the brain that you are training. Something shorter now.\n\n";
        

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        CloseButton.Click += new MouseEventHandler(CloseButton_Click);
	}

    void CloseButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Hide();
    }

    public void DisplayMission(int mission, float y)
    {
        panel.Show();       
        print("Y: " + y);
        panel.RelativePosition = new Vector3(16, y);
        if (mission == 1)
        {
            MissionTextLabel.Text = mission1Text;
        }
        else
        {
            MissionTextLabel.Text = mission2Text;
        }
        panel.Height = MissionTextLabel.Height + 100;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
