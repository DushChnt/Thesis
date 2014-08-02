using UnityEngine;
using System.Collections;

public class Mission3 : MonoBehaviour {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 3;
    Mission3State state;
    MissionUI UI;
    int rangedHits;

    public PopupText PopupText;

    void Awake()
    {
        PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, 3);
    }

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        Controller.RangedHitEvent += new FightController.AttackEventHandler(Controller_RangedHitEvent);
        StartShoot5();        
    }

    void Controller_RangedHitEvent()
    {
        rangedHits++;
        if (state.Equals(Mission3State.Shoot5) || state.Equals(Mission3State.CircleShoot5))
        {
            UI.SetGoalText(string.Format("{0} / {1}", rangedHits, 5));
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        CheckConditions();
    }

    private void CheckConditions()
    {
        switch (state)
        {
            case Mission3State.Shoot5:
                CheckShoot5();
                break;
            case Mission3State.CircleShoot5:
                CheckCircleShoot5();
                break;
            case Mission3State.BeatIt:
                CheckBeatIt();
                break;
        }
    }

    private void StartShoot5()
    {
        rangedHits = 0;
        Opponent.SwitchMovement(OpponentState.Stationary);
        PopupText.ShowText("5x hits!", 1);
        state = Mission3State.Shoot5;
        UI.UpdateQuest(1, MaxQuest);
        Controller.DummyAttack = true;
        UI.SetGoalText("Hit target with ranged weapon", string.Format("{0} / {1}", rangedHits, 5));
    }

    private void StartCircleShoot5()
    {
        rangedHits = 0;
        Opponent.SwitchMovement(OpponentState.CircleMovement);
        PopupText.ShowText("Bull's eye!", 1);
        state = Mission3State.CircleShoot5;
        UI.UpdateQuest(2, MaxQuest);
        Controller.DummyAttack = true;
        UI.SetGoalText("Hit moving target with ranged weapon", string.Format("{0} / {1}", rangedHits, 5));
    }

    private void StartBeatIt()
    {
        Opponent.SwitchMovement(OpponentState.BackwardsFiring);
        PopupText.ShowText("Beat it!", 1);
        state = Mission3State.BeatIt;
        UI.UpdateQuest(3, MaxQuest);
        Controller.DummyAttack = false;
        UI.SetGoalText("Beat opponent", string.Format("Health remaining: {0:0.00}", Controller.OpponentHealth.Health > 0 ? Controller.OpponentHealth.Health : 0));
    }

    private void CheckShoot5()
    {
        if (rangedHits >= 5)
        {
            StartCircleShoot5();
        }
    }

    private void CheckCircleShoot5()
    {
        if (rangedHits >= 5)
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
        state = Mission3State.Done;
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
        state = Mission3State.Done;
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

    enum Mission3State
    {
        Shoot5, CircleShoot5, BeatIt, Done
    }
}
