using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public class FightController : LevelController {

    IBlackBox brain1, brain2, brain3, brain4, defaultBrain;
    int ActiveBrain;

    public override void Activate(IBlackBox box, GameObject target)
    {
        this.defaultBrain = box;
        this.brain = box;
        this.brain.ResetState();
        this.IsRunning = true;
        this.Target = target;
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

    protected override bool CanRun()
    {
        return true;
    }

    protected override void MeleeAttack()
    {
      //  throw new System.NotImplementedException();
    }

    protected override void RangedAttack()
    {
        throw new System.NotImplementedException();
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
