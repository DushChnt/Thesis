using UnityEngine;
using System.Collections;

public class OptimizerGUI : MonoBehaviour {

    public static uint CurrentGeneration;
    public static int CurrentIteration, MaxIterations;
    public static double BestFitness;
    bool StopPressed = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Start EA"))
        {
            GameObject.Find("Evaluator").GetComponent<Optimizer>().StartEA();
        }

        if (GUI.Button(new Rect(10, 50, 100, 30), "Run Best"))
        {
            GameObject.Find("Evaluator").GetComponent<Optimizer>().RunBest();
        }

        if (GUI.Button(new Rect(10, 90, 100, 30), "Stop EA"))
        {
            StopPressed = true;
            GameObject.Find("Evaluator").GetComponent<Optimizer>().StopEA();
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Back"))
        {
            Application.LoadLevel("Training Overview");
        }
        int height = 70;
        int top = Screen.height - height - 10;
        GUI.Box(new Rect(10, top, 300, height), string.Format("Current generation: {0}\nCurrent iteration: {1} / {2}\nBest fitness: {3:0.00}{4}",
            CurrentGeneration, CurrentIteration, MaxIterations, BestFitness, StopPressed ? "\nStop pressed" : ""));
	}
}
