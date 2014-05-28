using UnityEngine;
using System.Collections;
using Parse;

public class NetworkManager : Photon.MonoBehaviour {

    bool ReadyToStart;
    public float StartTimer = 2f;
    private float _startTimer = 0;

    // Use this for initialization
    void Start()
    {
        Connect();
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
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        print("FailedW");
        //PhotonNetwork.CreateRoom(null);
        RoomOptions rOpt = new RoomOptions();
        rOpt.isOpen = true;
        rOpt.isVisible = true;
        rOpt.maxPlayers = 2;
        TypedLobby tl = new TypedLobby();

        PhotonNetwork.CreateRoom(null, rOpt, tl);
    }

    void OnJoinedRoom()
    {
        print("Joined");

        SpawnMyPlayer();
    }

    void SpawnMyPlayer()
    {
        float x = Random.Range(-10, 10);
        float z = Random.Range(-10, 10);
        GameObject player = PhotonNetwork.Instantiate("ModRobot", new Vector3(x, 1, z), Quaternion.identity, 0);
    }

    [RPC]
    protected void StartGame()
    {
        print("Starting game");
        if (photonView.isMine)
        {
            Stats();
        }
    }

    public static void Stats()
    {
        
        PhotonView[] players = GameObject.FindObjectsOfType<PhotonView>();
        GameObject other = null;
        GameObject mine = null;
        print("Number of PhotonViews: " + players.Length);
        foreach (PhotonView ph in players)
        {
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
            string path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, ParseUser.CurrentUser.Get<string>("slot1"));
            print("Path: " + path);
            var brain1 = Utility.LoadBrain(path);

            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, ParseUser.CurrentUser.Get<string>("slot2"));
            var brain2 = Utility.LoadBrain(path);

            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, ParseUser.CurrentUser.Get<string>("slot3"));
            var brain3 = Utility.LoadBrain(path);

            path = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", ParseUser.CurrentUser.Username, ParseUser.CurrentUser.Get<string>("slot4"));
            var brain4 = Utility.LoadBrain(path);

            var controller1 = mine.GetComponent<BattleController>();
            controller1.HitLayers = 1 << LayerMask.NameToLayer("Robot");

            controller1.Activate(brain1, other);
            controller1.SetBrains(brain1, brain2, brain3, brain4);
        }
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        print("OnPhotonPlayerConnected. ID: " + newPlayer.ID + ", Name: " + newPlayer.name);
        
        GameObject other = null;
        PhotonView[] players = GameObject.FindObjectsOfType<PhotonView>();
        
            ReadyToStart = true;
        
        print("Number of PhotonViews: " + players.Length);
        foreach (PhotonView ph in players)
        {
            print("ViewID: " + ph.viewID + ", SubID: " + ph.subId + ", InstantiationId: " + ph.instantiationId + ", Name: " + ph.name);
            if (!ph.isMine)
            {
                other = ph.gameObject;
             // break;
                print("Is not mine");
            }
            else
            {
                print("Is Mine");
            }
        }
        if (other != null)
        {
            print("JOE");
        }
        
    }
}
