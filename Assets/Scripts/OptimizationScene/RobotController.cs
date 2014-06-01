using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class RobotController : BaseController
{
    private int RifleAttacks;
    private int RifleHits;
    private int Hits;
    private int MortarAttacks;
    private int MortarHits;
    private float AccMortarDamage;
    private float MortarHitDamage;
    

    // Use this for initialization
    void Start()
    {
        turret = gameObject.transform.FindChild("Turret");
        if (HumanControlled)
        {

            target = GameObject.Find("Target");
            Utility.DebugLog = true;
        }
        else
        {
            int targetLayer = LayerMask.NameToLayer("Target");
            HitLayers = 1 << targetLayer;
        }
    }

    public void HumanActivate(GameObject obj)
    {
        this.target = obj;
        this.isRunning = true;
        this.HumanControlled = true;
        int targetLayer = LayerMask.NameToLayer("Robot");
        HitLayers = 1 << targetLayer;
    }

    protected override void RifleAttack()
    {
        if (rifleTimer > RifleCoolDown)
        {
            RifleAttacks++;
            rifleTimer = 0;
            Speed -= RifleSpeedPenalty;
            RecoveryRate = RifleRecoveryRate;
            RaycastHit hit;
          //  Utility.Log("Raycasting");
            Vector3 point = transform.position + transform.forward * SensorRange;
            bool hitIt = false;
            if (Physics.Raycast(transform.position, transform.forward, out hit, SensorRange, HitLayers))
            {
            //    Utility.Log("Hit " + hit.collider.tag);
                if (hit.collider.tag.Equals("Target"))
                {
                    RifleHits++;
                    point = hit.point;
                    hitIt = true;
                }
            }
            if (RunBestOnly)
            {
                Utility.Log("RifleHits: " + RifleHits);
                if (hitIt)
                {
                    Debug.DrawLine(transform.position, point, Color.green, 0.1f);
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

    }

    protected override void MortarAttack(float force)
    {       
        if (mortarTimer > MortarCoolDown)
        {
         
        //    print("Force: " + force);
            MortarAttacks++;
            mortarTimer = 0;
            Speed -= MortarSpeedPenalty;
            RecoveryRate = MortarRecoveryRate;
            if (turret != null)
            {
                
                GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
                Rigidbody body = m.GetComponent<Rigidbody>();
                IList<GameObject> targets = new List<GameObject>() {target};            
                m.GetComponent<MortarImpact>().SetTargets(targets, this);
                var direction = (turret.forward + turret.up * 1f) * force * MaxMortarForce;
                body.AddForce(direction);
            }
        }
    }

    protected override void Attack(float distance, float angle)
    {
        if (attackTimer > AttackCoolDown)
        {
            attackTimer = 0;
            Speed -= MeleeSpeedPenalty;
            RecoveryRate = MeleeRecoveryRate;
            if (distance < MeleeRange && angle > -15 && angle < 15)
            {
                // Do attack

               
                Hits++;
                if (RunBestOnly)
                {
                    //   sphere.renderer.material = AttackSphereMat;
                    //  AttackShowState = true;
                    Debug.DrawLine(transform.position, target.transform.position, Color.blue, 0.1f);
                    Utility.Log("Attack! Distance: " + distance + ", angle: " + angle);
                    Utility.Log("Joe");
                    //if (opponent != null && opponent.health != null)
                    //{
                        
                    //    opponent.health.TakeDamage(10f + Utility.GenerateNoise(1f));
                    //}
                }
            }
        }

    }

    public float GetFitness()
    {
        //if (Vector3.Distance(transform.position, startPos) < 1)
        //{
        //    return 0;
        //}
        float fit = 0;
        // Approach fitness
        float approach = 1.0f / (totalDistance / ticks) * Settings.Brain.KeepDistance;
        fit += approach;

        // Melee fitness
        float melee = Hits * Settings.Brain.MeleeAttacks;
        fit += melee;

        // Rifle fitness
        float rifle = RifleHits * Settings.Brain.RifleHits;
        fit += rifle;

        float rifleAttacks = RifleAttacks * Settings.Brain.RifleAttacks;
        fit += rifleAttacks;

        // Markmanship = precision
        float precision = RifleAttacks > 0 ? ((float)RifleHits / (float)RifleAttacks) * Settings.Brain.RiflePrecision : 0;
        fit += precision;

        // Angle fitness

        float angle = Utility.Clamp(totalAngle / ticks);
        angle *= Settings.Brain.FaceTarget;
        fit += angle;

        // Turret angle fitness
        float tAngle = Utility.Clamp(totalTurretAngle / ticks);
        tAngle *= Settings.Brain.TurretFaceTarget;
        fit += tAngle;

      //  print("tAngle: " + tAngle);

        // Mortar attacks fitness
        float mAttacks = MortarAttacks * Settings.Brain.MortarAttacks;
        fit += mAttacks;

        float mHits = MortarHits * Settings.Brain.MortarHits;
        fit += mHits;

        float mPrecision = MortarAttacks > 0 ? ((float)MortarHits / (float)MortarAttacks) * Settings.Brain.MortarPrecision : 0;
        fit += mPrecision;

        //float mDamage = MortarHits > 0 ? (1 - (AccMortarDamage / (float)MortarAttacks)) * Settings.Brain.mor.WMortarDamage : 0;
        //fit += mDamage;

        float mDamagePerHit = MortarAttacks > 0 ? ((MortarHitDamage / (float)MortarAttacks)) * Settings.Brain.MortarDamagePerHit : 0;
        fit += mDamagePerHit;

        return fit;
    }

    public float GetFitness2()
    {
        //if (Vector3.Distance(transform.position, startPos) < 1)
        //{
        //    return 0;
        //}
        float fit = 0;
        // Approach fitness
        float approach = 1.0f / (totalDistance / ticks) * OptimizerParameters.WApproach;
        fit += approach;

        // Melee fitness
        float melee = Hits * OptimizerParameters.WMeleeAttack;
        fit += melee;

        // Rifle fitness
        float rifle = RifleHits * OptimizerParameters.WRifleHits;
        fit += rifle;

        float rifleAttacks = RifleAttacks * OptimizerParameters.WRifleAttack;
        fit += rifleAttacks;

        // Markmanship = precision
        float precision = RifleAttacks > 0 ? ((float)RifleHits / (float)RifleAttacks) * OptimizerParameters.WRiflePrecision : 0;
        fit += precision;

        // Angle fitness

        float angle = Utility.Clamp(totalAngle / ticks);
        angle *= OptimizerParameters.WAngleTowards;
        fit += angle;

        // Turret angle fitness
        float tAngle = Utility.Clamp(totalTurretAngle / ticks);
        tAngle *= OptimizerParameters.WTurretAngleTowards;
        fit += tAngle;

        //  print("tAngle: " + tAngle);

        // Mortar attacks fitness
        float mAttacks = MortarAttacks * OptimizerParameters.WMortarAttack;
        fit += mAttacks;

        float mHits = MortarHits * OptimizerParameters.WMortarHits;
        fit += mHits;

        float mPrecision = MortarAttacks > 0 ? ((float)MortarHits / (float)MortarAttacks) * OptimizerParameters.WMortarPrecision : 0;
        fit += mPrecision;

        float mDamage = MortarHits > 0 ? (1 - (AccMortarDamage / (float)MortarAttacks)) * OptimizerParameters.WMortarDamage : 0;
        fit += mDamage;

        float mDamagePerHit = MortarAttacks > 0 ? ((MortarHitDamage / (float)MortarAttacks)) * OptimizerParameters.WMortarDamagePerHit : 0;
        fit += mDamagePerHit;

        return fit;
    }

    public override void Activate(IBlackBox box, GameObject target)
    {
        isRunning = true;
        this.brain = box;
        this.brain.ResetState();
        this.target = target;
        this.startPos = transform.position;
     //   this.health = this.GetComponent<HealthScript>();
    //    this.opponent = target.GetComponent<RobotController>();
    }

    public override void Stop()
    {
        this.isRunning = false;
    }



    public override void ReceiveMortarInfo(float hitRate, float distFromCenterSquared)
    {
        // Do something
      //  print("Receiving info");
        if (hitRate > -1)
        {
            MortarHits++;
            MortarHitDamage += hitRate;
        }
        AccMortarDamage += distFromCenterSquared / (50 * 50);

        if (RunBestOnly)
        {
            print("BOOM, hitrate: " + hitRate + ", dist: " + (distFromCenterSquared / (50 * 50)));
        }
    }

    #region Photon Methods

    private void SendInt()
    {
        if(PhotonNetwork.isMasterClient)
        
        photonView.RPC("GetCoolInt", PhotonTargets.Others, 5, transform.position);
    }

    [RPC]
    private void GetCoolInt(int i, Vector3 position)
    {
        Debug.Log("I got an int over network: " + i);
    }

    #endregion
}
