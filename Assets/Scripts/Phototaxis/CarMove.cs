using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class CarMove : MonoBehaviour {

    public float SensorRange = 50f;
    public float Speed = 5;
    public float TurnSpeed = 180;
    public bool isRunning = false;
    private GameObject target;
    private IBlackBox brain;
    Vector3 startPos;
    float shortestDistance;
    

    public float GetDistance()
    {
        if (target != null)
        {
            Vector2 a = new Vector2(target.transform.position.x, target.transform.position.z);
            Vector2 b = new Vector2(transform.position.x, transform.position.z);
            return Vector2.Distance(a, b);
        }
        return 0.0f;
    }

    public float GetFitness()
    {
        if (Vector3.Distance(transform.position, startPos) < 1)
        {
            return 0;
        }
        float fit = 1.0f / shortestDistance;
        print("Fitness: " + fit);
        return fit;
    }

	// Use this for initialization
	void Start () {
        //Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isRunning)
        {
            var direction = target.transform.position - transform.position;
           // var distance = direction.magnitude;
           // distance /= 20f; // Normalize to a radius of 20
            direction.Normalize();

            ISignalArray inputArr = brain.InputSignalArray;
            ISignalArray outputArr = brain.OutputSignalArray;

            RaycastHit hit;

            float leftSensor = 1;
            float rightSensor = 1;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.1f, 0, 1).normalized), out hit, SensorRange))
            {
                //  Debug.Log("Hit something!");
                // Debug.Log("Tag: " + hit.collider.tag);
                if (hit.collider.tag.Equals("Wall"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    rightSensor = hit.distance / SensorRange;
                }
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.1f, 0, 1).normalized), out hit, SensorRange))
            {
                //  Debug.Log("Hit something!");
               //  Debug.Log("Tag: " + hit.collider.tag);
                if (hit.collider.tag.Equals("Wall"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    leftSensor = hit.distance / SensorRange;
                }
            }

            inputArr[0] = direction.x;
            inputArr[1] = direction.z;
            inputArr[2] = rightSensor;
            inputArr[3] = leftSensor;
          //  inputArr[2] = 1; // bias
           // inputArr[2] = direction.z;
           //inputArr[3] = distance; // Wait and see

            brain.Activate();

            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;

          //  var steer = Input.GetAxis("Horizontal");
           // var gas = Input.GetAxis("Vertical");

            // steer = Random.Range(-1, 1);
            //gas = Random.Range(-1, 1);

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);

            PhotoGUI.inX = (float)inputArr[0];
            PhotoGUI.inZ = (float)inputArr[1];
            PhotoGUI.outGas = gas;
            PhotoGUI.outSteer = steer;
            PhotoGUI.dist = GetDistance();
            PhotoGUI.currentFitness = 1.0f / PhotoGUI.dist;

            shortestDistance = Mathf.Min(shortestDistance, GetDistance());
        }
	}

    public void Activate(IBlackBox brain, GameObject target) {
        this.target = target;
        this.brain = brain;
        brain.ResetState();
        this.startPos = transform.position;
        shortestDistance = int.MaxValue;
        isRunning = true;
    }

    public void Stop()
    {
        this.isRunning = false;
    }
}
