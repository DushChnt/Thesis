using UnityEngine;
using System.Collections;

public class Mission4 : MonoBehaviour {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 2;
    Mission4State state;
    MissionUI UI;
    int mortarHits;

    public PopupText PopupText;

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        Controller.MortarHitEvent += new FightController.AttackEventHandler(Controller_MortarHitEvent);
        StartHit3();
       // StartBeatIt();
    }

    void Controller_MortarHitEvent()
    {
        if (state.Equals(Mission4State.Hit3))
        {
            mortarHits++;
            UI.SetGoalText(string.Format("{0} / {1}", mortarHits, 3));
        }
    }

	// Use this for initialization
	void Start () {
	
	}

    void Awake()
    {
        PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, 4);
    }

    private void StartHit3()
    {
        Opponent.SwitchMovement(OpponentState.CircleMovement);
        PopupText.ShowText("3x bombs!", 1);
        state = Mission4State.Hit3;
        UI.UpdateQuest(1, MaxQuest);
        Controller.DummyAttack = true;
        UI.SetGoalText("Hit target with mortar", string.Format("{0} / {1}", mortarHits, 3));
    }

    private void StartBeatIt()
    {
        Opponent.SwitchMovement(OpponentState.MortarFiring);
        PopupText.ShowText("Beat it!", 1);
        state = Mission4State.BeatIt;
        UI.UpdateQuest(2, MaxQuest);
        Controller.DummyAttack = false;
        UI.SetGoalText("Beat opponent", string.Format("Health remaining: {0:0.00}", Controller.OpponentHealth.Health > 0 ? Controller.OpponentHealth.Health : 0));
    }

    void Update()
    {
        CheckConditions();
    }

    private void CheckConditions()
    {
        switch (state)
        {
            case Mission4State.Hit3:
                CheckHit3();
                break;            
            case Mission4State.BeatIt:
                CheckBeatIt();
                break;
        }
    }

    private void CheckHit3()
    {
        if (mortarHits >= 3)
        {
            StartBeatIt();
        }
    }

    private void CheckBeatIt()
    {
        UI.SetGoalText(string.Format("Health remaining: {0:0.00}", Controller.OpponentHealth.Health > 0 ? Controller.OpponentHealth.Health : 0));
        if (Opponent.OpponentHealth.IsDead)
        {
            // You lose!
            DoneLose();
        }
        else if (Controller.OpponentHealth.IsDead)
        {
            // You win!
            DoneWin();
        }

    }

    private void DoneWin()
    {
        state = Mission4State.Done;
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
        state = Mission4State.Done;
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

    enum Mission4State
    {
        Hit3, BeatIt, Done
    }
}
