using UnityEngine;
using System.Collections;

public class TrainingController : LevelController {

    private float DistanceMoved, TurnAmount, TurretTurnAmount, KeepDistanceCount, ReachDistanceCount = 1000, ReachedTick, FaceTarget, ticks;
    private int MeleeAttacks, MeleeHits, RangedAttacks, RangedHits, MortarAttacks, MortarHits;
    private float MortarDamage;
    private float MaxMeleeAttacks, MaxRangedAttacks, MaxMortarAttacks;
    private float AimTurret;

    const float A = 19.8232173425f;
    const float B = 1.3229827143f;
    const float C = 0.5331043504f;

    const float At = 1.8050623168f;
    const float Bt = 0.3424113095f;

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
        if (MortarWeapon != null)
        {
            MaxMortarAttacks = MortarWeapon.AttackSpeed * 20; // Should be Optimizer.TrialDuration
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
        MortarAttacks++;
        if (turret != null)
        {
            //GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
            //Rigidbody body = m.GetComponent<Rigidbody>();
            //m.GetComponent<MortarHit>().MortarCollision += new MortarHit.MortarEventHandler(TrainingController_MortarCollision);
            //var direction = (turret.forward + turret.up * 1f) * mortarForce * MaxMortarForce;
            //body.AddForce(direction);

            SimulateMortarAttack(mortarForce, turret.position, turret.forward, true);
        }
    }

    private void SimulateMortarAttack(float mortarForce, Vector3 startPos, Vector3 direction, bool simulateTime)
    {       
        float distance = A * mortarForce * mortarForce + B * mortarForce + C;
        Vector3 hitPoint = startPos + direction.normalized * distance;

        if (simulateTime)        {          

            float time = At * mortarForce + Bt;

            // print("Simulated: F: " + mortarForce + "; D: " + distance + "; H: " + hitPoint);
            StartCoroutine(simulate(mortarForce, hitPoint, time));
        }
        else
        {
            MortarResult(mortarForce, hitPoint);
        }
    }

