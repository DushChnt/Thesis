using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class CarMove : MonoBehaviour {

    public float Speed = 5;
    public float TurnSpeed = 180;
    public bool isRunning = false;
    private GameObject target;
    private IBlackBox brain;

    public float GetFitness()
    {
        if (target != null)
        {
            return Vector3.Distance(target.transform.position, transform.position);
        }
        return 0.0f;
    }

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
        if (isRunning)
        {
            var direction = target.transform.position - transform.position;
           // var distance = direction.magnitude;
           // distance /= 20f; // Normalize to a radius of 20
            direction.Normalize();

            ISignalArray inputArr = brain.InputSignalArray;
            ISignalArray outputArr = brain.OutputSignalArray;

            inputArr[0] = direction.x;
            inputArr[1] = direction.y;
            inputArr[2] = direction.z;
           //inputArr[3] = distance; // Wait and see

            brain.Activate();

            var steer = (float)outputArr[0];
            var gas = (float)outputArr[1];

          //  var steer = Input.GetAxis("Horizontal");
           // var gas = Input.GetAxis("Vertical");

            // steer = Random.Range(-1, 1);
            //gas = Random.Range(-1, 1);

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);
        }
	}

    public void Activate(IBlackBox brain, GameObject target) {
        this.target = target;
        this.brain = brain;
        brain.ResetState();
        isRunning = true;
    }

    public void Stop()
    {
        this.isRunning = false;
    }
}
