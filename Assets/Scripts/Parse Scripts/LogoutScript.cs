using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;

public class LogoutScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void Logout()
    {

        Player player = ParseUser.CurrentUser as Player;
        player.IsOnline = false;
        Task saveTask = player.SaveAsync();
    }

    IEnumerator logOut()
    {
        Player player = ParseUser.CurrentUser as Player;
        player.IsOnline = false;
        Task saveTask = player.SaveAsync();

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }
        print("Done logging out");
    }

    void OnApplicationQuit()
    {
        StartCoroutine(logOut());
        
        print("Exit");
    }
}
