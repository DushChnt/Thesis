using UnityEngine;
using System.Collections;

public class TrainingDialogScript : MonoBehaviour {

    public dfButton OKButton;
    public dfPanel panel;

	// Use this for initialization
	void Start () {
        panel = this.GetComponent<dfPanel>();
        OKButton.Click += new MouseEventHandler(OKButton_Click);
	}

    void OKButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        panel.Hide();
    }

    public void ShowDialog()
    {
        panel.Show();
    }

    public void ShowDialog(string text)
    {
        dfLabel label = panel.transform.FindChild("Text Label").GetComponent<dfLabel>();
        label.Text = text;
        panel.Show();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
