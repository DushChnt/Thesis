using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MortarImpact : MonoBehaviour {

    IList<GameObject> targets;
    public float DamageRadius = 10f;
    BaseController owner;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTargets(IList<GameObject> targets, BaseController owner)
    {
        this.targets = targets;
        this.owner = owner;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (targets != null)
        {
            foreach (GameObject go in targets)
            {
                // Check if inside area of damage
                float x = transform.position.x;
                float z = transform.position.z;

                float gx = go.transform.position.x;
                float gz = go.transform.position.z;

                float distFromCenterSquared = (gx - x) * (gx - x) + (gz - z) * (gz - z);
                float dmgRadiusSquared = DamageRadius * DamageRadius;
                float dmg = -1;
                if (distFromCenterSquared < dmgRadiusSquared)
                {
                    dmg = 1 - distFromCenterSquared / dmgRadiusSquared;
                //    print("Hit target, percentage: " + dmg * 100 + "%");
                    
                }
                owner.ReceiveMortarInfo(dmg, distFromCenterSquared);
            }
        }

    //    print("BOOM!");
        Destroy(gameObject);
    }
}

public class MortarInfo
{

}
