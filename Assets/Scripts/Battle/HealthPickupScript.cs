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
        print("Health pickup");
        print("Tag:" + other.tag);
        if (other.tag.Equals("Robot"))
        {
            BaseController robot = other.gameObject.transform.parent.GetComponent<BaseController>();
            robot.PickupHealth();
            Destroy(gameObject);
        }
       
    }
}
