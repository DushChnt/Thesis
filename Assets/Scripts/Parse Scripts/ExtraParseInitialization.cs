using UnityEngine;
using System.Collections;
using Parse;

public class ExtraParseInitialization : MonoBehaviour
{

    void Awake()
    {
        ParseObject.RegisterSubclass<Brain>();
        ParseUser.RegisterSubclass<Player>();
        ParseObject.RegisterSubclass<Match>();
        ParseObject.RegisterSubclass<Frame>();
        ParseAnalytics.TrackAppOpenedAsync();
    }
}
