using UnityEngine;
using System.Collections;

public class Mission2 : MonoBehaviour, IMission {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 2;
    Mission2State state;
    MissionUI UI;

    int meleeHits;

    public PopupText PopupText;

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        Controller.MeleeHitEvent += new FightController.AttackEventHandler(Controller_MeleeHit);
        StartHit5();
    }

    void Controller_MeleeHit()
    {
        if (state.Equals(Mission2State.Hit5))
        {
            meleeHits++;
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

    private void StartHit5()
    {
        Opponent.SwitchMovement(OpponentState.CircleMovement);
        PopupText.ShowText("3x hits!", 1);
        state = Mission2State.Hit5;
        UI.UpdateQuest(1, MaxQuest);
        Controller.DummyAttack = true;
    }

    private void StartBeatIt()
    {
        Opponent.SwitchMovement(OpponentState.MeleeAttack);
        PopupText.ShowText("Beat it!", 1);
        state = Mission2State.BeatIt;
        UI.UpdateQuest(2, MaxQuest);
        Controller.DummyAttack = false;
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
