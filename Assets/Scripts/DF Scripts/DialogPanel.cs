using UnityEngine;
using System.Collections;
using System;

public class DialogPanel : MonoBehaviour {

    public dfButton DismissButton;
    public dfLabel TitleLabel, MessageLabel;
    public dfTextbox EmailTextbox;
    dfPanel dialogPanel;

    bool EmailReset;

	// Use this for initialization
	void Start () {
        dialogPanel = GetComponent<dfPanel>();

        DismissButton.Click += new MouseEventHandler(DismissButton_Click);

        dialogPanel.KeyDown += new KeyPressHandler(dialogPanel_KeyDown);
	}

    void dialogPanel_KeyDown(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (keyEvent.KeyCode == KeyCode.Return)
        {
            DismissButton.DoClick();
        }
    }

    void DismissButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if (EmailReset)
        {
            print("Resetting for " + EmailTextbox.Text);
            Parse.ParseUser.RequestPasswordResetAsync(EmailTextbox.Text);
        }

        this.dialogPanel.Hide();
        Dismissed(this, EventArgs.Empty);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show(string title, string message) 
    {
        EmailReset = false;
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        this.dialogPanel.Show();
        EmailTextbox.Hide();
    }

    public void ShowResetPassword()
    {
        TitleLabel.Text = "Reset password";
        MessageLabel.Text = "Enter your email adress to reset your password. Look in your mailbox for further instructions.";
        EmailTextbox.Show();
        EmailReset = true;
        this.dialogPanel.Show();
        EmailTextbox.Focus();
    }

    public delegate void DismissedEventHandler(object sender, EventArgs e);

    public event DismissedEventHandler Dismissed;
}
