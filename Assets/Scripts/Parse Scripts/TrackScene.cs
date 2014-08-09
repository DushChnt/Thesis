using UnityEngine;
using System.Collections;
using Parse;
using System.Collections.Generic;

public class TrackScene : MonoBehaviour {

    float timer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.unscaledDeltaTime;
	}

    string GetTimeBucket()
    {
        if (timer <= 5f)
        {
            return "0s - 5s";
        }
        else if (timer <= 15f)
        {
            return "5s - 15s";
        }
        else if (timer <= 30f)
        {
            return "15s - 30s";
        }
        else if (timer <= 60f)
        {
            return "30s - 60s";
        }
        else if (timer <= 120f)
        {
            return "1m - 2m";
        }
        else if (timer <= 300f)
        {
            return "2m - 5m";
        }
        else if (timer <= 600f)
        {
            return "5m - 10m";
        }
        else if (timer <= 1200f)
        {
            return "10m - 20m";
        }
        else
        {
            return "> 20m";
        }
    }

    void OnDestroy()
    {
        var dimensions = new Dictionary<string, string> 
        {
            { "scene", Application.loadedLevelName },
            { "time", GetTimeBucket() }
        };

        ParseAnalytics.TrackEventAsync("Scenes Visited", dimensions);
    }
}
