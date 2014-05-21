using UnityEngine;
using System.Collections;

public class NetworkController : BaseController
{

    public BattleController Opponent;
    public HealthScript Health;
    public float MeleeDamage = 10f;

	// Use this for initialization
	void Start () {
        turret = gameObject.transform.FindChild("Turret");
        this.Health = gameObject.AddComponent<HealthScript>();
        HitLayers = 1 << LayerMask.NameToLayer("Robot");
	}	

    protected override void Attack(float dist, float angle)
    {
        if (attackTimer > AttackCoolDown)
        {
            if (dist < MeleeRange && angle > -15 && angle < 15)
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
        throw new System.NotImplementedException();
    }

    protected override void MortarAttack(float force)
    {
        throw new System.NotImplementedException();
    }

    public override void ReceiveMortarInfo(float hitRate, float distFromCenterSquared)
    {
        throw new System.NotImplementedException();
    }

    public override void Activate(SharpNeat.Phenomes.IBlackBox box, GameObject target)
    {
        throw new System.NotImplementedException();
    }

    public override void Stop()
    {
        throw new System.NotImplementedException();
    }
}
