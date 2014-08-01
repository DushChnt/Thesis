using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System;

public class FightController : LevelController {

    IBlackBox brain1, brain2, brain3, brain4, defaultBrain;
    int ActiveBrain;
    HammerAttack hammer;
    bool TargetIsInMeleeBox;
    FightController Opponent;
    public HealthScript HealthScript;
    public HealthScript OpponentHealth;
    public bool DummyAttack;
    public delegate void AttackEventHandler();
    float laserTimer;
    public float LaserExposure = 0.1f;

    public event AttackEventHandler MeleeAttackEvent;
    public event AttackEventHandler MeleeHitEvent;
    public event AttackEventHandler RangedAttackEvent;
    public event AttackEventHandler RangedHitEvent;

    LineRenderer lineRenderer;

    public override void Activate(IBlackBox box, GameObject target)
    {
        this.defaultBrain = box;
        this.brain = box;
        this.brain.ResetState();
        this.IsRunning = true;
        this.Target = target;
        FightController opponent = Target.GetComponent<FightController>();
        if (opponent != null)
        {
            Opponent = opponent;
        }
        if (OpponentHealth != null)
        {
            OpponentHealth.Died += new global::HealthScript.DeathEventHandler(OpponentHealth_Died);
        }
        if (HealthScript != null)
        {
            HealthScript.Died += new global::HealthScript.DeathEventHandler(HealthScript_Died);
        }
    }

    void HealthScript_Died(object sender, EventArgs e)
    {
        print("I died");
        Stop();
    }

    protected virtual void OnMeleeHitEvent()
    {
        if (MeleeHitEvent != null)
        {
            MeleeHitEvent();
        }
    }

    protected virtual void OnMeleeAttackEvent()
    {
        if (MeleeAttackEvent != null)
        {
            MeleeAttackEvent();
        }
    }

    protected virtual void OnRangedAttackEvent()
    {
        if (RangedAttackEvent != null)
        {
            RangedAttackEvent();
        }
    }

    protected virtual void OnRangedHitEvent()
    {
        if (RangedHitEvent != null)
        {
            RangedHitEvent();
        }
    }

    void OpponentHealth_Died(object sender, EventArgs e)
    {
        print("Died");
        Stop();
    }
    protected override void Initialize()
    {
        
        hammer = transform.FindChild("Hammer").GetComponent<HammerAttack>();
        hammer.ActivateAnimations();

        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (lineRenderer.enabled)
        {
            laserTimer += Time.deltaTime;
            if (laserTimer > LaserExposure)
            {
                lineRenderer.enabled = false;
            }
        }
    }
   
    public void SetBrains(IBlackBox _brain1, IBlackBox _brain2, IBlackBox _brain3, IBlackBox _brain4)
    {
        this.brain1 = _brain1;
        this.brain2 = _brain2;
        this.brain3 = _brain3;
        this.brain4 = _brain4;
    }

    public void SwitchBrain(int number)
    {
        ActiveBrain = number;
        switch (number)
        {
            case 1:
                if (brain1 != null)
                {
                    this.brain = brain1;
                }
                else
                {
                    this.brain = defaultBrain;
                }
                break;
            case 2:
                if (brain2 != null)
                {
                    this.brain = brain2;
                }
                else
                {
                    this.brain = defaultBrain;
                }
                break;
            case 3:
                if (brain3 != null)
                {
                    this.brain = brain3;
                }
                else
                {
                    this.brain = defaultBrain;
                }
                break;
            case 4:
                if (brain4 != null)
                {
                    this.brain = brain4;
                }
                else
                {
                    this.brain = defaultBrain;
                }
                break;
            default:
                this.brain = defaultBrain;
                break;
        }

        this.brain.ResetState();
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

    protected override bool CanRun()
    {
        return true;
    }

    protected override void MeleeAttack()
    {
       // MeleeAttackEvent();
        hammer.PerformAttack();
        if (TargetIsInMeleeBox)
        {
            OnMeleeHitEvent();
            float dmg = 0;
            if (!DummyAttack)
            {
                dmg = MeleeWeapon.MinimumDamage + UnityEngine.Random.value * (MeleeWeapon.MaximumDamage - MeleeWeapon.MinimumDamage);
            }
            OpponentHealth.TakeDamage(dmg);
        }
        
    }

    protected override void RangedAttack()
    {
        RaycastHit hit;
        bool isHit = false;
        OnRangedAttackEvent();
        Vector3 s_0 = transform.position + transform.forward * 1.1f;
        Vector3 p_0 = s_0 + transform.forward * 50;
        Vector3 t = new Vector3(p_0.x, transform.position.y, p_0.z);
        Vector3 dir = Vector3.Normalize(t - s_0);
        if (Physics.Raycast(s_0, dir, out hit, 50))
        {
            isHit = true;
            if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
            {
                // Do damage
         //       print("Ranged Damage!");
                OnRangedHitEvent();
                float dmg = 0;
                if (!DummyAttack)
                {
                    dmg = RangedWeapon.MinimumDamage + UnityEngine.Random.value * (RangedWeapon.MaximumDamage - RangedWeapon.MinimumDamage);
                }
                if (OpponentHealth != null)
                {
                    OpponentHealth.TakeDamage(dmg);
                }
            }

        }
        Vector3 point;
        if (isHit)
        {
            point = hit.point;
        }
        else
        {
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

    protected override void MortarAttack(float mortarForce)
    {
        throw new System.NotImplementedException();
    }

    protected override void FitnessStats(float moveDist, float turnAngle, float turretTurnAngle, float pickup_sensor, float on_target, float turret_on_target)
    {
        // Do nothing
    }

    
}
