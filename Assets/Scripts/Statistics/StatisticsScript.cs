using UnityEngine;
using System.Collections;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class StatisticsScript : MonoBehaviour {

    public dfLabel MatchesPlayed, MatchesWon, MatchesLost, DamageDealt, DamageTaken;
    IEnumerable<ParseObject> matches;

	// Use this for initialization
	void Start () {
        StartCoroutine(FindMatches());
	}

    IEnumerator FindMatches()
    {
        var query = ParseObject.GetQuery("Match").WhereEqualTo("player", ParseUser.CurrentUser);

        bool isRunning = true;
        Task findTask = query.FindAsync().ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                print("Oh no");
                print(t.Exception.Message);
            }
            matches = t.Result;
            print("IsRunning = false");
            isRunning = false;
            
        });

        

        while (isRunning)
        {
            yield return null;
        }
        print("Hello?");
        if (!findTask.IsCanceled || !findTask.IsFaulted)
        {
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        int matchesPlayed = matches.Count();
        int matchesWon = matches.Where(t => (bool)t["won"]).Count();
        int matchesLost = matchesPlayed - matchesWon;

        MatchesPlayed.Text = matchesPlayed + "";
        MatchesWon.Text = matchesWon + "";
        MatchesLost.Text = matchesLost + "";
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
