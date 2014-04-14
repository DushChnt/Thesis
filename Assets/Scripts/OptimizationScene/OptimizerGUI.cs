using UnityEngine;
using System.Collections;

public class OptimizerGUI : MonoBehaviour {

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
            GameObject.Find("Evaluator").GetComponent<Optimizer>().StopEA();
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Back"))
        {
            Application.LoadLevel("Optimizer Menu scene");
        }
	}
}
