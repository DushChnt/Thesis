using UnityEngine;
using System.Collections;

public class OpponentController : MonoBehaviour {

    public GameObject Target;
    OpponentState state;
    public float Speed = 4;
    public float TurnSpeed = 180;
    bool IsRunning;

	// Use this for initialization
	void Start () {
        state = OpponentState.Stationary;
        IsRunning = true;
	}
	
	// Update is called once per frame
	void Update () {
        DoMovement();
        
	}

    public void Stop()
    {
        this.IsRunning = false;
    }

    public void SwitchMovement(OpponentState state)
    {
        this.state = state;
        IsRunning = true;
    }

    void DoMovement()
    {
        if (IsRunning)
        {
            switch (state)
            {
                case OpponentState.Stationary:

                    break;
                case OpponentState.Approaching:
                    ApproachMovment();
                    break;
                case OpponentState.Fleeing:
                    FleeMovement();
                    break;
            }
        }
    }

    void FleeMovement()
    {
        var direction = Target.transform.position - transform.position;
        var angle = Utility.AngleSigned(transform.forward, direction, transform.up);
        var steer = 0;
        var gas = 1;
        if (angle < -2)
        {
            steer = 1;
        }
        else if (angle > 2)
        {
            steer = -1;
        }

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }

    void ApproachMovment()
    {
        var direction = Target.transform.position - transform.position;
        var angle = Utility.AngleSigned(transform.forward, direction, transform.up);
        var steer = 0;
        var gas = 1;
        if (angle < -2)
        {
            steer = -1;
        }
        else if (angle > 2)
        {
            steer = 1;
        }

        var moveDist = gas * Speed * Time.deltaTime;
        var turnAngle = steer * TurnSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, turnAngle, 0));
        transform.Translate(Vector3.forward * moveDist);
    }

    
}
public enum OpponentState
{
    Stationary,
    Approaching,
    Fleeing
}
