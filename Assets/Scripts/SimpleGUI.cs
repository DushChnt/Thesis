using UnityEngine;
using System.Collections;

public class SimpleGUI : MonoBehaviour {

    public static int CurrentlyHitting = 0;
    public static int NotHitting = 0;
    public static int Total = 0;

	// Use this for initialization
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 100), "Total: " + (CurrentlyHitting + NotHitting) + "\n- Hitting: " + CurrentlyHitting + "\n- Not: " + NotHitting);
    }
}
