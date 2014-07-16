using UnityEngine;
using System.Collections;

public class GeneralUI : MonoBehaviour {

    public dfButton BackButton;

	// Use this for initialization
	void Start () {
        BackButton.Click += new MouseEventHandler(BackButton_Click);
	}

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Main Menu");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
