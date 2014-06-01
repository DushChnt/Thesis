using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public abstract class BaseController : Photon.MonoBehaviour {

    protected bool isRunning;
    protected GameObject target;
    protected IBlackBox brain;
    public float SensorRange = 50f;
    public float MeleeRange = 7f;
    public float AttackCoolDown = 0.7f;
    public float RifleCoolDown = 0.2f;
    public float MortarCoolDown = 1.5f;
    /// <summary>
    /// Current speed
    /// </summary>
    protected float Speed = 5;
    /// <summary>
    /// Maximum top speed
    /// </summary>
    public float TopSpeed = 5;
    /// <summary>
    /// How fast the speed recovers to top speed in value/second
    /// </summary>
    protected float RecoveryRate = 1f;

    public float MeleeSpeedPenalty = 2f;
    public float MeleeRecoveryRate = 2f;
    public float RifleSpeedPenalty = 0.5f;
    public float RifleRecoveryRate = 2f;
    public float MortarSpeedPenalty = 5f;
    public float MortarRecoveryRate = 2f;
    public float MaxMortarForce = 500f;

    public float TurnSpeed = 180;
    public float TurretTurnSpeed = 180;
    protected float attackTimer = 0;
    protected float rifleTimer = 0;
    protected float mortarTimer = 0;
    protected Vector3 startPos;
    protected float shortestDistance;
    protected float totalDistance;
    protected float totalAngle;
    protected float totalTurretAngle;
    protected long ticks;
    public bool RunBestOnly = false;
  //  public HealthScript health;
   // protected RobotController opponent;

    public LayerMask HitLayers;

    protected Transform turret;

    public GameObject Mortar;
    public bool HumanControlled;

    protected abstract void Attack(float dist, float angle);
    protected abstract void RifleAttack();
    protected abstract void MortarAttack(float force);
    public abstract void ReceiveMortarInfo(float hitRate, float distFromCenterSquared);
    public abstract void Activate(IBlackBox box, GameObject target);
    public abstract void Stop();

	// Use this for initialization
	void Start () {
	
	}

    void HumanController()
    {
        var direction = target.transform.position - transform.position;
        var properDistance = direction.magnitude;
        var distance = properDistance / SensorRange;

        direction.Normalize();
        direction.y = 0;

        float angle = Utility.AngleSigned(direction, transform.forward, transform.up);
        if (Input.GetButtonDown("Fire1"))
        {

            Utility.Log("Firing!");

            Attack(properDistance, angle);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Utility.Log("Rifling!");
            if (angle < 5f && angle > -5f)
            {
                Utility.Log("Enemy within [-5, 5] degrees of front");
            }
            RifleAttack();
        }
        if (Input.GetButton("Jump"))
        {
            var turretTurnAngle = 1 * TurretTurnSpeed * Time.deltaTime;
            turret.Rotate(new Vector3(0, turretTurnAngle, 0));
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            
            MortarAttack(0.9f);
        }
        rifleTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;
        var moveDist = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        var turnAngle = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime; // * gas;
        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);


        attackTimer += Time.deltaTime;
    }

    /// <summary>
    /// Network inputs:
    /// Pie slice inputs are normalized distance in the sensor range from 0 - 1. 0 if far away, 1 if really close
    /// 0: Pie slice in front of robot [-15, 15] degrees
    /// 1: Pie slice to the right of robot (15, 30] degress
    /// 2: Pie slice to the left of robot [-30, -15) degress
    /// 3: Wall sensor in the direction Vector3(0.1f, 0, 1).normalized
    /// 4: Wall sensor in the direction Vector3(-0.1f, 0, 1).normalized
    /// 5: Melee. If the target is in front of the robot and within the melee range, this is 1. Otherwise it is 0
    /// 6: Line-of-fire. If the target is in front of the robot within [-5, 5] degrees it is 1, otherwise 0.
    /// </summary>
    void NetworkController()
    {
        if (!photonView.isMine) return;

        if (isRunning && target != null)
        {
            var direction = target.transform.position - transform.position;
            var properDistance = direction.magnitude;
            var distance = properDistance / SensorRange;

            direction.Normalize();
            direction.y = 0;

            float angle = Utility.AngleSigned(direction, transform.forward, transform.up);
            var turretDirection = target.transform.position - turret.position;
            turretDirection.y = 0;
            turretDirection.Normalize();
            float turretAngle = Utility.AngleSigned(turretDirection, turret.forward, turret.up);
          //  print(turretAngle);
          //  turretAngle = turretAngle /180f;
         //   print(turretAngle);
        //    totalTurretAngle += Mathf.Abs(turretAngle);

            float pie1 = 0;
            float pie2 = 0;
            float pie3 = 0;
            float melee = 0;
            float lof = 0; // Line of Fire
            float turret_lof = 0;

            if (angle < 5f && angle > -5f)
            {
                lof = 1 - Mathf.Abs(angle) / 5f;
                totalAngle++;
            }
            if (turretAngle > -5f && turretAngle < 5f)
            {
                turret_lof = 1 - Mathf.Abs(turretAngle) / 5f;
                totalTurretAngle++;
            }

            if (distance < 1)
            {
                if (angle > -15 && angle < 15)
                {
                    // We are in front of the car
                    pie1 = 1 - distance;
                    if (properDistance < MeleeRange)
                    {
                        melee = 1;
                    }
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

            ISignalArray inputArr = brain.InputSignalArray;
            inputArr[0] = pie1;
            inputArr[1] = pie2;
            inputArr[2] = pie3;
            inputArr[3] = rightSensor;
            inputArr[4] = leftSensor;
            inputArr[5] = melee;
            inputArr[6] = lof;
            inputArr[7] = turret_lof;
            inputArr[8] = distance;

            brain.Activate();

            ISignalArray outputArr = brain.OutputSignalArray;
            var steer = (float)outputArr[0] * 2 - 1;
            var gas = (float)outputArr[1] * 2 - 1;
            var meleeAttack = (float)outputArr[2];
            float rifleAttack = (float)outputArr[3];
            float turretTurn = (float)outputArr[4] * 2 - 1;
            float mortarForce = (float)outputArr[5];

            if (!PhotonNetwork.offlineMode)
            {
                photonView.RPC("GetOutput", PhotonTargets.Others, properDistance, angle, steer, gas, meleeAttack, rifleAttack,
                    turretTurn, mortarForce, transform.position, transform.rotation, turret.rotation, PhotonNetwork.time);
            }
            var moveDist = gas * Speed * Time.deltaTime;
            var turnAngle = steer * TurnSpeed * Time.deltaTime; // * gas;
            var turretTurnAngle = turretTurn * TurretTurnSpeed * Time.deltaTime;

            if (meleeAttack > 0.5f)
            {
                Attack(properDistance, angle);
            }
            attackTimer += Time.deltaTime;

            if (rifleAttack > 0.5f)
            {
                RifleAttack();
            }
            if (mortarForce > 0.1f)
            {
                MortarAttack(mortarForce);
            }
            rifleTimer += Time.deltaTime;
            mortarTimer += Time.deltaTime;
            transform.Rotate(new Vector3(0, turnAngle, 0));
            transform.Translate(Vector3.forward * moveDist);
            turret.Rotate(new Vector3(0, turretTurnAngle, 0));

            if (PhotonNetwork.offlineMode)
            {
                //  totalDistance += Mathf.Abs(GetDistance() - OptimizerParameters.DistanceToKeep);
                totalDistance += Mathf.Abs(GetDistance() - Settings.Brain.DistanceToKeep);
                //    totalAngle += Mathf.Abs(angle);
                ticks++;
            }
        }
    }

    [RPC]
    protected void GetOutput(float properDistance, float angle, float steer, float gas, float meleeAttack, float rifleAttack, float turretTurn, 
        float mortarForce, Vector3 realPosition, Quaternion realRotation, Quaternion turretRotation, double time) 
    {
    //    print("GetOutput");
        double timeDifference = PhotonNetwork.time - time;

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime; // * gas;
        var turretTurnAngle = turretTurn * TurretTurnSpeed * Time.deltaTime;

        if (meleeAttack > 0.5f)
        {
            Attack(properDistance, angle);
        }
        attackTimer += Time.deltaTime;

        if (rifleAttack > 0.5f)
        {
            RifleAttack();
        }
        if (mortarForce > 0.1f)
        {
            MortarAttack(mortarForce);
        }

        transform.position = realPosition; // Vector3.Lerp(transform.position, realPosition, 0.2f);
        //  transform.position = realPosition;
        transform.rotation = realRotation; // Quaternion.Lerp(transform.rotation, realRotation, 0.2f);
        if (turret != null)
        {
            turret.rotation = turretRotation; // Quaternion.Lerp(turret.rotation, turretRotation, 0.2f);
        }

        rifleTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;
        //transform.Rotate(new Vector3(0, turnAngle, 0));
        //transform.Translate(Vector3.forward * moveDist);
        //turret.Rotate(new Vector3(0, turretTurnAngle, 0));
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Speed < TopSpeed)
        {
            if (Speed < 0)
            {
                Speed = 0;
            }
            Speed += RecoveryRate * Time.deltaTime;
            if (Speed > TopSpeed)
            {
                Speed = TopSpeed;
            }
        }
        if (HumanControlled)
        {
            HumanController();
        }
        else
        {
            NetworkController();
        }
        
    }
}
