using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class RobotController : BaseController
{
    private int RifleAttacks;
    private int RifleHits;
    private int Hits;
    private int MeleeAttacks;
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
                m.GetComponent<MortarImpact>().SetTargets(targets, this, RunBestOnly);
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
            MeleeAttacks++;
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

    private float GetAdvancedFitness()
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

    private float GetSimpleFitness()
    {
        float fit = 1000;

        if (Settings.Brain.SMovement)
        {
    //        protected float distanceMoved; maximum is ticks
    //protected float closestToTarget = int.MaxValue; maximum is diagonal of arena = 141
            //protected float longestFromTarget = -1; maximum is diagonal of arena = 141
    //protected float sumOfDistToTarget; maximum is diagonal * ticks = 141 * ticks

            //fit += distanceMoved;
            //if (distanceMoved < 2)
            //{
            //    fit -= 500;
            //}
            //// SDistance goes from 0 to 4, 5 values.
            //float approach = 1 / closestToTarget * 100;
            //float dist = Settings.Brain.SDistance - 2;
            //fit += approach * -dist;

            //float flee = longestFromTarget;
            //fit += flee * dist;

            //float sumOfApproach = 1.0f / (sumOfDistToTarget / ticks) * -dist * 100;

            //fit += sumOfApproach;
            const float diagonal = 141;
            float moved = distanceMoved / ticks; // [0, 1]
            float closest = closestToTarget / diagonal;
            float longest = longestFromTarget / diagonal;
            float sum = sumOfDistToTarget / (diagonal * ticks);
            float dist = Settings.Brain.SDistance - 2;
            float towards = totalAngle / ticks;

            fit += towards * 100 * -dist;
            fit += moved * 10;
            fit += (1 - closest) * -dist * 200;
            fit += longest * dist * 200;
            fit += (1 - sum) * -dist * 100;

            if (distanceMoved < 2)
            {
                fit = 1;
            }
        }

        if (Settings.Brain.STurret)
        {
            float turretFit = totalTurretAngle / ticks;
            fit += turretFit * 100;
        }

        if (Settings.Brain.SMelee) 
        {
            float towards = totalAngle / ticks;

            fit += towards * 10;

            float attacks = MeleeAttacks;
            fit += attacks * 10;

            float hits = Hits;
            fit += hits * 100;

            float precision = MeleeAttacks > 0 ? Hits / MeleeAttacks : 0;
            fit += precision * 100;
        }

        if (Settings.Brain.SRifle)
        {
            float towards = totalAngle / ticks;

            fit += towards * 10;

            float rifleAttacks = RifleAttacks;
            fit += rifleAttacks * 10;

            // Rifle fitness
            float rifle = RifleHits;
            fit += rifle * 100;

           

            // Markmanship = precision
            float precision = RifleAttacks > 0 ? ((float)RifleHits / (float)RifleAttacks) : 0;
            fit += precision * 100;
        }

        if (Settings.Brain.SMortar)
        {
            float turretFit = totalTurretAngle / ticks;
            fit += turretFit * 10;

            // Mortar attacks fitness
            float mAttacks = MortarAttacks;
            fit += mAttacks * 10;

            float mHits = MortarHits;
            fit += mHits * 100;

            float mPrecision = MortarAttacks > 0 ? ((float)MortarHits / (float)MortarAttacks) : 0;
            fit += mPrecision * 1000;

            //float mDamage = MortarHits > 0 ? (1 - (AccMortarDamage / (float)MortarAttacks)) * Settings.Brain.mor.WMortarDamage : 0;
            //fit += mDamage;

            float mDamagePerHit = MortarAttacks > 0 ? ((MortarHitDamage / (float)MortarAttacks)) : 0;
            fit += mDamagePerHit * 100;
        }

       // print("Fitness: " + fit);
        return fit;
    }

    public float GetBattleFitness()
    {
        return 0;
    }

    public float GetFitness()
    {
        switch (Settings.Brain.FitnessMode)
        {
            case Brain.SIMPLE:
                return GetSimpleFitness();
            case Brain.ADVANCED:
                return GetAdvancedFitness();
            case Brain.BATTLE:
                return GetBattleFitness();                
            default:
                return GetAdvancedFitness();
        }
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
