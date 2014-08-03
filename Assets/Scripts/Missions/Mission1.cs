using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Mission1 : MonoBehaviour, IMission {

    public FightController Controller;
    public OpponentController Opponent;
    int MaxQuest = 3;
    Mission1State state;
    MissionUI UI;

    Queue<float> PastDistances;

    public PopupText PopupText;

	// Use this for initialization
	void Start () {
        
        
	}

    void Awake()
    {
        PlayerPrefs.SetInt(MissionPanel.CURRENT_MISSION, 1);
    }

    public void Initialize(MissionUI ui)
    {
        this.UI = ui;
        StartApproach();
        Controller.DummyAttack = true;
    }

	// Update is called once per frame
	void Update () {
        
        CheckConditions();
	}

    private void CheckConditions()
    {
        switch (state)
        {
            case Mission1State.Approach:
                CheckApproach();
                break;
            case Mission1State.Flee:
                CheckFlee();
                break;
            case Mission1State.Follow:
                CheckFollow();
                break;
        }
    }

    private void CheckApproach()
    {
        
        float dist = GetDistance();
        
        if (dist >= 0)
        {
            UI.SetGoalText(string.Format("Distance remaining: {0:0.00}", GetDistance() - 2.5f));
            if (dist < 2.5f)
            {
                print("Approach done!");
                StartFlee();
                
            }
        }
    }

    private void Done()
    {
        state = Mission1State.Done;
        Controller.Stop();
        Opponent.Stop();
     //   PopupText.ShowText("You win!");
        UI.MissionDone();
    }

    private void CheckFollow()
    {
        float dist = GetDistance();

        if (dist >= 0)
        {
            UI.SetGoalText(string.Format("Distance remaining: {0:0.00}", GetDistance() - 2.5f));
            if (dist < 2.5f)
            {                
                Done();

            }
        }
    }

    private void StartApproach()
    {
        print("Showing text");
        PopupText.ShowText("Get close!");
        state = Mission1State.Approach;
        UI.UpdateQuest(1, MaxQuest);
        UI.SetGoalText("Approach the target", string.Format("Distance remaining: {0:0.00}", GetDistance() - 2.5f));
    }

    private void StartFlee()
    {
        state = Mission1State.Flee;
        Opponent.SwitchMovement(OpponentState.Approaching);
        PastDistances = new Queue<float>(100);
        PopupText.ShowText("Flee!");
        UI.UpdateQuest(2, MaxQuest);
        UI.SetGoalText("Flee from the target", string.Format("Distance remaining: {0:0.00}", 5f - PastDistances.Average()));
    }

    private void StartFollow()
    {
        state = Mission1State.Follow;
        Opponent.SwitchMovement(OpponentState.Fleeing);
        PopupText.ShowText("Catch him!");
        UI.UpdateQuest(3, MaxQuest);
        UI.SetGoalText("Catch the target", string.Format("Distance remaining: {0:0.00}", GetDistance() - 2.5f));
    }

    private void CheckFlee()
    {
        float dist = GetDistance();
        PastDistances.Enqueue(dist);
        if (PastDistances.Count >= 100)
        {
            PastDistances.Dequeue();
        }
        float avg = PastDistances.Average();
        UI.SetGoalText("Flee from the target", string.Format("Distance remaining: {0:0.00}", 5f - avg));
      //  print("Average: " + avg);
        if (avg > 5)
        {
            StartFollow();
        }
    }

    private float GetDistance()
    {
        if (Opponent != null)
        {
            Vector2 a = new Vector2(Opponent.transform.position.x, Opponent.transform.position.z);
            Vector2 b = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
            return Vector2.Distance(a, b);
        }
        return -1f;
    }

    enum Mission1State
    {
        Approach,
        Flee,
        Follow,
        Done
    }
}
