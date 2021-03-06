﻿using UnityEngine;
using System.Collections;

public class Mission2 : MonoBehaviour, IMission {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 2;
    Mission2State state;
    MissionUI UI;

    int meleeHits;

    public PopupText PopupText;

    void Awake()
    {
        PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, 2);
    }

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        Controller.MeleeHitEvent += new FightController.AttackEventHandler(Controller_MeleeHit);
        StartHit3();
    }

    void Controller_MeleeHit()
    {
        if (state.Equals(Mission2State.Hit5))
        {
            meleeHits++;
            UI.SetGoalText(string.Format("{0} / {1}", meleeHits, 3));
        }
    }

   

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        CheckConditions();
	}

    private void CheckConditions()
    {
        switch (state)
        {
            case Mission2State.Hit5:
                CheckHit5();
                break;
            case Mission2State.BeatIt:
                CheckBeatIt();
                break;            
        }
    }

    private void StartHit3()
    {
        Opponent.SwitchMovement(OpponentState.CircleMovement);
        PopupText.ShowText("3x hits!", 1);
        state = Mission2State.Hit5;
        UI.UpdateQuest(1, MaxQuest);
        Controller.DummyAttack = true;
        UI.SetGoalText("Hit target with melee weapon", string.Format("{0} / {1}", meleeHits, 3));
    }

    private void StartBeatIt()
    {
        Opponent.SwitchMovement(OpponentState.MeleeAttack);
        PopupText.ShowText("Beat it!", 1);
        state = Mission2State.BeatIt;
        UI.UpdateQuest(2, MaxQuest);
        Controller.DummyAttack = false;
        UI.SetGoalText("Beat opponent", string.Format("Health remaining: {0:0.00}", Controller.OpponentHealth.Health > 0 ? Controller.OpponentHealth.Health : 0));
    }

    private void CheckHit5()
    {
        if (meleeHits >= 3)
        {
            StartBeatIt();
        }
    }

    private void CheckBeatIt()
    {
        UI.SetGoalText(string.Format("Health remaining: {0:0.00}", Controller.OpponentHealth.Health > 0 ? Controller.OpponentHealth.Health : 0));
        if (Controller.OpponentHealth.IsDead)
        {
            // You win!
            DoneWin(); 
        }
        else if (Opponent.OpponentHealth.IsDead)
        {
            // You lose!
            DoneLose();
        }
    }

    private void DoneWin()
    {
        state = Mission2State.Done;
        if (Controller != null)
        {
            Controller.Stop();
        }
        if (Opponent != null)
        {
            Opponent.Stop();
        }
        UI.MissionDone(true);
    }

    private void DoneLose()
    {
        state = Mission2State.Done;
        if (Controller != null)
        {
            Controller.Stop();
        }
        if (Opponent != null)
        {
            Opponent.Stop();
        }
        UI.MissionDone(false);
    }

    enum Mission2State
    {
        Hit5, BeatIt, Done
    }
}
