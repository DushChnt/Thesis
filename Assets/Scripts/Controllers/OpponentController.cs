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
    float meleeTimer, rangedTimer, mortarTimer, laserTimer, LaserExposure = 0.2f;
    LineRenderer lineRenderer;
    float forwardTimer;

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
            }
        }
    }

    void MeleeMovement()
    {
        ApproachMovement();
        if (meleeTimer >= 1.5f && Utility.GetDistance(this.gameObject, Target) < 5)
        {
            meleeTimer = 0;
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
                meleeTimer = 0;
                MeleeAttack();
            }
        }
        else
        {
            if (rangedTimer >= 0.5f)
            {
                rangedTimer = 0;
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
            print("Enter!");
            TargetIsInMeleeBox = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        //     print("Hammer: " + col.tag);
        if (col.tag.Equals("Robot"))
        {
            print("Exit!");
            TargetIsInMeleeBox = false;
        }
    }

    void MeleeAttack()
    {
        hammer.PerformAttack();
        if (TargetIsInMeleeBox)
        {
            float dmg = 8 + Random.value * 2;
            OpponentHealth.TakeDamage(dmg);
        }

    }

    void RangedAttack()
    {
        RaycastHit hit;
        bool isHit = false;
        Vector3 s_0 = transform.position + transform.forward * 1.1f;
        Vector3 p_0 = s_0 + transform.forward * 50;
        Vector3 t = new Vector3(p_0.x, transform.position.y, p_0.z);
        Vector3 dir = Vector3.Normalize(t - s_0);
        print("Dir: " + dir);
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
            print("Hit something at distance " + Utility.GetDistance(gameObject.transform.position, hit.point));
        }
        else
        {
            //point = transform.position + transform.forward * 50;
            point = t;
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
}
