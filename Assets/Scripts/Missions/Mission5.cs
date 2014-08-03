using UnityEngine;
using System.Collections;

public class Mission5 : MonoBehaviour {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 2;
    Mission5State state;
    MissionUI UI;
    int mortarHits;

    public PopupText PopupText;

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        
        StartBeatIt();
    }

    

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {
        PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, 5);
    }    

    private void StartBeatIt()
    {
        Opponent.SwitchMovement(OpponentState.MortarFiring);
        PopupText.ShowText("Beat it!", 1);
        state = Mission5State.BeatIt;
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
            case Mission5State.BeatIt:
                CheckBeatIt();
                break;
        }
    }
    
    private void CheckBeatIt()
    {
        if (UI != null)
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
    }

    private void DoneWin()
    {
        state = Mission5State.Done;
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
        state = Mission5State.Done;
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

    enum Mission5State
    {
        BeatIt, Done
    }
}
