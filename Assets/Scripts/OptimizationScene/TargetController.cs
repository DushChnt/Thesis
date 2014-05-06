using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour {

    public delegate void PerformMovement();
    PerformMovement movementHandler;
    bool IsActive = false;
    public float MoveDistance = 10;
    public float MoveSpeed = 4;
    private float distanceMoved = 0;
    private bool moveForward = true;    
    Vector3 direction = Vector3.forward;
    float changeProb = 0.5f;
    private Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (IsActive)
        {
            movementHandler();
        }
	}

    public void Activate(Transform target)
    {
        this.target = target;
        movementHandler = new PerformMovement(ApproachMovement);
        IsActive = true;
    }

    public void Activate()
    {
        movementHandler = AssignMovement();
        IsActive = true;
    }

    public void Stop()
    {
        IsActive = false;
    }

    private PerformMovement AssignMovement()
    {
        switch (OptimizerParameters.TargetMoveStrategy)
        {
            case TargetMove.Stationary:
                return new PerformMovement(StationaryMovement);                
            case TargetMove.Simple:
                return new PerformMovement(SimpleMovement);                            
            case TargetMove.Advanced:
                return new PerformMovement(AdvancedMovement);            
            case TargetMove.Random:
                return new PerformMovement(RandomMovement);            
        }
        return null;
    }

    private void StationaryMovement()
    {

    }

    private void SimpleMovement()
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

    private void AdvancedMovement()
    {

    }

    private void ApproachMovement()
    {
        var step = Time.deltaTime * MoveSpeed;
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        direction.Normalize();
        transform.Translate(direction * step);
    }

    private void RandomMovement()
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
        var move = direction * step;
        var check = transform.position + move;
        if (check.x > 47 || check.x < -47 || check.z > 47 || check.z < -47)
        {
            move = -move;
        }
        transform.Translate(move);
    }
}
