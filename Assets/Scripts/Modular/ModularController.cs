using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class ModularController : MonoBehaviour {

    public bool UseMovement;
    public bool UseMelee;
    public bool UseRanged;

    public bool IsRunning;

    public GameObject Target;

    public float SensorRange = 50f;
    public float Speed = 5;
    public float TurnSpeed = 180;

    private IBlackBox _movementBrain;
    private IBlackBox _meleeBrain;
    private IBlackBox _rangedBrain;

    private Transform _turret;

	// Use this for initialization
	void Start () {
        _turret = transform.FindChild("Turret");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (IsRunning && Target != null)
        {
            if (UseMovement && _movementBrain != null)
            {
                Movement();
            }
            if (UseMelee && _meleeBrain != null)
            {
                Melee();
            }
            if (UseRanged && _rangedBrain != null)
            {
                Ranged();
            }
        }
	}

    private void Ranged()
    {
        throw new System.NotImplementedException();
    }

    private void Melee()
    {
        throw new System.NotImplementedException();
    }

    private void Movement()
    {
        var direction = Target.transform.position - transform.position;
        var properDistance = direction.magnitude;
        var distance = properDistance / SensorRange;
        direction.y = 0;
        direction.Normalize();
        float angle = Utility.AngleSigned(direction, transform.forward, transform.up);

        float pie1 = 0;
        float pie2 = 0;
        float pie3 = 0;

        if (distance < 1)
        {
            if (angle > -15 && angle < 15)
            {
                // We are in front of the car
                pie1 = 1 - distance;
                
            }
            else if (angle < 30)
            {
                // We are in second row of pie slices
                pie2 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
            }
            else if (angle > -30)
            {
                pie3 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
            }
        }

        RaycastHit hit;

        float leftSensor = 0;
        float rightSensor = 0;

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.1f, 0, 1).normalized), out hit, SensorRange))
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                rightSensor = 1 - hit.distance / SensorRange;
            }
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.1f, 0, 1).normalized), out hit, SensorRange))
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                leftSensor = 1 - hit.distance / SensorRange;
            }
        }

        ISignalArray inputArr = _movementBrain.InputSignalArray;
        inputArr[0] = pie1;
        inputArr[1] = pie2;
        inputArr[2] = pie3;
        inputArr[3] = rightSensor;
        inputArr[4] = leftSensor;

        _movementBrain.Activate();

        ISignalArray outputArr = _movementBrain.OutputSignalArray;
        var steer = (float)outputArr[0] * 2 - 1;
        var gas = (float)outputArr[1] * 2 - 1;        

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime; // * gas;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }
}
