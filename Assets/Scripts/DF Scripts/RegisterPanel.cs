using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using System;

public class RegisterPanel : MonoBehaviour {

	public dfTextbox Username, Password, Email;
	public dfButton RegisterButton, BackButton;
	public dfPanel LoginPanel;
	dfPanel registerPanel;
    public DialogPanel DialogPanel;

	// Use this for initialization
	void Start () {
		this.registerPanel = GetComponent<dfPanel>();

		BackButton.Click += new MouseEventHandler(BackButton_Click);
		RegisterButton.Click += new MouseEventHandler(RegisterButton_Click);

        registerPanel.KeyDown += new KeyPressHandler(registerPanel_KeyDown);
	}

    void registerPanel_KeyDown(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (keyEvent.KeyCode == KeyCode.Return)
        {
            RegisterButton.DoClick();
        }
    }

	void RegisterButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		StartCoroutine(TryRegister());
	}

	IEnumerator TryRegister()
	{
        registerPanel.Disable();
		var user = new ParseUser()
		{
			Username = Username.Text,
			Password = Password.Text,
			Email = Email.Text
		};


		Task signUpTask = user.SignUpAsync();

		while (!signUpTask.IsCompleted)
		{
			yield return null;
		}
		if (signUpTask.IsFaulted || signUpTask.IsCanceled)
		{
            DialogPanel.Show("Think differently", "The username or email adress is already in use. Please choose something even more original.");
            DialogPanel.Dismissed += new global::DialogPanel.DismissedEventHandler(DialogPanel_Dismissed);
		}
		else
		{
			LoginPanel.GetComponent<LoginPanel>().Username.Text = user.Username;
			LoginPanel.GetComponent<LoginPanel>().Password.Text = Password.Text;

			registerPanel.Hide();
			LoginPanel.Show();
		}
	}

    void DialogPanel_Dismissed(object sender, EventArgs e, ButtonState s)
    {
        registerPanel.Enable();
    }

	void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		this.registerPanel.Hide();
		LoginPanel.Show();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
