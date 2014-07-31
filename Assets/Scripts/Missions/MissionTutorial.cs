using UnityEngine;
using System.Collections;

public class MissionTutorial : MonoBehaviour {

    public dfButton StartButton;
    dfPanel panel;

	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        StartButton.Click += new MouseEventHandler(StartButton_Click);
	}

    void StartButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Time.timeScale = 1;
        panel.Hide();
    }

    public void ShowTutorial()
    {
        panel.Show();
        StartButton.Text = "OK";
    }

	// Update is called once per frame
	void Update () {
      
	}
}
