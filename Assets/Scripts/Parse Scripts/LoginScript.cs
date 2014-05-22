using UnityEngine;
using System.Collections;
using Parse;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

public class LoginScript : MonoBehaviour {

    LoginState state;
    string UserName = "";
    string Password = "";
    string Email = "";
    bool enterPressed = false;
    float loadtime = 2f;
    float loadTimer = 0;

	// Use this for initialization
    void Start()
    {
        state = LoginState.WaitForConnection;

        ParseQuery<ParseObject> query = ParseObject.GetQuery("TestObject");
        query.GetAsync("BAi0Ok6Ipi").ContinueWith(t =>
        {
            if (t != null)
            {
                CheckAutoLogin();
            }
        });
    }

    void CheckAutoLogin()
    {
        if (ParseUser.CurrentUser != null)
        {
            // User is logged in - proceed
            UserName = ParseUser.CurrentUser.Username;
            DoLogin();
        }
        else
        {
            // Show login screen
            state = LoginState.LoginPending;
        }
    }

    void OnGUI()
    {
        switch (state)
        {
            case LoginState.WaitForConnection:
                WaitForConnectionGUI();
                break;
            case LoginState.LoginPending:
                LoginPendingGUI();
                break;
            case LoginState.LoggingIn:
                LoggingInGUI();
                break;
            case LoginState.Register:
                RegisterGUI();
                break;
        }
    }

    void WaitForConnectionGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 50), "Waiting on connection...");
    }

    void DoLogin()
    {
        state = LoginState.LoggingIn;
    }

    void TryLogin()
    {
        ParseUser.LogInAsync(UserName, Password).ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                // Login failed
            }
            else
            {
                // Login successful
                DoLogin();
            }
        });
    }

    void TryLogout()
    {
        ParseUser.LogOut();
        state = LoginState.LoginPending;
    }

    void TryRegister()
    {
        var user = new ParseUser()
        {
            Username = UserName,
            Password = Password,
            Email = Email
        };

        try
        {
            Task signUpTask = user.SignUpAsync().ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        state = LoginState.LoginPending;
                    }
                });
        }
        catch (Exception e)
        {
            print(e.StackTrace);
        }
    }

    void LoginPendingGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "password_input")
        {
            enterPressed = true;
        }
        else
        {
            enterPressed = false;
            GUI.SetNextControlName("password_input");
            Password = GUI.PasswordField(new Rect(50, 150, 200, 25), Password, '¤');
        }
        GUI.Label(new Rect(50, 50, 200, 20), "Please log in.");

        GUI.Label(new Rect(50, 80, 200, 20), "Username:");
        UserName = GUI.TextField(new Rect(50, 100, 200, 25), UserName);
        GUI.Label(new Rect(50, 130, 200, 20), "Password:");
       
        

        if (GUI.Button(new Rect(50, 190, 90, 30), "Log in"))
        {
            TryLogin();
        }

        if (GUI.Button(new Rect(160, 190, 90, 30), "Register"))
        {
            state = LoginState.Register;
        }

        
        

        if (enterPressed)
        {
            TryLogin();
        }
    }

    void LoggingInGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 50), "Welcome, " + UserName + "!");
        if (GUI.Button(new Rect(50, 180, 90, 30), "Log out"))
        {
            TryLogout();
        }

        loadTimer += Time.deltaTime;
        if (loadTimer > loadtime)
        {
            Application.LoadLevel("Start Menu");
            Destroy(this);
        }
    }

    void RegisterGUI()
    {
        GUI.Label(new Rect(50, 50, 200, 20), "Register");

        GUI.Label(new Rect(50, 80, 200, 20), "Username:");
        UserName = GUI.TextField(new Rect(50, 100, 200, 25), UserName);
        GUI.Label(new Rect(50, 130, 200, 20), "Password:");
        Password = GUI.PasswordField(new Rect(50, 150, 200, 25), Password, '¤');
        GUI.Label(new Rect(50, 180, 200, 20), "Email:");
        Email = GUI.TextField(new Rect(50, 200, 200, 25), Email);

        if (GUI.Button(new Rect(50, 230, 90, 30), "Register"))
        {
            TryRegister();
        }
    }

    bool GUIKeyDown(KeyCode key)
    {
        if (Event.current.type == EventType.KeyDown)
            return (Event.current.keyCode == key);
        return false;
    }
}

public enum LoginState
{
    WaitForConnection, LoginPending, LoggingIn, Register
}