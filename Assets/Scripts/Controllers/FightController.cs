using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System;

public class FightController : LevelController {

    IBlackBox brain1, brain2, brain3, brain4, defaultBrain;
    int ActiveBrain;
    HammerAttack hammer;
    bool TargetIsInMeleeBox;
    public FightController Opponent;
    public HealthScript HealthScript;
    public HealthScript OpponentHealth;
    public bool DummyAttack;
    public delegate void AttackEventHandler();
    float laserTimer;
    public float LaserExposure = 0.1f;
    public GameObject Bloom;

    public event AttackEventHandler MeleeAttackEvent;
    public event AttackEventHandler MeleeHitEvent;
    public event AttackEventHandler RangedAttackEvent;
    public event AttackEventHandler RangedHitEvent;
    public event AttackEventHandler MortarAttackEvent;
    public event AttackEventHandler MortarHitEvent;

    LineRenderer lineRenderer;

    public override void Activate(IBlackBox box, GameObject target)
    {
        this.defaultBrain = box;
        this.brain = box;
        this.brain.ResetState();
        this.IsRunning = true;
        this.Target = target;
        FightController opponent = Target.GetComponent<FightController>();
        OpponentHealth = Target.GetComponent<HealthScript>();
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

    protected virtual void OnMortarAttackEvent()
    {
        if (MortarAttackEvent != null)
        {
            MortarAttackEvent();
        }
    }

    protected virtual void OnMortarHitEvent()
    {
        if (MortarHitEvent != null)
        {
            MortarHitEvent();
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

        Bloom = (GameObject) Resources.Load("Mortar Bloom", typeof(GameObject));
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
        return Target != null;
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
        if (!PhotonNetwork.offlineMode)
        {
            photonView.RPC("GetMeleeAttack", PhotonTargets.Others);
        }
    }

    [RPC]
    protected void GetMeleeAttack()
    {
        hammer.PerformAttack();
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
        bool hitOpponent = false;
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
                hitOpponent = true;
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
        if (!PhotonNetwork.offlineMode)
        {
            photonView.RPC("GetRangedAttack", PhotonTargets.Others, hitOpponent, point);
        }
    }

    [RPC]
    protected void GetRangedAttack(bool hit, Vector3 hitPoint)
    {
        Vector3 point = hitPoint;
        if (hit)
        {
            point = Opponent.transform.position;
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
       
        OnMortarAttackEvent();
        GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
        
     //   GameObject m = PhotonNetwork.Instantiate("Mortar", turret.transform.position + Vector3.up, Quaternion.identity, 0);
        Rigidbody body = m.GetComponent<Rigidbody>();
        m.GetComponent<MortarHit>().MortarCollision += new MortarHit.MortarEventHandler(FightController_MortarCollision);
        var direction = (turret.forward + turret.up * 1f) * mortarForce * MaxMortarForce;
        body.AddForce(direction);
        print("----------------------- Mortarforce: " + mortarForce + " direction: " + direction);
        if (!PhotonNetwork.offlineMode)
        {
            photonView.RPC("ShootMortar", PhotonTargets.Others, direction);
        }
    }

    [RPC]
    protected void ShootMortar(Vector3 direction)
    {
        GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
        Rigidbody body = m.GetComponent<Rigidbody>();        
        body.AddForce(direction);
    }

    void FightController_MortarCollision(object sender, MortarEventArgs args)
    {
        if (this != null && transform != null && Target != null)
        {
            float x = args.CollisionPoint.x;
            float z = args.CollisionPoint.z;

            float gx = Target.transform.position.x;
            float gz = Target.transform.position.z;

            float distFromCenterSquared = (gx - x) * (gx - x) + (gz - z) * (gz - z);
            float dmgRadiusSquared = DamageRadius * DamageRadius;
            float dmg = -1;
            if (distFromCenterSquared < dmgRadiusSquared)
            {
                dmg = 1 - distFromCenterSquared / dmgRadiusSquared;
            }

            if (dmg > 0)
            {
                // Hit the target!
                OnMortarHitEvent();
                if (OpponentHealth != null)
                {
                    float damage = 0;
                    
                        
                    if (!DummyAttack)
                    {
                        damage = MortarWeapon.MaximumDamage * dmg;
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
                if (HealthScript != null)
                {
                    float damage = MortarWeapon.MaximumDamage * dmg;
                    if (damage < 1)
                    {
                        damage = 1;
                    }
                    if (!DummyAttack)
                    {
                        HealthScript.TakeDamage(damage);
                    }
                }
            }

            //GameObject mortar = ((MortarHit)sender).gameObject;
            //Instantiate(Bloom, mortar.transform.position, Quaternion.identity);
        }
      
    }

    protected override void FitnessStats(float moveDist, float turnAngle, float turretTurnAngle, float pickup_sensor, float on_target, float turret_on_target)
    {
        // Do nothing
    }



    protected override void SendRPC(bool meleeAttack, bool rangedAttack, bool mortarAttack, float mortarForce)
    {
        // Do photon stuff
        if (!PhotonNetwork.offlineMode)
        {
            photonView.RPC("GetMovement", PhotonTargets.Others, transform.position, transform.rotation, turret != null ? turret.rotation : Quaternion.identity, PhotonNetwork.time);
        }
    }

    [RPC]
    protected void GetMovement(Vector3 realPosition, Quaternion realRotation, Quaternion turretRotation, double time)
    {
        transform.position = realPosition; 
        transform.rotation = realRotation; 
        if (turret != null)
        {
            turret.rotation = turretRotation; 
        }
    }
}
