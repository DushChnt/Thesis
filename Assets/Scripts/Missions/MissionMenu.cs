using UnityEngine;
using System.Collections;

public class MissionMenu : MonoBehaviour {

    public dfButton InstructionsButton, ResumeButton, QuitButton;
    public MissionTutorial Tutorial;
    dfPanel panel;

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        InstructionsButton.Click += new MouseEventHandler(InstructionsButton_Click);
        ResumeButton.Click += new MouseEventHandler(ResumeButton_Click);
        QuitButton.Click += new MouseEventHandler(QuitButton_Click);

        switch (PlayerPrefs.GetString(CameFromScript.CAME_FROM))
        {
            case CameFromScript.BOOTCAMP:
                QuitButton.Text = "Quit mission";
                break;
            case CameFromScript.CHAMPIONS_ARENA:
                QuitButton.Text = "Quit match";
                InstructionsButton.Hide();
                break;
        }
	}

    void QuitButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        switch (PlayerPrefs.GetString(CameFromScript.CAME_FROM))
        {
            case CameFromScript.BOOTCAMP:
                Application.LoadLevel("Bootcamp");
                break;
            case CameFromScript.CHAMPIONS_ARENA:
                PhotonNetwork.Disconnect();
                Application.LoadLevel("Champions arena");
                break;
        }
        Time.timeScale = 1;
    }

    void ResumeButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Hide();
        Time.timeScale = 1;
    }

    void InstructionsButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Hide();
        Tutorial.ShowTutorial();
    }

    public void ShowMissionMenu()
    {
        switch (PlayerPrefs.GetString(CameFromScript.CAME_FROM))
        {
            case CameFromScript.BOOTCAMP:
                Time.timeScale = 0;
                break;
            case CameFromScript.CHAMPIONS_ARENA:
                Time.timeScale = 1;
                break;
        }
        
        panel.Show();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowMissionMenu();
        }
	}
}
