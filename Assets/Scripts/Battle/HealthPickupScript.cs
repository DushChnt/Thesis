using UnityEngine;
using System.Collections;

public class HealthPickupScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {        
        if (other.tag.Equals("Robot"))
        {
            // print("HEALTH: Hitting a robot");
            FightController robot = other.gameObject.transform.parent.GetComponent<FightController>();
            robot.PickupHealth();
            Destroy(gameObject);
        }
       
    }
}
