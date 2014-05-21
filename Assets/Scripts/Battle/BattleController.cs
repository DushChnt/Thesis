using UnityEngine;
using System.Collections;

public class BattleController : BaseController {

    public BattleController Opponent;
    public HealthScript Health;
    public float MeleeDamage = 10f;

	// Use this for initialization
	void Start () {
        turret = gameObject.transform.FindChild("Turret");
        this.Health = gameObject.AddComponent<HealthScript>();
        HitLayers = 1 << LayerMask.NameToLayer("Robot");
	}
	
	// Update is called once per frame
	void Update () {
	    
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

    public override void Activate(SharpNeat.Phenomes.IBlackBox box, GameObject target)
    {
        this.isRunning = true;
        this.brain = box;
        this.brain.ResetState();
        this.target = target;
        this.Opponent = target.GetComponent<BattleController>();
        this.Health = gameObject.AddComponent<HealthScript>();
    }

    public override void Stop()
    {
        this.isRunning = false;
    }

    protected override void MortarAttack(float distance)
    {
      //  throw new System.NotImplementedException();
    }

    public override void ReceiveMortarInfo(float hitRate, float distFromCenterSquared)
    {
     //   throw new System.NotImplementedException();
    }
}
