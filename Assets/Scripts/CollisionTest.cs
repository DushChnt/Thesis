using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTest : MonoBehaviour {

    List<Collider> currentlyHitting;

	// Use this for initialization
	void Start () {
        print("Yo");
        currentlyHitting = new List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        print("Enter trigger");

        var direction = other.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            if (hit.collider.Equals(other))
            {
                print("Hit the target");
            }

        }

        if (!currentlyHitting.Contains(other))
        {
            currentlyHitting.Add(other);
        }
        print("Currently hitting " + currentlyHitting.Count);
    }

    void OnTriggerExit(Collider other)
    {
        print("Exit trigger");

        currentlyHitting.Remove(other);

        print("Currently hitting " + currentlyHitting.Count);
    }
}
