using UnityEngine;
using System.Collections;

public class TrainingController : LevelController {

    private float DistanceMoved, TurnAmount, TurretTurnAmount, KeepDistanceCount, ReachDistanceCount = 1000, ReachedTick, FaceTarget, ticks;
    private int MeleeAttacks, MeleeHits, RangedAttacks, RangedHits;
    private float MaxMeleeAttacks, MaxRangedAttacks;

	// Use this for initialization
	void Start () {
        if (MeleeWeapon != null)
        {
            MaxMeleeAttacks = MeleeWeapon.AttackSpeed * 20; // Should be Optimizer.TrialDuration          
        }
        if (RangedWeapon != null)
        {
            MaxRangedAttacks = RangedWeapon.AttackSpeed * 20; // Should be Optimizer.TrialDuration
        }
	}	

    protected override bool CanRun()
    {
        return true;
    }

    protected override void MeleeAttack()
    {
        MeleeAttacks++;
        RaycastHit hit;

        // Check raycast a short distance in front of the robot
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5, layerMask))
        {

            if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
            {
                MeleeHits++;
            }
        }

    }

    protected override void RangedAttack()
    {

        RangedAttacks++;
        RaycastHit hit;

        // Check raycast a long distance in front of the robot
        if (Physics.Raycast(transform.position, transform.forward, out hit, 50, layerMask))
        {

            if (hit.collider.tag.Equals("Target") || hit.collider.tag.Equals("Robot"))
            {
                RangedHits++;
            }

        }

    }

    protected override void MortarAttack(float mortarForce)
    {
        
    }

    private float InverseDistance(float x)
    {
        return 20 / (((x * x) / 20) + 1);
    }

    protected override void FitnessStats(float moveDist, float turnAngle, float turretTurnAngle, float pickup_sensor, float on_target, float turret_on_target)
    {
        ticks++;

        DistanceMoved += moveDist;
        TurnAmount += turnAngle;
        TurretTurnAmount += turretTurnAngle;

        float dist = GetDistance();
        float desiredDist = Mathf.Abs(dist - Settings.Brain.DistanceToKeep); // Close to 0 is better

        KeepDistanceCount += InverseDistance(desiredDist);

        //if (desiredDist < 5)
        //{
        //    KeepDistanceCount += 10;
        //}
        //else if (desiredDist < 10)
        //{
        //    KeepDistanceCount += 5;
        //}
        //else if (desiredDist < 18)
        //{
        //    KeepDistanceCount += 2;
        //}
        if (desiredDist < ReachDistanceCount)
        {
            ReachDistanceCount = desiredDist;
            ReachedTick = ticks;
        }

        FaceTarget += on_target;
        
    }

    public float GetDistance()
    {
        if (Target != null)
        {
            Vector2 a = new Vector2(Target.transform.position.x, Target.transform.position.z);
            Vector2 b = new Vector2(transform.position.x, transform.position.z);
            return Vector2.Distance(a, b);
        }
        return 0.0f;
    }

    public override void Activate(SharpNeat.Phenomes.IBlackBox box, GameObject target)
    {
        this.brain = box;
        this.brain.ResetState();
        this.Target = target;
        this.IsRunning = true;
    }

    public float GetFitness()
    {
        return GetAdvancedFitness();
    }

    private float GetAdvancedFitness()
    {
        if (DistanceMoved + TurnAmount < 2)
        {
            return 0;
        }
        float fit = 1000;

        float moveAround = DistanceMoved / ticks * Settings.Brain.MoveAround;
        fit += moveAround;

        float reachDistance = (1 - ReachDistanceCount / 50f) * Settings.Brain.ReachDistance; // *(1 - ReachedTick / ticks);
        fit += reachDistance;

        float keepDistance = (KeepDistanceCount / (10 * ticks)) * Settings.Brain.KeepDistance;
        fit += keepDistance;

        float faceTarget = FaceTarget / ticks * Settings.Brain.FaceTarget;
        fit += faceTarget;

        if (Player.CanUseMelee && Player.MeleeWeapon != null)
        {

            float meleeAttacks = MeleeAttacks / MaxMeleeAttacks * Settings.Brain.MeleeAttacks;
            fit += meleeAttacks;                     

            float meleeHits = MeleeHits / MaxMeleeAttacks * Settings.Brain.MeleeHits;
            fit += MeleeHits;

            float meleePrecision = MeleeAttacks > 0 ? (float)MeleeHits / (float)MeleeAttacks : 0;
            meleePrecision *= Settings.Brain.MeleePrecision;
            fit += meleePrecision;

        }

        if (Player.CanUseRanged && Player.RangedWeapon != null)
        {
            float rangedAttacks = RangedAttacks / MaxRangedAttacks * Settings.Brain.RifleAttacks;
            fit += rangedAttacks;

            float rangedHits = RangedHits / MaxRangedAttacks * Settings.Brain.RifleHits;
            fit += rangedHits;

            //if (RangedHits > 0)
            //{
            //    print("Ranged hits: " + RangedHits + ", score: " + rangedHits + 
            //        ". Facetarget: " + FaceTarget + ", score: " + faceTarget +
            //        ". Ranged attacks: " + RangedAttacks + ", score: " + rangedAttacks);
            //}

            float rangedPrecision = RangedAttacks > 0 ? RangedHits / RangedAttacks : 0;
            rangedPrecision *= Settings.Brain.RiflePrecision;
            fit += rangedPrecision;
        }

        return fit;
    }

    protected override void Initialize()
    {
        // Do nothing;
    }
}
