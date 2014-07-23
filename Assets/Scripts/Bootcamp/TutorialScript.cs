using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

    public dfLabel TitleLabel, ExplanationLabel, ProgressLabel;
    public dfButton NextButton, PrevButton, CloseButton;
    public dfTweenColor32 MissionFlash, RobotFlash, BrainsFlash;
    dfPanel panel;
    public int MaxPages = 4;
    public const string TUTORIAL_SEEN = "TutorialSeen";
   

    int CurrentPage = 1;

    #region LONG STRINGS
    string page1Title = "Welcome to robot bootcamp!";
    string page1Explanation = @"Here you will train the brains that control your robot to accomplish the tasks required to win the battles. 

At first your robot will only be able to move around, but once you master it you will unlock different attacks. When you have mastered all the skills of your robot you are ready to battle against others!

Click on the ? to bring up this window again.";

    string page2Title = "Your robot";
    string page2Explanation = @"On the right side you can see the status of your robot. You can see which level your robot is in and what attack types it has unlocked. Click on any unlocked attack type to choose it's weapon.";

    string page3Title = "Your missions";
    string page3Explanation = @"On the left side you can see the missions that you have to complete in order to make it through the bootcamp. Click on an available mission to get your orders. Keep these in mind and create the right set of brains to be able to complete the mission.

Once your brains are ready, select them for your robot and press on 'Test it!' to see if your robot can complete the mission.";

    string page4Title = "Your brains";
    string page4Explanation = @"In the middle you can see the brains you have trained for your robot. It is a good idea to create separate brains for specific tasks. You can use your existing brains as basis for new brains by clicking + on the brains. That way the new brains already know what the existing brains knows and you can then train it in a new direction.

Click on a brain to train it. Drag your brains to the 'Selected brains' panel to use them in battle.";
    #endregion
   
	// Use this for initialization
	void Start () {
        panel = GetComponent<dfPanel>();
        CloseButton.Click += new MouseEventHandler(CloseButton_Click);
        NextButton.Click += new MouseEventHandler(NextButton_Click);
        PrevButton.Click += new MouseEventHandler(PrevButton_Click);

        UpdatePage();

        if (PlayerPrefs.GetInt(TUTORIAL_SEEN, 0) == 1)
        {
            panel.Hide();
        }
	}

    void PrevButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        PrevPage();
    }

    void NextButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        NextPage();
    }

    void NextPage()
    {
        CurrentPage++;
        UpdatePage();
    }

    void PrevPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            UpdatePage();
        }
    }

    void UpdatePage()
    {
        ProgressLabel.Text = string.Format("{0} / {1}", CurrentPage, MaxPages);

        if (CurrentPage == 1)
        {
            PrevButton.Disable();
            PrevButton.Hide();
        }
        else
        {
            PrevButton.Enable();
            PrevButton.Show();
        }

        if (CurrentPage == MaxPages)
        {
            NextButton.Text = "Close";
            NextButton.RemoveAllEventHandlers();
            NextButton.Click += new MouseEventHandler(CloseButton_Click);
            RobotFlash.Stop();
            MissionFlash.Stop();
            BrainsFlash.Stop();
            RobotFlash.Reset();
            MissionFlash.Reset();
            BrainsFlash.Reset();
        }
        else
        {
            NextButton.Text = "Next";
            NextButton.RemoveAllEventHandlers();
            NextButton.Click += new MouseEventHandler(NextButton_Click);
        }

       
        RobotFlash.Reset();
        MissionFlash.Reset();
        BrainsFlash.Reset();
        RobotFlash.Stop();
        MissionFlash.Stop();
        BrainsFlash.Stop();
        switch (CurrentPage)
        {
            case 2:
                RobotFlash.Play();
                break;
            case 3:
                MissionFlash.Play();
                break;
            case 4:
                BrainsFlash.Play();
                break;
        }

        ShowText();
    }

    void ShowText()
    {
        string title = "";
        string explanation = "";
        switch (CurrentPage)
        {
            case 1:
                title = page1Title;
                explanation = page1Explanation;
                break;
            case 2:
                title = page2Title;
                explanation = page2Explanation;
                break;
            case 3:
                title = page3Title;
                explanation = page3Explanation;
                break;
            case 4:
                title = page4Title;
                explanation = page4Explanation;
                break;
        }
        TitleLabel.Text = title;
        ExplanationLabel.Text = explanation;
    }

    void OnControlShown()
    {
        CurrentPage = 1;
        UpdatePage();
    }

    void CloseButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        RobotFlash.Stop();
        MissionFlash.Stop();
        BrainsFlash.Stop();
        RobotFlash.Reset();
        MissionFlash.Reset();
        BrainsFlash.Reset();
        panel.Hide();

        PlayerPrefs.SetInt(TUTORIAL_SEEN, 1);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                NextButton.DoClick();
            }
            else
            {
                PrevPage();
            }
        }
	}
}
