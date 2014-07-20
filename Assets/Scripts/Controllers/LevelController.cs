using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public abstract class LevelController : MonoBehaviour {

    public GameObject Target;
    public float SensorRange = 100;
    protected bool IsRunning;
    protected IBlackBox brain;
    protected Transform turret;
    public float Speed, TurnSpeed, TurretTurnSpeed;
    protected float attackTimer = 0;
    protected float rifleTimer = 0;
    protected float mortarTimer = 0;


    Player Player
    {
        get
        {
            return Parse.ParseUser.CurrentUser as Player;
        }
    }

    // Abstract functions
    public abstract void Activate(IBlackBox box, GameObject target);
    protected abstract bool CanRun();
    protected abstract void MeleeAttack();
    protected abstract void RifleAttack();
    protected abstract void MortarAttack(float mortarForce);
    protected abstract void FitnessStats(float moveDist, float turnAngle, float turretTurnAngle, float pickup_sensor, float on_target, float turret_on_target);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Loop();
	}

    public void Stop()
    {
        this.IsRunning = false;
    }

    void Loop()
    {       
        if (IsRunning && CanRun())
        {
          //  print("Is Running!");
            // All pies input are the distance to the target in the pie, 1 if close, 0 if far away
            float pie_front = 0; // Pie in [-15, 15]
            float pie_left1 = 0; // Pie slice in [-90, -15]
            float pie_left2 = 0; // Pie slice in [-90, -180]
            float pie_right1 = 0; // Pie slice in [15, 90]
            float pie_right2 = 0; // Pie slice in [90, 180]           

            float wall_left = 0; // Wall sensor in the direction Vector3(-0.1f, 0, 1).normalized
            float wall_right = 0; // Wall sensor in the direction Vector3(0.1f, 0, 1).normalized
            float pickup_sensor = 0; // Angle towards nearest pickup object, 1 if straigt on.

            float on_target = 0; // If target is in line in front of robot (ray cast) 1, else 0
            float distance_to_target = 0; // The distance to target, 1 if close, 0 if far away
            float turret_on_target = 0; // If turret points to target (ray cast?) 1, else 0. Otherwise angle towards target for turret

            ComputeInput(ref pie_front, ref pie_left1, ref pie_left2, ref pie_right1, ref pie_right2, ref wall_left, ref wall_right,
                ref pickup_sensor, ref on_target, ref distance_to_target, ref turret_on_target);

            DoOutput(pie_front, pie_left1, pie_left2, pie_right1, pie_right2, wall_left, wall_right, pickup_sensor, on_target, distance_to_target, turret_on_target);
        }
    }

    protected void DoOutput(float pie_front, float pie_left1, float pie_left2, float pie_right1, float pie_right2,
         float wall_left, float wall_right, float pickup_sensor, float on_target, float distance_to_target, float turret_on_target)
    {
        ISignalArray inputArr = brain.InputSignalArray;
        inputArr[0] = pie_front;
        inputArr[1] = pie_left1;
        inputArr[2] = pie_left2;
        inputArr[3] = pie_right1;
        inputArr[4] = pie_right2;
        inputArr[5] = wall_left;
        inputArr[6] = wall_right;
        inputArr[7] = pickup_sensor;
        inputArr[8] = on_target;
        inputArr[9] = distance_to_target;
        inputArr[10] = turret_on_target;

        brain.Activate();

        ISignalArray outputArr = brain.OutputSignalArray;
        var steer = (float)outputArr[0] * 2 - 1;
        var gas = (float)outputArr[1] * 2 - 1;
        var meleeAttack = (float)outputArr[2];
        float rifleAttack = (float)outputArr[3];
        float turretTurn = (float)outputArr[4] * 2 - 1;
        float mortarForce = (float)outputArr[5];

        var moveDist = gas * Speed * Time.deltaTime;       
        var turnAngle = steer * TurnSpeed * Time.deltaTime; // * gas;
        var turretTurnAngle = turretTurn * TurretTurnSpeed * Time.deltaTime;

        if (Player.CanUseMelee && meleeAttack > 0.5f)
        {
            MeleeAttack();
        }
        if (Player.CanUseRifle && rifleAttack > 0.5f)
        {
            RifleAttack();
        }
        if (Player.CanUseMortar && mortarForce > 0.5f)
        {
            MortarAttack(mortarForce);
        }
        attackTimer += Time.deltaTime;
        rifleTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
        if (Player.CanUseMortar)
        {
            turret.Rotate(new Vector3(0, turretTurnAngle, 0));
        }

        FitnessStats(moveDist, turnAngle, turretTurnAngle, pickup_sensor, on_target, turret_on_target);
    }

   
    private float GetAngleToNearestPickup()
    {
        // TODO: Implement...
        return Utility.Clamp(Utility.GenerateNoise(1.0f));
    }

    protected void ComputeInput(ref float pie_front, ref float pie_left1, ref float pie_left2, ref float pie_right1, ref float pie_right2,
        ref float wall_left, ref float wall_right, ref float pickup_sensor, ref float on_target, ref float distance_to_target, ref float turret_on_target)
    {
        var direction = Target.transform.position - transform.position;
        var properDistance = direction.magnitude;
        var distance = properDistance / SensorRange;

        direction.Normalize();
        direction.y = 0;

        float angle = Utility.AngleSigned(direction, transform.forward, transform.up);

        if (distance < 1)
        {
            if (angle > -180)
            {
                if (angle < -90)
                {
                    pie_left2 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
                }
                else if (angle < -15)
                {
                    pie_left1 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
                }
                else if (angle < 15)
                {
                    pie_front = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
                }
                else if (angle < 90)
                {
                    pie_right1 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
                }
                else if (angle < 180)
                {
                    pie_right2 = Utility.Clamp(1 - distance + Utility.GenerateNoise(0.1f));
                }
            }
        }

        RaycastHit hit;       

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.1f, 0, 1).normalized), out hit, SensorRange))
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                wall_right = 1 - hit.distance / SensorRange;
            }
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.1f, 0, 1).normalized), out hit, SensorRange))
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                wall_left = 1 - hit.distance / SensorRange;
            }
        }

        pickup_sensor = GetAngleToNearestPickup();

        int layerMask = 1 << LayerMask.NameToLayer("Target");

        if (Physics.Raycast(transform.position, transform.forward, out hit, SensorRange, layerMask))
        {
            
            if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
            {
                on_target = 1;
            }
        }
        distance_to_target = distance;

        if (Player.CanUseMortar)
        {
            // Only turret for advanced robots
            if (Physics.Raycast(turret.position, turret.forward, out hit, SensorRange))
            {
                if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
                {
                    turret_on_target = 1;
                }
            }
        }
    }
}
