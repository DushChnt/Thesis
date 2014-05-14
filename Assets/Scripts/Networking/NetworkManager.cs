using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Connect();
    }

    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("TEST 0.1");

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
            RobotController rc = mine.GetComponent<RobotController>();
            rc.HumanActivate(other);
        }
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        print("OnPhotonPlayerConnected. ID: " + newPlayer.ID + ", Name: " + newPlayer.name);
        
        GameObject other = null;
        PhotonView[] players = GameObject.FindObjectsOfType<PhotonView>();
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
