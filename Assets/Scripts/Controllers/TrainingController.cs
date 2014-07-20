using UnityEngine;
using System.Collections;

public class TrainingController : LevelController {

    private float DistanceMoved, TurnAmount, TurretTurnAmount, KeepDistanceCount, ReachDistanceCount = 1000, ReachedTick, FaceTarget, ticks;

	// Use this for initialization
	void Start () {
	
	}	

    protected override bool CanRun()
    {
        return true;
    }

    protected override void MeleeAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void RifleAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void MortarAttack(float mortarForce)
    {
        throw new System.NotImplementedException();
    }

    protected override void FitnessStats(float moveDist, float turnAngle, float turretTurnAngle, float pickup_sensor, float on_target, float turret_on_target)
    {
        ticks++;

        DistanceMoved += moveDist;
        TurnAmount += turnAngle;
        TurretTurnAmount += turretTurnAngle;

        float dist = GetDistance();
        float desiredDist = Mathf.Abs(dist - Settings.Brain.DistanceToKeep); // Close to 0 is better
        if (desiredDist < 5)
        {
            KeepDistanceCount += 10;
        }
        else if (desiredDist < 10)
        {
            KeepDistanceCount += 2;
        }
        else if (desiredDist < 18)
        {
            KeepDistanceCount += 1;
        }
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
        float fit = 1000;

        float moveAround = DistanceMoved / ticks * Settings.Brain.MoveAround;
        fit += moveAround;

        float reachDistance = (1 - ReachDistanceCount / 50f) * Settings.Brain.ReachDistance; // *(1 - ReachedTick / ticks);
        fit += reachDistance;

        float keepDistance = (KeepDistanceCount / (10 * ticks)) * Settings.Brain.KeepDistance;
        fit += keepDistance;

        float faceTarget = FaceTarget / ticks * Settings.Brain.FaceTarget;
        fit += faceTarget;
        print("Fitness: " + fit);

        if (DistanceMoved + TurnAmount < 2)
        {
            return 0;
        }
        return fit;
    }
}
