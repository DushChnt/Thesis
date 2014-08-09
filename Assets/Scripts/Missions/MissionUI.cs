using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class MissionUI : MonoBehaviour {

	public BrainPanelState Slot1, Slot2, Slot3, Slot4, activePanel;
    public float WaitingTime = 0.5f;
    public FightController Controller;
    public GameObject Target;
    public dfLabel QuestLabel;

    public Mission1 Mission1;
    public Mission2 Mission2;
    public Mission3 Mission3;
    public Mission4 Mission4;
    public Mission5 Mission5;

    public dfButton StartButton;

    public dfPanel MissionDonePanel;
    public dfPanel MissionDoneLosePanel;

    public dfLabel GoalExplanationLabel, GoalLabel;

    bool initialized;
    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    public delegate void UIEventHandler();
    public event UIEventHandler UIMissionDone;
    public event UIEventHandler UIMissionFailed;
    public event UIEventHandler UIStartPressed;

    protected virtual void OnUIStartPressed()
    {
        if (UIStartPressed != null)
        {
            UIStartPressed();
        }
    }

	// Use this for initialization
	void Start () {
        StartButton.Click += new MouseEventHandler(StartButton_Click);
      //  Camera.main.GetComponent<MousePan>().Activated = false;
        PhotonNetwork.offlineMode = true;
	}

    void StartButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        Initialize();
    }

    void SlotClick(dfControl control, dfMouseEventArgs mouseEvent, int slot)
    {
        activePanel.DeactivatePanel();
        // Switch brain
        Controller.SwitchBrain(slot);
        switch (slot)
        {
            case 1:
                activePanel = Slot1;
                break;
            case 2:
                activePanel = Slot2;
                break;
            case 3:
                activePanel = Slot3;
                break;
            case 4:
                activePanel = Slot4;
                break;
        }
        activePanel.ActivatePanel();
    }

    void Initialize()
    {
       // Camera.main.GetComponent<MousePan>().Activated = true;
        Slot1.ActivatePanel();
        activePanel = Slot1;

        if (Slot1.InUse) Slot1.GetPanel().Click += new MouseEventHandler((control, mouseEvent) => SlotClick(control, mouseEvent, 1));
        if (Slot2.InUse) Slot2.GetPanel().Click += new MouseEventHandler((control, mouseEvent) => SlotClick(control, mouseEvent, 2));
        if (Slot3.InUse) Slot3.GetPanel().Click += new MouseEventHandler((control, mouseEvent) => SlotClick(control, mouseEvent, 3));
        if (Slot4.InUse) Slot4.GetPanel().Click += new MouseEventHandler((control, mouseEvent) => SlotClick(control, mouseEvent, 4));

        IBlackBox brain1 = null, brain2 = null, brain3 = null, brain4 = null, activeBrain = null;
        string path = "";
        if (Player.Brain1 != null)
        {
            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Player.Username, Player.Brain1.ObjectId);
            print("Path: " + path);
            brain1 = Utility.LoadBrain(path);
            activeBrain = brain1;
        }
        if (Player.Brain2 != null)
        {
            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Player.Username, Player.Brain2.ObjectId);
            brain2 = Utility.LoadBrain(path);
            if (activeBrain == null)
            {
                activeBrain = brain2;
            }
        }

        if (Player.Brain3 != null)
        {
            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Player.Username, Player.Brain3.ObjectId);
            brain3 = Utility.LoadBrain(path);
            if (activeBrain == null)
            {
                activeBrain = brain3;
            }
        }

        if (Player.Brain4 != null)
        {
            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Player.Username, Player.Brain4.ObjectId);
            brain4 = Utility.LoadBrain(path);
            if (activeBrain == null)
            {
                activeBrain = brain4;
            }
        }


        Controller.Activate(activeBrain, Target);
        Controller.SetBrains(brain1, brain2, brain3, brain4);

        print("Current mission: " + PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1));

        switch (PlayerPrefs.GetInt(MissionPanel.CURRENT_MISSION, 1))
        {
            case 1:


                Mission1.Initialize(this);
                break;
            case 2:

                Mission2.Initialize(this);
                break;
            case 3:
                Mission3.Initialize(this);
                break;

            case 4:
                Mission4.Initialize(this);
                break;

            case 5:
                Mission5.Initialize(this);
                break;
        }

        OnUIStartPressed();
    }

    public void UpdateQuest(int quest, int maxQuest)
    {
        QuestLabel.Text = string.Format("{0} / {1}", quest, maxQuest);
    }

    public void SetGoalText(string goal)
    {
        this.GoalLabel.Text = goal;
    }

    public void SetGoalText(string goalExplanation, string goal)
    {
        this.GoalExplanationLabel.Text = goalExplanation;
        this.GoalLabel.Text = goal;
    }

    public void MissionDone()
    {
        MissionDone(true);
    }

    public void MissionDone(bool win)
    {
        StartCoroutine(missionDone(win));
    }

    public IEnumerator missionDone(bool win)
    {
        yield return new WaitForSeconds(2);
        if (win)
        {
            MissionDonePanel.Show();
        }
        else
        {
            MissionDoneLosePanel.Show();
        }
       // Camera.main.GetComponent<MousePan>().Activated = false;
    }

	// Update is called once per frame
	void Update () {

        //if (!initialized && WaitingTime < 0)
        //{
        //    initialized = true;
        //    Initialize();
        //}
        //else
        //{
        //    WaitingTime -= Time.deltaTime;
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            Slot1.GetPanel().DoClick();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            Slot2.GetPanel().DoClick();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            Slot3.GetPanel().DoClick();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            Slot4.GetPanel().DoClick();
        }

	}
}
