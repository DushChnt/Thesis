using UnityEngine;
using System.Collections;

public class CameFromScript : MonoBehaviour {

    public const string BOOTCAMP = "Bootcamp";
    public const string CHAMPIONS_ARENA = "Champion's Arena";

    public const string CAME_FROM = "Came from";

    public CameFrom CameFrom;

	// Use this for initialization
	void Start () {
        switch (CameFrom)
        {
            case global::CameFrom.Bootcamp:
                PhotonNetwork.offlineMode = true;
                PlayerPrefs.SetString(CAME_FROM, BOOTCAMP);
                break;
            case global::CameFrom.ChampionsArena:
                PhotonNetwork.offlineMode = false;
                PlayerPrefs.SetString(CAME_FROM, CHAMPIONS_ARENA);
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
}
public enum CameFrom
{
    Bootcamp, ChampionsArena
}
