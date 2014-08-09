using UnityEngine;
using System.Collections;
using Parse;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class NetworkBattleManager : Photon.MonoBehaviour
{

    bool ReadyToStart, GameStarted, ShowingCountdown;
    public float StartTimer = 3f;
    private float _startTimer = 0;
    public Material ownColor;
    NetworkGUI gui;
    public Transform Spawn1, Spawn2, Spawn3, Spawn4;
    bool opponentDied, iDied;
    float waitForEndTimer, WaitForEndTime = 2f;
    Match match;
    Frame currentFrame;
    IList<ParseObject> frames;
    EventLogger logger;
    int distance;

    float NextFrameTimer = 2.0f, _nextFrameTimer;
    bool waitForNextFrame;

    int ownWins, opponentWins;

    public dfProgressBar PlayerHealthbar, OpponentHealthbar;

    List<int> occupiedSpawnPositions;

    Transform chosenSpawnPosition;

    Player Player
    {
        get
        {
            return ParseUser.CurrentUser as Player;
        }
    }

    // Use this for initialization
    void Start()
    {
        
     //   Connect();
        gui = GameObject.Find("Battle GUI").GetComponent<NetworkGUI>();
        gui.SetOwnName(Player.Username);

        switch (PhotonNetwork.room.customProperties["mode"].ToString())
        {
            case "Single":
                distance = 1;
                break;
            case "Best of 3":
                distance = 3;
                break;
            case "Best of 5":
                distance = 5;
                break;
        }
        print("Distance: " + distance);
        frames = new List<ParseObject>();        
        match = new Match();


        StartFrame();
  
    }

    void StartFrame()
    {
        _startTimer = 0;
        GameStarted = false;
        currentFrame = new Frame();
        frames.Add(currentFrame);

        match["frames"] = frames;
        match.SaveAsync().ContinueWith(t =>
        {
            currentFrame["match"] = match;
            currentFrame.SaveAsync();
        });

        occupiedSpawnPositions = new List<int>();
        if (PhotonNetwork.isMasterClient)
        {
            ChooseSpawn();
        }
    }

    void SaveSequentially()
    {
        currentFrame.SaveAsync().ContinueWith( t => {
            print("Done saving current frame");  
        });
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("TEST 0.1");

    }

    void Update()
    {
        if (logger != null)
        {
            logger.UpdateTime(Time.unscaledDeltaTime);
        }

        if (waitForNextFrame)
        {
            _nextFrameTimer += Time.deltaTime;
            if (_nextFrameTimer > NextFrameTimer)
            {
                if (PhotonNetwork.isMasterClient)
                {
                    PhotonNetwork.DestroyAll();
                }
                waitForNextFrame = false;
                StartFrame();
            }
        }

        if (!GameStarted && PhotonNetwork.isMasterClient)
        {
            if (PhotonNetwork.room.playerCount == 2)
            {
                if (!ShowingCountdown)
                {
                    photonView.RPC("ShowCountdown", PhotonTargets.All);
                }

                if (_startTimer > StartTimer)
                {
                    GameStarted = true;
                    print("RPC call start game");
                    photonView.RPC("StartGame", PhotonTargets.All);
                    
                    Camera.main.GetComponent<MousePan>().Activated = true;
                    
                }
            }
        }
        if (ShowingCountdown)
        {
            _startTimer += Time.deltaTime;
            gui.UpdateCountdownPanel(StartTimer - _startTimer);
        }

        if (opponentDied || iDied)
        {
            waitForEndTimer += Time.deltaTime;
            if (waitForEndTimer > WaitForEndTime)
            {
                // Match is officially over, calculate winner and loser
                CalculateWinner();
            }
        }
    }

    void CalculateWinner()
    {
        string outcome = "draw";
        if ((iDied && opponentDied) || (!iDied && !opponentDied)) 
        {
            // It's a draw!
            print("It's a draw");
            gui.ShowMatchStatus("It's a draw!", Color.white);
            outcome = "draw";
        }
        if (iDied && !opponentDied)
        {
            // I lost!
            print("I lost");
            gui.ShowMatchStatus("You lose...", Color.red);
            outcome = "lost";
            opponentWins++;
        }
        if (!iDied && opponentDied)
        {
            // I won!
            print("I won");
            gui.ShowMatchStatus("You win!", Color.green);
            outcome = "won";
            ownWins++;
        }
        if (logger != null)
        {
            logger.StopLogging(outcome);
        }
        //currentFrame.Outcome = outcome;

        gui.UpdateFrameScore(ownWins, opponentWins);
        //match.Won = !iDied;
        
        iDied = false;
        opponentDied = false;
        gui.StopGame();

        NextFrame();
    }

    private void NextFrame()
    {
        if (ownWins > distance / 2.0f || opponentWins > distance / 2.0f)
        {

        }
        else
        {
            _nextFrameTimer = 0;
            waitForNextFrame = true;
        }
    }

    [RPC]
    protected void UpdateFrameScore(string masterOutcome)
    {
        switch (masterOutcome)
        {
            case "won":
                if (PhotonNetwork.isMasterClient)
                {
                    ownWins++;
                }
                else
                {
                    opponentWins++;
                }
                break;
            case "lost":
                if (PhotonNetwork.isMasterClient)
                {
                    opponentWins++;
                }
                else
                {
                    ownWins++;
                }
                break;
            case "draw":

                break;               
        }
        gui.UpdateFrameScore(ownWins, opponentWins);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PhotonNetwork.room.open = false;
        PhotonNetwork.room.visible = false;
    }   

    void SpawnMyPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("HammerRobot", chosenSpawnPosition.position, chosenSpawnPosition.rotation, 0);
    }

    protected void ChooseSpawn()
    {
        int chosen = Random.Range(0, 3);
        while (occupiedSpawnPositions.Contains(chosen))
        {
            chosen = Random.Range(0, 3);
        }
        occupiedSpawnPositions.Add(chosen);

        switch (chosen)
        {
            case 0:
                chosenSpawnPosition = Spawn1;
                break;
            case 1:
                chosenSpawnPosition = Spawn2;
                break;
            case 2:
                chosenSpawnPosition = Spawn3;
                break;
            case 3:
                chosenSpawnPosition = Spawn4;
                break;
        }
    }

    [RPC]
    protected void ReceiveSpawnPosition(int pos)
    {
        switch (pos)
        {
            case 0:
                chosenSpawnPosition = Spawn1;
                break;
            case 1:
                chosenSpawnPosition = Spawn2;
                break;
            case 2:
                chosenSpawnPosition = Spawn3;
                break;
            case 3:
                chosenSpawnPosition = Spawn4;
                break;
        }
    }

    [RPC]
    protected void ShowCountdown()
    {
        ShowingCountdown = true;
        photonView.RPC("SendInfo", PhotonTargets.OthersBuffered, Player.Username, Player.ObjectId);
        gui.SetCountdownVisibility(true);
        
        SpawnMyPlayer();
    }

    [RPC]
    protected void SendInfo(string oppName, string oppId)
    {
        gui.SetOpponentName(oppName);
        Player opp = Player.CreateWithoutData<Player>(oppId);       
        match.Opponent = opp;
    }

    [RPC]
    protected void StartGame()
    {
        gui.SetCountdownVisibility(false);
        ShowingCountdown = false;
        print("Starting game");
        // if (photonView.isMine)
        {
            Stats();
        }
    }



    public void Stats()
    {
       
        Player Player = ParseUser.CurrentUser as Player;
        PhotonView[] players = GameObject.FindObjectsOfType<PhotonView>();
        GameObject other = null;
        GameObject mine = null;
        print("Number of PhotonViews: " + players.Length);
        foreach (PhotonView ph in players)
        {
            if (!ph.gameObject.name.Contains("HammerRobot"))
            {
                continue;
            }
            print("ViewID: " + ph.viewID + ", SubID: " + ph.subId + ", InstantiationId: " + ph.instantiationId + ", Name: " + ph.name);
            if (!ph.isMine)
            {
                other = ph.gameObject;
                print("Is not mine");
            }
            else
            {
                print("Is Mine");
                mine = ph.gameObject;
            }
        }
        if (other != null)
        {
            //RobotController rc = mine.GetComponent<RobotController>();

            //Destroy(rc);
            // rc.HumanActivate(other);
            //mine.AddComponent<BattleController>();
            //  var brain1 = Utility.LoadBrain(Application.persistentDataPath + string.Format("/Populations/{0}Champ.gnm.xml", "Mortar Precision"));
            var controller1 = mine.GetComponent<FightController>();
       //     controller1.HitLayers = 1 << LayerMask.NameToLayer("BattleRobot");

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

            print("Joe 1");
            controller1.Activate(activeBrain, other);
            controller1.SetBrains(brain1, brain2, brain3, brain4);
            print("Joe 2");
            var opponent = other.GetComponent<FightController>();
            gui.ResetTimer();
            gui.MyRobot = controller1;
            gui.OpponentRobot = opponent;
            opponent.Opponent = controller1;
          //  controller1.Opponent.Opponent = controller1;
          //  controller1.Opponent.target = controller1.gameObject;
            gui.ResetButton.Controller = controller1.gameObject;
            print("Joe 3");

            GameObject UIRoot = GameObject.Find("Battle GUI");

       //     UIRoot.GetComponent<MouseTotem>().SetController(controller1);

            HealthScript health = mine.GetComponent<HealthScript>();
            health.Died += new HealthScript.DeathEventHandler(health_Died);

            GameObject HealthBar = Instantiate(Resources.Load("HealthBar")) as GameObject;

            dfFollowObject Follow = HealthBar.GetComponent<dfFollowObject>();
            Follow.attach = mine;
            HealthBar.transform.parent = UIRoot.transform;
            Follow.enabled = true;
            health.FollowScript = Follow;

            dfPropertyBinding prop = HealthBar.GetComponent<dfPropertyBinding>();
            dfComponentMemberInfo info = new dfComponentMemberInfo();
            info.Component = health;
            info.MemberName = "HealthPercentage";
            prop.DataSource = info;

            dfComponentMemberInfo targetInfo = new dfComponentMemberInfo();
            targetInfo.Component = HealthBar.GetComponent<dfProgressBar>();
            targetInfo.MemberName = "Value";

            prop.DataTarget = targetInfo;

            prop.Unbind();
            prop.Bind();

            prop.enabled = true;

            print("Joe 4");

            HealthScript oppHealth = other.GetComponent<HealthScript>();
            oppHealth.Died += new HealthScript.DeathEventHandler(oppHealth_Died);

            GameObject OppHealthBar = Instantiate(Resources.Load("OppHealthBar")) as GameObject;
            dfFollowObject OppFollow = OppHealthBar.GetComponent<dfFollowObject>();
            OppFollow.attach = other;
            OppHealthBar.transform.parent = UIRoot.transform;
            OppFollow.enabled = true;
           // OppHealthBar.transform.Find("Me Label").GetComponent<dfLabel>().Hide();
            oppHealth.FollowScript = OppFollow;

            dfPropertyBinding oppProp = OppHealthBar.GetComponent<dfPropertyBinding>();
            dfComponentMemberInfo oppInfo = new dfComponentMemberInfo();
            oppInfo.Component = oppHealth;
            oppInfo.MemberName = "HealthPercentage";
            oppProp.DataSource = oppInfo;

            dfComponentMemberInfo oppTargetInfo = new dfComponentMemberInfo();
            oppTargetInfo.Component = OppHealthBar.GetComponent<dfProgressBar>();
            oppTargetInfo.MemberName = "Value";

            oppProp.DataTarget = oppTargetInfo;

            oppProp.Unbind();
            oppProp.Bind();

            oppProp.enabled = true;

            print("Joe 5");


            if (PhotonNetwork.isMasterClient)
            {
                mine.transform.Find("Body").renderer.material = ownColor;
            }
            else
            {
                other.transform.Find("Body").renderer.material = ownColor;
            }

            print("Joe 6");
         //   GameLogger.Instance.StartLogging(controller1, controller1.Opponent);
            logger = new EventLogger(controller1, Application.loadedLevelName, currentFrame);
            logger.StartLogging();
        }
    }

    void oppHealth_Died(object sender, System.EventArgs e)
    {
        // Wait for two seconds to see if our own robot dies as well (from mortars)
        opponentDied = true;
    }

    void health_Died(object sender, System.EventArgs e)
    {
        // Wait for two seconds to see if the other robot dies as well (from mortars)
        iDied = true;
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        // Send spawn position to new player
        int chosen = Random.Range(0, 3);
        while (occupiedSpawnPositions.Contains(chosen))
        {
            chosen = Random.Range(0, 3);
        }
        occupiedSpawnPositions.Add(chosen);

        photonView.RPC("ReceiveSpawnPosition", newPlayer, chosen);
    }

    void OnDestroy()
    {
        if (logger != null)
        {
            logger.StopLogging("quit");
        }
    }
}
