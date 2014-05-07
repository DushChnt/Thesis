using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Collections.Generic;

public class RobotController : BaseController
{
    private int RifleAttacks;
    private int RifleHits;
    private int Hits;

    

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

    protected override void RifleAttack()
    {
        if (rifleTimer > RifleCoolDown)
        {
            RifleAttacks++;
            rifleTimer = 0;
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

    protected override void MortarAttack(float distance)
    {
       
        if (mortarTimer > MortarCoolDown)
        {
            mortarTimer = 0;
            
            if (turret != null)
            {
                
                GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
                Rigidbody body = m.GetComponent<Rigidbody>();
                IList<GameObject> targets = new List<GameObject>();
                targets.Add(target);
                m.GetComponent<MortarImpact>().SetTargets(targets, this);
                var direction = (turret.forward + turret.up * 1f) * distance;
                body.AddForce(direction);
            }
        }
    }

    protected override void Attack(float distance, float angle)
    {
        if (attackTimer > AttackCoolDown)
        {
            if (distance < MeleeRange && angle > -15 && angle < 15)
            {
                // Do attack

                attackTimer = 0;
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
        float precision = RifleAttacks > 0 ? (RifleHits / RifleAttacks) * OptimizerParameters.WRiflePrecision : 0;
        fit += precision;

        // Angle fitness

        float angle = Utility.Clamp(totalAngle / ticks);
        angle *= OptimizerParameters.WAngleTowards;
        fit += angle;

        return fit;
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

    public override void Activate(IBlackBox box, GameObject target)
    {
        isRunning = true;
        this.brain = box;
        this.brain.ResetState();
        this.target = target;
        this.startPos = transform.position;
        this.health = this.GetComponent<HealthScript>();
    //    this.opponent = target.GetComponent<RobotController>();
    }

    public override void Stop()
    {
        this.isRunning = false;
    }



    public override void ReceiveMortarInfo(float hitRate)
    {
        // Do something
        print("Receiving info");
    }
}
