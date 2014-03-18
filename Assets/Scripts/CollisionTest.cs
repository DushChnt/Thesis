using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTest : MonoBehaviour {

    List<Collider> currentlyHitting;
    List<Collider> notHitting;

	// Use this for initialization
	void Start () {
        print("Yo");
        currentlyHitting = new List<Collider>();
        notHitting = new List<Collider>();
	}

    void UpdateVisibility()
    {
        // Update visibility status
        RaycastHit hit;
        Vector3 direction;
        List<Collider> changes = new List<Collider>();
        bool hasChanged = false;
        foreach (Collider c in notHitting)
        {
            direction = c.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (Physics.Raycast(transform.position, direction, out hit, 100f))
            {
                if (hit.collider.Equals(c))
                {
                    // Now we see it
                    changes.Add(c);
                    hasChanged = true;
                }
            }
        }
        foreach (Collider c in changes)
        {
            notHitting.Remove(c);
            currentlyHitting.Add(c);
        }
        changes = new List<Collider>();

        foreach (Collider c in currentlyHitting)
        {
            direction = c.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (Physics.Raycast(transform.position, direction, out hit, 100f))
            {
                if (!hit.collider.Equals(c))
                {
                    // Now we don't see it
                    changes.Add(c);
                    hasChanged = true;
                }
            }
        }
        foreach (Collider c in changes)
        {
            currentlyHitting.Remove(c);
            notHitting.Add(c);
        }

        if (hasChanged)
        {
            print("Currently Hitting: " + currentlyHitting.Count + ", not hitting: " + notHitting.Count);
        }
        SimpleGUI.CurrentlyHitting = currentlyHitting.Count;
        SimpleGUI.NotHitting = notHitting.Count;
    }

	// Update is called once per frame
	void Update () {
        UpdateVisibility();
        
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Target"))
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
                    if (!currentlyHitting.Contains(other))
                    {
                        currentlyHitting.Add(other);
                    }
                }
                else
                {
                    print("Obstacle in the way");
                    if (!notHitting.Contains(other))
                    {
                        notHitting.Add(other);
                    }
                }

            }


            print("Currently hitting " + currentlyHitting.Count);
            print("Not hitting " + notHitting.Count);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Target"))
        {
            print("Exit trigger");

            currentlyHitting.Remove(other);
            notHitting.Remove(other);

            print("Currently hitting " + currentlyHitting.Count);
        }
    }
}
