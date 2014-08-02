using UnityEngine;
using System.Collections;
using System.IO;

public class ManualController : MonoBehaviour {

    public Transform turret;
    public GameObject Mortar;
    public float MaxMortarForce = 500f;
    public float WaitingTime = 0.2f;
    float wait;
    float force = 0.3f;
    bool isShooting;

	// Use this for initialization
	void Start () {
        isShooting = true;
        using (StreamWriter sw = File.CreateText("MortarLog.csv"))
        {
            sw.WriteLine("Force;Distance;Time");
        }
	}

    private void SimulateMortarAttack(float mortarForce, Vector3 startPos, Vector3 direction, bool simulateTime)
    {
        float A = 19.8232173425f;
        float B = 1.3229827143f;
        float C = 0.5331043504f;

        float distance = A * mortarForce * mortarForce + B * mortarForce + C;
        Vector3 hitPoint = startPos + direction.normalized * distance;

        if (simulateTime)
        {
            A = 1.8050623168f;
            B = 0.3424113095f;

            float time = A * mortarForce + B;

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

    }

    private IEnumerator simulate(float mortarForce, Vector3 hitPoint, float time)
    {
        yield return new WaitForSeconds(time);
        print("Simulated: F: " + mortarForce + "; T: " + time + "; H: " + hitPoint);
        MortarResult(mortarForce, hitPoint);
    }

    private void FireMortar(float mortarForce)
    {
        if (turret != null)
        {
            GameObject m = Instantiate(Mortar, turret.transform.position + Vector3.up, Quaternion.identity) as GameObject;
            Rigidbody body = m.GetComponent<Rigidbody>();
            var time = Time.time;
            m.GetComponent<MortarHit>().MortarCollision += new MortarHit.MortarEventHandler((sender, args) => ManualController_MortarCollision(sender, args, mortarForce, time));
            var direction = (turret.forward + turret.up * 1f) * mortarForce * MaxMortarForce;
            body.AddForce(direction);

            SimulateMortarAttack(mortarForce, turret.position, turret.forward, false);
        }
    }

    void ManualController_MortarCollision(object sender, MortarEventArgs args, float force, float time)
    {

        print("F: " + force + "; D: " + Utility.GetDistance(turret.position, args.CollisionPoint) + "; H: " + args.CollisionPoint + "; T: " + (Time.time - time));
        using (StreamWriter sw = File.AppendText("MortarLog.csv"))
        {
            sw.WriteLine(force + ";" + Utility.GetDistance(turret.position, args.CollisionPoint) + ";" + (Time.time - time));
        }
    }

	// Update is called once per frame
	void Update () {
        var turretTurnAngle = 1 * 180 * Time.deltaTime;
        turret.Rotate(new Vector3(0, turretTurnAngle, 0));
        if (Input.GetKeyDown(KeyCode.M))
        {
            FireMortar(1f);
        }
        if (isShooting)
        {
            wait += Time.deltaTime;
            if (wait > WaitingTime)
            {
                FireMortar(force);
                wait = 0;
                if (force < 1)
                {
                    force += 0.01f;

                }
                else
                {
                    isShooting = false;
                }
            }
        }
	}
}
