using UnityEngine;
using System.Collections;

public class HammerHead : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
   //     print("Hammer: " + col.tag);
        if (col.tag.Equals("Robot"))
        {
            print("BANG!");
        }
    }
}
