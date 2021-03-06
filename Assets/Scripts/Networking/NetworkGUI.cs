﻿using UnityEngine;
using System.Collections;

public class NetworkGUI : MonoBehaviour {	
   
	public dfPanel Slot1, Slot2, Slot3, Slot4, CountdownPanel;
	public FightController MyRobot, OpponentRobot;
    public dfLabel OwnWinsLabel, OpponentWinsLabel, TimeLabel, OwnNameLabel, OpponentNameLabel, StatusLabel, TipLabel, BestOfLabel;
    public ResetButton ResetButton;
	BrainPanelState activePanel;
    dfLabel countdownLabel, countdownTitle, countdownFraction;
    public dfButton BackButton;

    string OwnName, OpponentName;

    public bool GameStarted;

    public float _Timer
    {
        get
        {
            return timer;
        }
    }

    private float timer = 99.99f;
    public int Timer
    {
        get
        {
            if (timer < 0)
            {
                return 0;
            }
            return (int)timer;
        }
    }

    public float ownHealth
    {
        get
        {
            if (MyRobot != null)
            {
                var h = (MyRobot.HealthScript.Health / MyRobot.HealthScript.MaxHealth) * 100;
                return h;
            }
            return -1;
        }       
    }

    public float opponentHealth
    {
        get
        {
            if (OpponentRobot != null)
            {
                var h = OpponentRobot.HealthScript.MaxHealth - OpponentRobot.HealthScript.Health;
                h = (h / OpponentRobot.HealthScript.MaxHealth) * 100;
                return h;
            }
            return 101;
        }
    }

	// Use this for initialization
	void Start () {
		//BackButton.Click += BackButton_Click;     

		Slot1.Click += Slot1_Click;
		Slot2.Click += Slot2_Click;
		Slot3.Click += new MouseEventHandler(Slot3_Click);
		Slot4.Click += new MouseEventHandler(Slot4_Click);

		activePanel = Slot1.GetComponent<BrainPanelState>();

        countdownLabel = CountdownPanel.transform.Find("Countdown Label").GetComponent<dfLabel>();
        countdownTitle = CountdownPanel.transform.Find("Title Part").GetComponent<dfLabel>();
        countdownFraction = CountdownPanel.transform.Find("Fraction Part").GetComponent<dfLabel>();
        TipLabel = CountdownPanel.transform.Find("Tip Label").GetComponent<dfLabel>();

        if (Random.Range(0, 2) == 0)
        {
            TipLabel.Text = "Use your mouse as an alternative target to guide your robot.";
        }

        StatusLabel.Text = "";
	}

    public void UpdateCountdownPanel(float time)
    {
        if (time < 0)
        {
            time = 0;
        }
        int i = (int)time;
        float frac = time % 1;
        countdownLabel.Text = string.Format("{0}", i);
        countdownFraction.Text = string.Format("{0:.000}", frac);
    }

    public void SetOwnName(string name)
    {
        this.OwnName = name;
        OwnNameLabel.Text = name;
    }

    public void SetDistance(string distance)
    {
        BestOfLabel.Text = distance;
    }

    public void SetOpponentName(string name)
    {
        this.OpponentName = name;
        OpponentNameLabel.Text = name;
    }

    public void SetCountdownVisibility(bool visible)
    {
        if (visible)
        {
           
            CountdownPanel.Show();
            countdownTitle.Text = "Match starts in";
            countdownFraction.Show();
            countdownLabel.Show();
        }
        else
        {
            CountdownPanel.Hide();
            GameStarted = true;
        }
    }

    public void HideMatchStatus()
    {
        CountdownPanel.Hide();
    }

    public void ShowMatchStatus(string status, Color color)
    {
        TipLabel.Hide();
        CountdownPanel.Show();
        StatusLabel.Text = status;
        StatusLabel.Color = color;
        countdownTitle.Text = "";
        countdownFraction.Hide();
        countdownLabel.Hide();
    }

    public void UpdateFrameScore(int ownWins, int oppWins)
    {
        OwnWinsLabel.Text = ownWins + "";
        OpponentWinsLabel.Text = oppWins + "";
    }

	void Slot4_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		PerformSlotClick(4, control as dfPanel);
	}

	void Slot3_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		PerformSlotClick(3, control as dfPanel);
	}

	void Slot2_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		PerformSlotClick(2, control as dfPanel);
	}

	void Slot1_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		PerformSlotClick(1, control as dfPanel);
	}

	void PerformSlotClick(int slot, dfPanel panel)
	{
		if (MyRobot != null)
		{
			activePanel.DeactivatePanel();
			MyRobot.SwitchBrain(slot);

			switch (slot)
			{
				case 1:
					activePanel = Slot1.GetComponent<BrainPanelState>();
					break;
				case 2:
					activePanel = Slot2.GetComponent<BrainPanelState>();
					break;
				case 3:
					activePanel = Slot3.GetComponent<BrainPanelState>();
					break;
				case 4:
					activePanel = Slot4.GetComponent<BrainPanelState>();
					break;
			}
			activePanel.ActivatePanel();
		}
	}

	void BackButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
	{
		PhotonNetwork.Disconnect();
	  //  PhotonNetwork.LeaveLobby();
		Application.LoadLevel("Champions arena");
	}

    public void StopGame()
    {
        GameStarted = false;
    }

    public void ShowMatchResult(int ownWins, int oppWins)
    {
        TipLabel.Hide();
        CountdownPanel.Show();

        if (ownWins > oppWins)
        {
            StatusLabel.Text = string.Format("You win {0} - {1}", ownWins, oppWins);
            StatusLabel.Color = new Color32(0, 255, 0, 255);
        }
        else
        {
            StatusLabel.Text = string.Format("You lose {0} - {1}", ownWins, oppWins);
            StatusLabel.Color = new Color32(255, 0, 0, 255);
        }
        countdownTitle.Text = "";
        countdownFraction.Hide();
        countdownLabel.Hide();

        BackButton.Click += new MouseEventHandler(BackButton_Click);
        BackButton.Show();
    }

    public void SetSelectedBrain(int number)
    {
        switch (number)
        {
            case 1:
                PerformSlotClick(number, Slot1);
                break;
            case 2:
                PerformSlotClick(number, Slot2);
                break;
            case 3:
                PerformSlotClick(number, Slot3);
                break;
            case 4:
                PerformSlotClick(number, Slot4);
                break;
        }
    }

    public void ResetTimer()
    {
        timer = 99.99f;
    }

    public bool TimeOut()
    {
        if (GameStarted)
        {
            return timer <= 0;
        }
        return false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PerformSlotClick(1, Slot1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PerformSlotClick(2, Slot2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PerformSlotClick(3, Slot3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PerformSlotClick(4, Slot4);
        }

        if (GameStarted)
        {
            timer -= Time.deltaTime;
        }
    }
}
