using UnityEngine;
using System.Collections;

public class TargetMovement : MonoBehaviour {

    public float MoveDistance = 10;
    public float MoveSpeed = 5;
    private float distanceMoved = 0;
    private bool moveForward = true;
    bool IsRunning = false;
    Vector3 direction = Vector3.forward;
    float changeProb = 0.5f;

	// Use this for initialization
	void Start () {
	    
	}

    public void Activate()
    {
        IsRunning = true;
    }
	
	// Update is called once per frame
    void FixedUpdate()
    {
        if (IsRunning)
        {
            var step = Time.deltaTime * MoveSpeed;
            if (Random.value > changeProb)
            {
                direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                direction.Normalize();
                changeProb = 1f;
            }
            else
            {
                changeProb -= 0.001f;
            }
            transform.Translate(direction * step);
            //if (moveForward)
            //{
            //    if (distanceMoved + step < MoveDistance)
            //    {
            //        distanceMoved += step;
            //        transform.Translate(Vector3.forward * step);
            //    }
            //    else
            //    {
            //        moveForward = false;
            //    }
            //}
            //else
            //{
            //    if (distanceMoved - step > -MoveDistance)
            //    {
            //        distanceMoved -= step;
            //        transform.Translate(Vector3.back * step);
            //    }
            //    else
            //    {
            //        moveForward = true;
            //    }
            //}
        }
    }
}
