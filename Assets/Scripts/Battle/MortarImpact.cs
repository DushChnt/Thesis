using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MortarImpact : MonoBehaviour {

	IList<GameObject> targets;
	public float DamageRadius = 10f;
	BaseController owner;
	public GameObject Bloom;
    bool RunBestOnly;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTargets(IList<GameObject> targets, BaseController owner, bool runBestOnly)
	{
		this.targets = targets;
		this.owner = owner;
        this.RunBestOnly = runBestOnly;

        
	}

	void OnCollisionEnter(Collision collision)
	{
      //  print("Mortar impact on " + Time.time);
		if (targets != null)
		{
			foreach (GameObject go in targets)
			{
				if (go != null)
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
				//		    print("Hit target, percentage: " + dmg * 100 + "%");

					}
					owner.ReceiveMortarInfo(dmg, distFromCenterSquared);
				}
			}
		}
        if (RunBestOnly)
        {
            Instantiate(Bloom, transform.position, Quaternion.identity);
        }
	  //  print("BOOM!");
		Destroy(gameObject);
	}
}

public class MortarInfo
{

}
