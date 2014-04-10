using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class CarMove : MonoBehaviour {

    public float SensorRange = 50f;
    public float MeleeRange = 5f;
    public float AttackCoolDown = 0.7f;
    public float Speed = 5;
    public float TurnSpeed = 180;
    public bool isRunning = false;
    private GameObject target;
    private IBlackBox brain;
    Vector3 startPos;
    float shortestDistance;
    float totalDistance;
    long ticks;
    float attackTimer = 0;
    public int Hits = 0;
    public Material NormalSphereMat, AttackSphereMat;
    Transform sphere;
    bool AttackShowState = false;
    public bool RunBestOnly = false;

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
   //     float fit = 1.0f / shortestDistance;
        float avg = totalDistance / ticks;
        float fit = 1.0f / avg;
     //   print("Fitness: " + fit);
        fit += Hits;
        return fit;
    }

	// Use this for initialization
	void Start () {
        //Time.timeScale = 1;
        totalDistance = 0;
        ticks = 0;
        sphere = transform.FindChild("Sphere");
	}

    private float Clamp(float val)
    {
        if (val < 0)
        {
            return 0;
        }
        if (val > 1)
        {
            return 1;
        }
        return val;
    }

    private float GenerateNoise(float threshold)
    {
        return Random.Range(-threshold, threshold);
    }

    /// <summary>

    /// Determine the signed angle between two vectors, with normal 'n'

    /// as the rotation axis.

    /// </summary>

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {

        return Mathf.Atan2(

            Vector3.Dot(n, Vector3.Cross(v1, v2)),

            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;

    }

	// Update is called once per frame
	void FixedUpdate () {
        if (isRunning)
        {
            var direction = target.transform.position - transform.position;
            var properDistance = direction.magnitude;
            var distance = properDistance / SensorRange;
           
            
            //distance /= 20f; // Normalize to a radius of 20
            direction.Normalize();
            direction.y = 0;
            
       //     float angle2 = Vector2.Angle(new Vector2(direction.x, direction.z), new Vector2(transform.forward.x, transform.forward.z));

            float angle = AngleSigned(direction, transform.forward, transform.up);

           // print("Angle: " + angle + ", angle2: " + angle2);

            float pie1 = 1;
            float pie2 = 1;
            float pie3 = 1;
            float melee = 0;

            if (false)
            {
                // Check first pie slice
                if (distance < 1)
                {
                    if (angle < 15)
                    {
                        // We are in front of the car
                        pie1 = distance;
                    }
                    else if (angle < 30)
                    {
                        // We are in second row of pie slices
                        pie2 = Clamp(distance + GenerateNoise(0.1f));
                    }
                    else if (angle < 60)
                    {
                        pie3 = Clamp(distance + GenerateNoise(0.3f));
                    }
                }
            }
            else
            {
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
                        pie2 = Clamp(distance + GenerateNoise(0.1f));
                    }
                    else if (angle > -30)
                    {
                        pie3 = Clamp(distance + GenerateNoise(0.1f));
                    }
                }
            }

            distance = Clamp(distance);

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
                    if (RunBestOnly)
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.red);
                    }
                    rightSensor = hit.distance / SensorRange;
                }
            }
            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.1f, 0, 1).normalized), out hit, SensorRange))
            {
                //  Debug.Log("Hit something!");
                //  Debug.Log("Tag: " + hit.collider.tag);
                if (hit.collider.tag.Equals("Wall"))
                {
                    if (RunBestOnly)
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.red);
                    }
                    leftSensor = hit.distance / SensorRange;
                }
            }

            //inputArr[0] = direction.x;
            //inputArr[1] = direction.z;
            //inputArr[2] = distance;
           // inputArr[0] = angle / 180f;
            inputArr[0] = pie1;
            inputArr[1] = pie2;
            inputArr[2] = pie3;
            inputArr[3] = rightSensor;
            inputArr[4] = leftSensor;
            inputArr[5] = melee;
          //  inputArr[2] = 1; // bias
           // inputArr[2] = direction.z;
           //inputArr[3] = distance; // Wait and see

            brain.Activate();

            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;
            var meleeAttack = (float)outputArr[2];

          //  var steer = Input.GetAxis("Horizontal");
           // var gas = Input.GetAxis("Vertical");

            // steer = Random.Range(-1, 1);
            //gas = Random.Range(-1, 1);

            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime * gas;

            if (meleeAttack > 0.5f)
            {
                Attack(properDistance, angle);
            }

            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);

            PhotoGUI.inX = (float)inputArr[0];
            PhotoGUI.inZ = (float)inputArr[1];
            PhotoGUI.outGas = gas;
            PhotoGUI.outSteer = steer;
            PhotoGUI.dist = GetDistance();
            PhotoGUI.currentFitness = 1.0f / PhotoGUI.dist;

            shortestDistance = Mathf.Min(shortestDistance, GetDistance());
            totalDistance += GetDistance();
            ticks++;

            if (AttackShowState && attackTimer > 0.2f)
            {
                AttackShowState = false;
                sphere.renderer.material = NormalSphereMat;
            }
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
                    sphere.renderer.material = AttackSphereMat;
                    AttackShowState = true;
                    print("Attack! Distance: " + distance + ", angle: " + angle);
                }
            }
        }
        attackTimer += Time.deltaTime;
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
