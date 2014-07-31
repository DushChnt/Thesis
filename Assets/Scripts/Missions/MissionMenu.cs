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
	}

    void QuitButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Bootcamp");
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
        Time.timeScale = 0;
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
