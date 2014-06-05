using UnityEngine;
using System.Collections;
using Parse;
using SharpNeat.Phenomes;

public class NetworkBattleManager : Photon.MonoBehaviour
{

    bool ReadyToStart, GameStarted, ShowingCountdown;
    public float StartTimer = 3f;
    private float _startTimer = 0;
    public Material ownColor;
    NetworkGUI gui;


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
        gui = GameObject.Find("_GUI").GetComponent<NetworkGUI>();
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("TEST 0.1");

    }

    void Update()
    {
        //if (ReadyToStart)
        //{
        //    _startTimer += Time.deltaTime;
        //    if (_startTimer > StartTimer)
        //    {
        //        print("Go!");
        //        ReadyToStart = false;
        //        photonView.RPC("StartGame", PhotonTargets.AllBuffered);
        //    }
        //}

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

                    print("RPC call start game");
                    photonView.RPC("StartGame", PhotonTargets.All);
                    GameStarted = true;
                }
            }
        }
        if (ShowingCountdown)
        {
            _startTimer += Time.deltaTime;
            gui.UpdateCountdownPanel(StartTimer - _startTimer);
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PhotonNetwork.room.open = false;
        PhotonNetwork.room.visible = false;
    }   

    //void OnGUI()
    //{
    //    GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    //}

    //void OnJoinedLobby()
    //{
    //    PhotonNetwork.JoinRandomRoom();
    //}

    //void OnPhotonRandomJoinFailed()
    //{
    //    print("FailedW");
    //    //PhotonNetwork.CreateRoom(null);
    //    RoomOptions rOpt = new RoomOptions();
    //    rOpt.isOpen = true;
    //    rOpt.isVisible = true;
    //    rOpt.maxPlayers = 2;
    //    TypedLobby tl = new TypedLobby();

    //    PhotonNetwork.CreateRoom(null, rOpt, tl);
    //}

    //void OnJoinedRoom()
    //{
    //    print("Joined");

    //    SpawnMyPlayer();
    //}

    void SpawnMyPlayer()
    {
        float x = Random.Range(-10, 10);
        float z = Random.Range(-10, 10);
        GameObject player = PhotonNetwork.Instantiate("ModRobot", new Vector3(x, 1, z), Quaternion.identity, 0);
        // NetworkGUI.MyRobot = player.GetComponent<BattleController>();



        // Make MyRobot red or something
        //  player.transform.Find("Body").renderer.material = ownColor;

    }

    [RPC]
    protected void ShowCountdown()
    {
        ShowingCountdown = true;
        gui.SetCountdownVisibility(true);
        SpawnMyPlayer();
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
            if (!ph.gameObject.name.Contains("ModRobot"))
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
            var controller1 = mine.GetComponent<BattleController>();
            controller1.HitLayers = 1 << LayerMask.NameToLayer("BattleRobot");

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


            controller1.Activate(activeBrain, other);
            controller1.SetBrains(brain1, brain2, brain3, brain4);


            gui.MyRobot = controller1;
            gui.OpponentRobot = controller1.Opponent;
            controller1.Opponent.Opponent = controller1;
            controller1.Opponent.target = controller1.gameObject;

            GameObject UIRoot = GameObject.Find("UI Root");

            HealthScript health = mine.GetComponent<HealthScript>();

            GameObject HealthBar = Instantiate(Resources.Load("HealthBar")) as GameObject;
            dfFollowObject Follow = HealthBar.GetComponent<dfFollowObject>();
            Follow.attach = mine;
            HealthBar.transform.parent = UIRoot.transform;
            Follow.enabled = true;
            health.FollowScript = Follow;

            dfPropertyBinding prop = HealthBar.GetComponent<dfPropertyBinding>();
            dfComponentMemberInfo info = new dfComponentMemberInfo();
            info.Component = health;
            info.MemberName = "Health";
            prop.DataSource = info;

            dfComponentMemberInfo targetInfo = new dfComponentMemberInfo();
            targetInfo.Component = HealthBar.GetComponent<dfProgressBar>();
            targetInfo.MemberName = "Value";

            prop.DataTarget = targetInfo;

            prop.Unbind();
            prop.Bind();

            prop.enabled = true;

            HealthScript oppHealth = other.GetComponent<HealthScript>();

            GameObject OppHealthBar = Instantiate(Resources.Load("HealthBar")) as GameObject;
            dfFollowObject OppFollow = OppHealthBar.GetComponent<dfFollowObject>();
            OppFollow.attach = other;
            OppHealthBar.transform.parent = UIRoot.transform;
            OppFollow.enabled = true;
            OppHealthBar.transform.Find("Me Label").GetComponent<dfLabel>().Hide();
            oppHealth.FollowScript = OppFollow;

            dfPropertyBinding oppProp = OppHealthBar.GetComponent<dfPropertyBinding>();
            dfComponentMemberInfo oppInfo = new dfComponentMemberInfo();
            oppInfo.Component = oppHealth;
            oppInfo.MemberName = "Health";
            oppProp.DataSource = oppInfo;

            dfComponentMemberInfo oppTargetInfo = new dfComponentMemberInfo();
            oppTargetInfo.Component = OppHealthBar.GetComponent<dfProgressBar>();
            oppTargetInfo.MemberName = "Value";

            oppProp.DataTarget = oppTargetInfo;

            oppProp.Unbind();
            oppProp.Bind();

            oppProp.enabled = true;


            if (PhotonNetwork.isMasterClient)
            {
                mine.transform.Find("Body").renderer.material = ownColor;
            }
            else
            {
                other.transform.Find("Body").renderer.material = ownColor;
            }
        }
    }

    //void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    //{
    //    print("OnPhotonPlayerConnected. ID: " + newPlayer.ID + ", Name: " + newPlayer.name);

    //    GameObject other = null;
    //    PhotonView[] players = GameObject.FindObjectsOfType<PhotonView>();

    //    ReadyToStart = true;

    //    print("Number of PhotonViews: " + players.Length);
    //    foreach (PhotonView ph in players)
    //    {
    //        print("ViewID: " + ph.viewID + ", SubID: " + ph.subId + ", InstantiationId: " + ph.instantiationId + ", Name: " + ph.name);
    //        if (!ph.isMine)
    //        {
    //            other = ph.gameObject;
    //            // break;
    //            print("Is not mine");
    //        }
    //        else
    //        {
    //            print("Is Mine");
    //        }
    //    }
    //    if (other != null)
    //    {
    //        print("JOE");
    //    }

    //}
}
