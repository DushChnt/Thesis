using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;

public class BattleController : BaseController {

    public BattleController Opponent;
    public HealthScript Health;
    public float MortarDamage = 20f;
    public float MeleeDamage = 10f;

    IBlackBox brain1, brain2, brain3, brain4, defaultBrain;

    // Use this for initialization
    void Start () {
        turret = gameObject.transform.FindChild("Turret");
        this.Health = GetComponent<HealthScript>();
        HitLayers = 1 << LayerMask.NameToLayer("Robot");
    }
    
    // Update is called once per frame
    void Update () {
        
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

    protected override void Attack(float distance, float angle)
    {
        if (attackTimer > AttackCoolDown)
        {
            if (distance < MeleeRange && angle > -15 && angle < 15)
            {
                // Do attack

                attackTimer = 0;
                Opponent.Health.TakeDamage(MeleeDamage);
                Debug.DrawLine(transform.position, target.transform.position, Color.blue, 0.1f);
            }
        }
    }

    protected override void RifleAttack()
    {
        if (rifleTimer > RifleCoolDown)
        {
            rifleTimer = 0;
            RaycastHit hit;
            //  Utility.Log("Raycasting");
            Vector3 point = transform.position + transform.forward * SensorRange;
            bool hitIt = false;
            if (Physics.Raycast(transform.position, transform.forward, out hit, SensorRange, HitLayers))
            {
                //    Utility.Log("Hit " + hit.collider.tag);
                //  if (hit.collider.tag.Equals("Robot"))
                //   {
                point = hit.point;
                hitIt = true;
                //   }
            }

            if (hitIt)
            {
                Debug.DrawLine(transform.position, point, Color.green, 0.1f);
                if (Opponent == null)
                {
                   print("Opponent is null");    
                }
                else
                {
                    if (Opponent.Health == null)
                    {
                        print("Opponent.Health is null");
                    }
                }
                Opponent.Health.TakeDamage(2f);
            }
            else
            {
                Debug.DrawLine(transform.position, point, Color.red, 0.1f);
            }

            //if (turret != null)
            //{
            //    point = turret.position + turret.forward * SensorRange;
            //    Debug.DrawLine(turret.position, point, Color.cyan, 0.1f);
            //}
        }

    }

    public override void Activate(IBlackBox box, GameObject target)
    {
        this.defaultBrain = box;
        this.isRunning = true;
        this.brain = box;
        this.brain.ResetState();
        this.target = target;
        this.Opponent = target.GetComponent<BattleController>();
        this.Health = GetComponent<HealthScript>();
    }

    public override void Stop()
    {
        this.isRunning = false;
    }

    protected override void MortarAttack(float force)
    {
        if (mortarTimer > MortarCoolDown)
        {

            //    print("Force: " + force);
          //  MortarAttacks++;
            mortarTimer = 0;
            Speed -= MortarSpeedPenalty;
            RecoveryRate = MortarRecoveryRate;
            if (turret != null)
            {

                GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
                Rigidbody body = m.GetComponent<Rigidbody>();
                IList<GameObject> targets = new List<GameObject>() { target };
                m.GetComponent<MortarImpact>().SetTargets(targets, this);
                var direction = (turret.forward + turret.up * 1f) * force * MaxMortarForce;
                body.AddForce(direction);
            }
        }
    }

    public override void ReceiveMortarInfo(float hitRate, float distFromCenterSquared)
    {
        if (hitRate > -1)
        {
            float dmg = hitRate * MortarDamage;
            Opponent.Health.TakeDamage(dmg);
        }
       
    }

    void OnDestroy()
    {
        Destroy(Health.FollowScript.gameObject);
    }
}
