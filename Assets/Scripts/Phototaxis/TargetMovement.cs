using UnityEngine;
using System.Collections;

public class TargetMovement : MonoBehaviour {

    public float MoveDistance = 10;
    public float MoveSpeed = 5;
    private float distanceMoved = 0;
    private bool moveForward = true;
    bool IsRunning = false;

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
            if (moveForward)
            {
                if (distanceMoved + step < MoveDistance)
                {
                    distanceMoved += step;
                    transform.Translate(Vector3.forward * step);
                }
                else
                {
                    moveForward = false;
                }
            }
            else
            {
                if (distanceMoved - step > -MoveDistance)
                {
                    distanceMoved -= step;
                    transform.Translate(Vector3.back * step);
                }
                else
                {
                    moveForward = true;
                }
            }
        }
    }
}
