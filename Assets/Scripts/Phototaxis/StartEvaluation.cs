using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class StartEvaluation : MonoBehaviour {

    public GameObject Robot;
    public GameObject Target;

	// Use this for initialization
	void Start () {
        Reset();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Evaluate(IBlackBox box)
    {
        Reset();
    }

    public float GetFitness()
    {
        if (Robot != null)
        {
            return Robot.GetComponent<CarMove>().GetFitness();
        }
        return 0.0f;
    }

    private void Reset()
    {
        print("Resetting");
        Robot.transform.position = new Vector3(10, 1, 0);
    }
}
