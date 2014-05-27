using UnityEngine;
using System.Collections;
using Parse;

public class ExtraParseInitialization : MonoBehaviour
{

    void Awake()
    {
        ParseObject.RegisterSubclass<Brain>();
        ParseAnalytics.TrackAppOpenedAsync();
    }
}
