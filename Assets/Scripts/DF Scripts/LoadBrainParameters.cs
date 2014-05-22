using UnityEngine;
using System.Collections;

public class LoadBrainParameters : MonoBehaviour {

    public dfTextbox NameTextbox;
    public dfTextbox DescriptionTextBox;

	// Use this for initialization
	void Start () {
        if (Settings.Brain == null)
        {
            Settings.Brain = new Brain();
        }
     //   if (!Settings.IsNewBrain && Settings.Brain.Name != null && Settings.Brain.Name.Length > 0)
        {
            NameTextbox.Text = Settings.Brain.Name;
        }
   //     if (!Settings.IsNewBrain && Settings.Brain.Description != null && Settings.Brain.Description.Length > 0)
        {
            DescriptionTextBox.Text = Settings.Brain.Description;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
