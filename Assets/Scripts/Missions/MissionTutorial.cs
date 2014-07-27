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
        panel.Hide();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
