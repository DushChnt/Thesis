using UnityEngine;
using System.Collections;

public class OpponentController : MonoBehaviour {

    public GameObject Target;
    OpponentState state;
    public float Speed = 4;
    public float TurnSpeed = 180;
    bool IsRunning;
    HammerAttack hammer;
    bool TargetIsInMeleeBox;
    public HealthScript OpponentHealth;
    public HealthScript OwnHealth;
    public GameObject Mortar, Bloom;
    float meleeTimer, rangedTimer, mortarTimer, laserTimer, LaserExposure = 0.2f;
    LineRenderer lineRenderer;
    float forwardTimer;
    Transform turret;

	// Use this for initialization
	void Start () {
        state = OpponentState.Stationary;
        IsRunning = true;

        hammer = transform.FindChild("Hammer").GetComponent<HammerAttack>();
        if (hammer != null)
        {
            hammer.ActivateAnimations();
        }
        lineRenderer = GetComponent<LineRenderer>();
        OpponentHealth.Died += new HealthScript.DeathEventHandler(OpponentHealth_Died);
        turret = gameObject.transform.FindChild("Turret");
        
	}

    void OpponentHealth_Died(object sender, System.EventArgs e)
    {
        Stop();
    }
	
	// Update is called once per frame
	void Update () {
        DoMovement();
        meleeTimer += Time.deltaTime;
        rangedTimer += Time.deltaTime;
        mortarTimer += Time.deltaTime;
        forwardTimer += Time.deltaTime;
        if (lineRenderer.enabled)
        {
            laserTimer += Time.deltaTime;
            if (laserTimer > LaserExposure)
            {
                lineRenderer.enabled = false;
            }
        }
	}   

    public void Stop()
    {
        this.IsRunning = false;
    }

    public void SwitchMovement(OpponentState state)
    {
        this.state = state;
        IsRunning = true;
    }

    void DoMovement()
    {
        if (IsRunning)
        {
            switch (state)
            {
                case OpponentState.Stationary:

                    break;
                case OpponentState.Approaching:
                    ApproachMovement();
                    break;
                case OpponentState.Fleeing:
                    FleeMovement();
                    break;
                case OpponentState.MeleeAttack:
                    MeleeMovement();
                    break;
                case OpponentState.CircleMovement:
                    CircleMovement();
                    break;
                case OpponentState.BackwardsFiring:
                    BackwardsFiringMovement();
                    break;

                case OpponentState.MortarFiring:
                    MortarFiringMovement();
                    break;
            }
        }
    }

    void MortarFiringMovement()
    {
         if (transform.position.z < -20 || transform.position.z > 20 || transform.position.x < -20 || transform.position.x > 20)
        {
            forwardTimer = 0;
        }

         if (forwardTimer < 8f)
         {
             ApproachMovement(1);
         }
         else
         {
             ApproachMovement(-1);
         }

        var direction = Target.transform.position - transform.position;
        var turretAngle = Utility.AngleSigned(direction, turret.forward, turret.up);
        var turretSpeed = 1f;
        if (turretAngle < 20 && turretAngle > -20)
        {
            turretSpeed = 0.1f;
        }
         var turretTurnAngle = turretSpeed * 180 * Time.deltaTime;
         turret.Rotate(new Vector3(0, turretTurnAngle, 0));

         if (mortarTimer >= 2.0f && (turretSpeed == 0.1f || Random.value < 0.1f))
         {             
             MortarAttack(0.6f + Random.value * 0.4f);
         }

         if (Utility.GetDistance(this.gameObject, Target) < 5)
         {
             if (meleeTimer >= 1.5f)
             {
                 MeleeAttack();
             }
         }
         else
         {
             if (rangedTimer >= 0.5f)
             {
                 
                 RangedAttack();
             }
         }
    }

    protected void MortarAttack(float mortarForce)
    {
        mortarTimer = 0;
        GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
        Rigidbody body = m.GetComponent<Rigidbody>();
        m.GetComponent<MortarHit>().MortarCollision += new MortarHit.MortarEventHandler(OpponentController_MortarCollision);
        var direction = (turret.forward + turret.up * 1f) * mortarForce * 500f;
        body.AddForce(direction);
    }

