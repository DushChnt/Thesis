using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class RobotController : MonoBehaviour {

    bool isRunning;
    private GameObject target;
    private IBlackBox brain;
    public float SensorRange = 50f;
    public float MeleeRange = 7f;
    public float AttackCoolDown = 0.7f;
    public float Speed = 5;
    public float TurnSpeed = 180;
    float attackTimer = 0;
    public int Hits = 0;
    Vector3 startPos;
    float shortestDistance;
    float totalDistance;
    long ticks;
    public bool RunBestOnly = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isRunning)
        {
            var direction = target.transform.position - transform.position;
            var properDistance = direction.magnitude;
            var distance = properDistance / SensorRange;

            direction.Normalize();
            direction.y = 0;

            float angle = Utility.AngleSigned(direction, transform.forward, transform.up);

            float pie1 = 1;
            float pie2 = 1;
            float pie3 = 1;
            float melee = 0;

            if (distance < 1)
            {
                if (angle > -15 && angle < 15)
                {
                    // We are in front of the car
                    pie1 = distance;
                    if (properDistance < MeleeRange)
                    {
                        melee = 1;
                    }
                }
                else if (angle < 30)
                {
                    // We are in second row of pie slices
                    pie2 = Utility.Clamp(distance + Utility.GenerateNoise(0.1f));
                }
                else if (angle > -30)
                {
                    pie3 = Utility.Clamp(distance + Utility.GenerateNoise(0.1f));
                }
            }

            RaycastHit hit;

            float leftSensor = 1;
            float rightSensor = 1;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.1f, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    rightSensor = hit.distance / SensorRange;
                }
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.1f, 0, 1).normalized), out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Wall"))
                {
                    leftSensor = hit.distance / SensorRange;
                }
            }

            ISignalArray inputArr = brain.InputSignalArray;
            inputArr[0] = pie1;
            inputArr[1] = pie2;
            inputArr[2] = pie3;
            inputArr[3] = rightSensor;
            inputArr[4] = leftSensor;
            inputArr[5] = melee;

            brain.Activate();

            ISignalArray outputArr = brain.OutputSignalArray;
            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;
            var meleeAttack = (float)outputArr[2];

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            if (meleeAttack > 0.5f)
            {
                Attack(properDistance, angle);
            }

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);

            totalDistance += GetDistance();
            ticks++;
        }
	}

    public void Attack(float distance, float angle)
    {
        if (attackTimer > AttackCoolDown)
        {
            if (distance < MeleeRange && angle > -15 && angle < 15)
            {
                // Do attack

                attackTimer = 0;
                Hits++;
                if (RunBestOnly)
                {
                    //sphere.renderer.material = AttackSphereMat;
                    //AttackShowState = true;
                    print("Attack! Distance: " + distance + ", angle: " + angle);
                }
            }
        }
        attackTimer += Time.deltaTime;
    }

    void ComputeInput()
    {
        
    }

    void ProcessOutput()
    {
        
    }

    public float GetFitness()
    {
        if (Vector3.Distance(transform.position, startPos) < 1)
        {
            return 0;
        }
        float fit = 0;
        // Approach fitness
        float approach = 1.0f / ( totalDistance / ticks) * OptimizerParameters.WApproach;
        fit += approach;
        
        // Melee fitness
        float melee = Hits * OptimizerParameters.WMeleeAttack;
        fit += melee;
        
        return fit;
    }

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

    public void Activate(IBlackBox box, GameObject target)
    {
        isRunning = true;
        this.brain = box;
        this.target = target;
        this.startPos = transform.position;
    }

    public void Stop()
    {
        this.isRunning = false;
    }
}