    private void MortarResult(float mortarForce, Vector3 hitPoint)
    {
        if (this != null && transform != null)
        {
            float x = hitPoint.x;
            float z = hitPoint.z;

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
                MortarHits++;
                MortarDamage += dmg;
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

            }
        }
    }

    private IEnumerator simulate(float mortarForce, Vector3 hitPoint, float time)
    {
        yield return new WaitForSeconds(time);
      //  print("Simulated: F: " + mortarForce + "; T: " + time + "; H: " + hitPoint);
        MortarResult(mortarForce, hitPoint);
    }

    void TrainingController_MortarCollision(object sender, MortarEventArgs args)
    {
        if (this != null && transform != null)
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
                MortarHits++;
                MortarDamage += dmg;
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

            }
        }
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
        AimTurret += turret_on_target;
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

    public FitnessDetails GetFitness()
    {
        return GetAdvancedFitness();
    }

    private FitnessDetails GetAdvancedFitness()
    {
        FitnessDetails details = new FitnessDetails();
        if (DistanceMoved + TurnAmount < 2)
        {
            return details;
        }
        float fit = 1000;

        details.DistanceMoved = DistanceMoved;

        float moveAround = DistanceMoved / ticks * Settings.Brain.MoveAround;
        fit += moveAround;

        details.ReachDistance = ReachDistanceCount;

        float reachDistance = (1 - ReachDistanceCount / 50f) * Settings.Brain.ReachDistance; // *(1 - ReachedTick / ticks);
        fit += reachDistance;

        details.KeepDistance = KeepDistanceCount;

        float keepDistance = (KeepDistanceCount / (10 * ticks)) * Settings.Brain.KeepDistance;
        fit += keepDistance;

        details.FaceTarget = FaceTarget;

        float faceTarget = FaceTarget / ticks * Settings.Brain.FaceTarget;
        fit += faceTarget;

        if (Player.CanUseMelee && Player.MeleeWeapon != null)
        {
            details.MeleeAttacks = MeleeAttacks;
            float meleeAttacks = MeleeAttacks / MaxMeleeAttacks * Settings.Brain.MeleeAttacks;
            fit += meleeAttacks;

            details.MeleeHits = MeleeHits;
            float meleeHits = MeleeHits / MaxMeleeAttacks * Settings.Brain.MeleeHits;
            fit += MeleeHits;

            float meleePrecision = MeleeAttacks > 0 ? (float)MeleeHits / (float)MeleeAttacks : 0;
            details.MeleePrecision = meleePrecision * 100;
            meleePrecision *= Settings.Brain.MeleePrecision;
            fit += meleePrecision;

        }

        if (Player.CanUseRanged && Player.RangedWeapon != null)
        {
            details.RangedAttacks = RangedAttacks;
            float rangedAttacks = RangedAttacks / MaxRangedAttacks * Settings.Brain.RifleAttacks;
            fit += rangedAttacks;

            details.RangedHits = RangedHits;
            float rangedHits = RangedHits / MaxRangedAttacks * Settings.Brain.RifleHits;
            fit += rangedHits;

            //if (RangedHits > 0)
            //{
            //    print("Ranged hits: " + RangedHits + ", score: " + rangedHits + 
            //        ". Facetarget: " + FaceTarget + ", score: " + faceTarget +
            //        ". Ranged attacks: " + RangedAttacks + ", score: " + rangedAttacks);
            //}

            float rangedPrecision = RangedAttacks > 0 ? RangedHits / RangedAttacks : 0;
            details.RangedPrecision = rangedPrecision * 100;
            rangedPrecision *= Settings.Brain.RiflePrecision;
            fit += rangedPrecision;
        }

        if (Player.CanUseMortar && Player.MortarWeapon != null)
        {
            details.AimTurret = AimTurret;
            float aimTurret = AimTurret / ticks * Settings.Brain.TurretFaceTarget;
            fit += aimTurret;

            details.MortarAttacks = MortarAttacks;
            float mortarAttacks = MortarAttacks / MaxMortarAttacks * Settings.Brain.MortarAttacks;
            fit += mortarAttacks;

            details.MortarHits = MortarHits;
            float mortarHits = MortarHits / MaxMortarAttacks * Settings.Brain.MortarHits;
            fit += mortarHits;

            float mortarPrecision = MortarAttacks > 0 ? MortarHits / MortarAttacks : 0;
            details.MortarPrecision = mortarPrecision * 100;
            mortarPrecision *= Settings.Brain.MortarPrecision;
            fit += mortarPrecision;

            float mortarDamagePerHit = MortarAttacks > 0 ? MortarDamage / MortarAttacks : 0;
            details.MortarDamagePerHit = mortarDamagePerHit;
            mortarDamagePerHit *= Settings.Brain.MortarDamagePerHit;
            fit += mortarDamagePerHit;
        }


        details.Fitness = fit; 

        return details;
    }

    protected override void Initialize()
    {
        // Do nothing;
    }

    protected override void SendRPC(bool meleeAttack, bool rangedAttack, bool mortarAttack, float mortarForce)
    {
        // Do nothing
    }
}

public class FitnessDetails
{
    public float Fitness { get; set; }

    // Movement
    public float DistanceMoved { get; set; }
    public float ReachDistance { get; set; }
    public float KeepDistance { get; set; }
    public float FaceTarget { get; set; }

    // Melee
    public float MeleeAttacks { get; set; }
    public float MeleeHits { get; set; }
    public float MeleePrecision { get; set; }
    
    // Ranged
    public float RangedAttacks { get; set; }
    public float RangedHits { get; set; }
    public float RangedPrecision { get; set; }

    public float MortarAttacks { get; set; }
    public float MortarHits { get; set; }
    public float MortarPrecision { get; set; }
    public float MortarDamagePerHit { get; set; }
    public float AimTurret { get; set; }
}