    void OpponentController_MortarCollision(object sender, MortarEventArgs args)
    {
        if (this != null && transform != null && Target != null)
        {
            float x = args.CollisionPoint.x;
            float z = args.CollisionPoint.z;

            float gx = Target.transform.position.x;
            float gz = Target.transform.position.z;

            float distFromCenterSquared = (gx - x) * (gx - x) + (gz - z) * (gz - z);
            float dmgRadiusSquared = 100;
            float dmg = -1;
            if (distFromCenterSquared < dmgRadiusSquared)
            {
                dmg = 1 - distFromCenterSquared / dmgRadiusSquared;
            }

            if (dmg > 0)
            {
                // Hit the target!
                
                if (OpponentHealth != null)
                {
                    float damage = 20 * dmg;
                    if (damage < 1)
                    {
                        damage = 1;
                    }
                    
                    OpponentHealth.TakeDamage(damage);
                    
                }
            }

            gx = transform.position.x;
            gz = transform.position.z;

            distFromCenterSquared = (gx - x) * (gx - x) + (gz - z) * (gz - z);

            dmg = -1;

            if (distFromCenterSquared < dmgRadiusSquared)
            {
                dmg = 1 - distFromCenterSquared / dmgRadiusSquared;
            }

            if (dmg > 0)
            {
                // Hit yourself... noob
                if (OwnHealth != null)
                {
                    float damage = 20 * dmg;
                    if (damage < 1)
                    {
                        damage = 1;
                    }
                    OwnHealth.TakeDamage(damage);
                }
            }
            // print("Instantiating bloom");
            GameObject mortar = ((MortarHit)sender).gameObject;
            Instantiate(Bloom, mortar.transform.position, Quaternion.identity);
        }

    }

    void MeleeMovement()
    {
        ApproachMovement();
        if (meleeTimer >= 1.5f && Utility.GetDistance(this.gameObject, Target) < 5)
        {            
            MeleeAttack();
        }
    }

    void FleeMovement()
    {
        var direction = Target.transform.position - transform.position;
        var angle = Utility.AngleSigned(transform.forward, direction, transform.up);
        var steer = 0;
        var gas = 1;
        if (angle < -2)
        {
            steer = 1;
        }
        else if (angle > 2)
        {
            steer = -1;
        }

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }

    void BackwardsFiringMovement()
    {
        if (transform.position.z < -20 || transform.position.z > 20 || transform.position.x < -20 || transform.position.x > 20)
        {
            forwardTimer = 0;
        }

        if (forwardTimer < 4f)
        {
            ApproachMovement(1);
        }
        else
        {
            ApproachMovement(-1);
        }

        if (Utility.GetDistance(this.gameObject, Target) < 5)
        {
            if (meleeTimer >= 1.5f)
            {
                
                MeleeAttack();
            }
        }
        else
        {
            if (rangedTimer >= 0.5f)
            {               
                RangedAttack();
            }
        }

    }

    void ApproachMovement()
    {
        ApproachMovement(1);
    }

    void ApproachMovement(float gas)
    {
        var direction = Target.transform.position - transform.position;
        var angle = Utility.AngleSigned(transform.forward, direction, transform.up);
        var steer = 0;        
        if (angle < -2)
        {
            steer = -1;
        }
        else if (angle > 2)
        {
            steer = 1;
        }

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }

    void CircleMovement()
    {
        var steer = 1;
        var gas = 0.5f;
       

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Robot"))
        {
            // print("Enter!");
            TargetIsInMeleeBox = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        //     print("Hammer: " + col.tag);
        if (col.tag.Equals("Robot"))
        {
            // print("Exit!");
            TargetIsInMeleeBox = false;
        }
    }

    void MeleeAttack()
    {
        meleeTimer = 0;
        hammer.PerformAttack();
        if (TargetIsInMeleeBox)
        {
            float dmg = 8 + Random.value * 2;
            OpponentHealth.TakeDamage(dmg);
        }

    }

    void RangedAttack()
    {
        rangedTimer = 0;
        RaycastHit hit;
        bool isHit = false;
        Vector3 s_0 = transform.position + transform.forward * 1.1f;
        Vector3 p_0 = s_0 + transform.forward * 50;
        Vector3 t = new Vector3(p_0.x, transform.position.y, p_0.z);
        Vector3 dir = Vector3.Normalize(t - s_0);
        // print("Dir: " + dir);
        Debug.DrawLine(s_0, t, Color.yellow);
        Vector3 ray = transform.forward;
        if (Random.value > 0.7)
        {
            ray = new Vector3(ray.x + Utility.GenerateNoise(0.2f), ray.y, ray.z + Utility.GenerateNoise(0.2f));
        }
        if (Physics.Raycast(s_0, ray, out hit, 50))
        {
            isHit = true;
            if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
            {
                // Do damage
                //       print("Ranged Damage!");
                float dmg = 1 + Random.value * 1;             
                OpponentHealth.TakeDamage(dmg);
            }

        }
        Vector3 point;
        if (isHit)
        {
            point = hit.point;
            // print("Hit something at distance " + Utility.GetDistance(gameObject.transform.position, hit.point));
        }
        else
        {
            //point = transform.position + transform.forward * 50;
            point = transform.position + ray * 50;
        }
        ShootLaser(point);
    }

    protected void ShootLaser(Vector3 endPoint)
    {
        laserTimer = 0;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
    
}
public enum OpponentState
{
    Stationary,
    Approaching,
    Fleeing,
    MeleeAttack,
    CircleMovement,
    BackwardsFiring,
    MortarFiring
}
