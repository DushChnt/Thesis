using UnityEngine;
using System.Collections;
using System;

public class DialogPanel : MonoBehaviour {

    public dfButton DismissButton, CancelButton;
    public dfLabel TitleLabel, MessageLabel;
    public dfTextbox EmailTextbox;
    dfPanel dialogPanel;

    bool EmailReset;

	// Use this for initialization
	void Start () {
        dialogPanel = GetComponent<dfPanel>();
        if (DismissButton == null)
        {
            print("Null");
        }
        DismissButton.Click += new MouseEventHandler(DismissButton_Click);

        if (CancelButton != null)
        {
            print("Not null");
            CancelButton.Click += new MouseEventHandler(CancelButton_Click);
        }

        dialogPanel.KeyDown += new KeyPressHandler(dialogPanel_KeyDown);
	}

    void CancelButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        print("Cancel");
        this.dialogPanel.Hide();
        EventArgs args = new EventArgs();
        
        Dismissed(this, EventArgs.Empty, ButtonState.Cancel);
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
        Dismissed(this, EventArgs.Empty, ButtonState.OK);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowCancel(string title, string message)
    {
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        this.dialogPanel.Show();
        EmailTextbox.Hide();
        EmailReset = false;
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

    public delegate void DismissedEventHandler(object sender, EventArgs e, ButtonState s);

    public event DismissedEventHandler Dismissed;
}

public enum ButtonState {
    OK, Cancel
}
