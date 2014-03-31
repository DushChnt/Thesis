using UnityEngine;
using System.Collections;

public class ParallelPhotoGUI : MonoBehaviour
{

    public static float inX, inZ, outSteer, outGas, dist, currentFitness;

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200),
            "In X: " + inX + "\n" +
            "In Z: " + inZ + "\n" +
            "Out steer: " + outSteer + "\n" +
            "Out gas: " + outGas + "\n" +
            "Distance: " + dist + "\n" +
            "Current Fitness: " + currentFitness);

        if (GUI.Button(new Rect(10, 160, 100, 30), "Start EA"))
        {
            GameObject.Find("Evaluator").GetComponent<ParallelStartEvaluation>().StartEA();
        }

        if (GUI.Button(new Rect(10, 200, 100, 30), "Run Best"))
        {
            GameObject.Find("Evaluator").GetComponent<ParallelStartEvaluation>().RunBest();
        }

        if (GUI.Button(new Rect(10, 240, 100, 30), "Stop EA"))
        {
            GameObject.Find("Evaluator").GetComponent<ParallelStartEvaluation>().StopEA();
        }
    }
}
