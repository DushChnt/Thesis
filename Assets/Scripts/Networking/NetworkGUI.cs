using UnityEngine;
using System.Collections;

public class NetworkGUI : MonoBehaviour {

    public dfButton BackButton;
    public dfButton StartButton;

    public dfPanel Slot1, Slot2, Slot3, Slot4;

	// Use this for initialization
	void Start () {
        BackButton.Click += BackButton_Click;
        StartButton.Click += StartButton_Click;

        Slot1.Click += Slot1_Click;
	}

    void Slot1_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        
    }

    void StartButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        NetworkManager.Stats();
    }

    void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Application.LoadLevel("Start Menu");
    }
}
