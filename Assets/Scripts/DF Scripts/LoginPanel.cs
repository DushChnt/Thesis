using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;

public class LoginPanel : MonoBehaviour {

    public dfButton LoginButton, RegisterButton, ForgotButton;
    public dfPanel RegisterPanel;
    public dfTextbox Username, Password;
    dfPanel loginPanel;
    public DialogPanel DialogPanel;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

	// Use this for initialization
	void Start () {
        loginPanel = GetComponent<dfPanel>();
        
        if (ParseUser.CurrentUser != null)
        {
            // User is logged in - proceed
            loginPanel.Hide();
            DoLogin();
            return;
        }
        // Show login screen
        LoginButton.Click += new MouseEventHandler(LoginButton_Click);
        RegisterButton.Click += new MouseEventHandler(RegisterButton_Click);
        ForgotButton.Click += new MouseEventHandler(ForgotButton_Click);

        loginPanel.KeyDown += new KeyPressHandler(loginPanel_KeyDown);

        DialogPanel.Dismissed += new global::DialogPanel.DismissedEventHandler(DialogPanel_Dismissed);
	}

    void ForgotButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        loginPanel.Disable();
        DialogPanel.ShowResetPassword();        
    }

    void loginPanel_KeyDown(dfControl control, dfKeyEventArgs keyEvent)
    {
        if (keyEvent.KeyCode == KeyCode.Return)
        {
            StartCoroutine(TryLogin());
        }
    }

    void LoginButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {

        StartCoroutine(TryLogin());
    }

    IEnumerator TryLogin()
    {

        loginPanel.Disable();
        Task LoginTask = ParseUser.LogInAsync(Username.Text, Password.Text);

        while (!LoginTask.IsCompleted)
        {
            yield return null;
        }

        if (LoginTask.IsFaulted || LoginTask.IsCanceled)
        {
            DialogPanel.Show("Oops", "It looks like you entered the wrong username or password. Try again.");
           
          //  loginPanel.Enable();
            
        }
        else
        {
            DoLogin();
        }
    }

    void DialogPanel_Dismissed(object sender, System.EventArgs e, ButtonState s)
    {
        loginPanel.Enable();
    }

    void RegisterButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        loginPanel.Hide();
        RegisterPanel.Show();
        RegisterPanel.GetComponent<RegisterPanel>().Username.Text = Username.Text;
        RegisterPanel.GetComponent<RegisterPanel>().Password.Text = Password.Text;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void DoLogin()
    {
        if (ParseUser.CurrentUser == null)
        {
            print("ParseUser is null");
        }
        Player player = (Player)ParseUser.CurrentUser;
        if (player == null)
        {
            print("player is null");
        }
        if (Player == null)
        {
            print("Player is null");
        }
        Player.IsOnline = true;
        Player.SaveAsync();
        Application.LoadLevel("Main Menu");
        Destroy(this);
    }


}
