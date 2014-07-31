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
    float meleeTimer, rangedTimer, mortarTimer;

	// Use this for initialization
	void Start () {
        state = OpponentState.Stationary;
        IsRunning = true;

        hammer = transform.FindChild("Hammer").GetComponent<HammerAttack>();
        if (hammer != null)
        {
            hammer.ActivateAnimations();
        }

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
                    ApproachMovment();
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
            }
        }
    }

    void MeleeMovement()
    {
        ApproachMovment();
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

    void ApproachMovment()
    {
        var direction = Target.transform.position - transform.position;
        var angle = Utility.AngleSigned(transform.forward, direction, transform.up);
        var steer = 0;
        var gas = 1;
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
    
}
public enum OpponentState
{
    Stationary,
    Approaching,
    Fleeing,
    MeleeAttack,
    CircleMovement
}
