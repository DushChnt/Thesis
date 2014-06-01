using UnityEngine;
using System.Collections;

public class DialogShow : MonoBehaviour {

    dfLabel _label;
    dfButton _button;
    dfPanel _panel;

	// Use this for initialization
	void Start () {
        _label = transform.Find("Label").GetComponent<dfLabel>();
        _button = transform.Find("Button").GetComponent<dfButton>();
        _panel = GetComponent<dfPanel>();

        _panel.Hide();
        _button.Click += button_Click;
	}

    void button_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        _panel.Hide();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowDialog(string text)
    {
        _label.Text = text;
        _panel.Show();
        _button.Enable();
    }
}
