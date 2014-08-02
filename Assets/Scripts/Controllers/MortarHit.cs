using UnityEngine;
using System.Collections;

public class MortarHit : MonoBehaviour {

    public delegate void MortarEventHandler(object sender, MortarEventArgs args);    

    public event MortarEventHandler MortarCollision;

    protected virtual void OnMortarCollision(MortarEventArgs args)
    {
        if (MortarCollision != null)
        {
            MortarCollision(this, args);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        MortarEventArgs args = new MortarEventArgs
        {
            CollisionPoint = this.transform.position
        };
        OnMortarCollision(args);
        Destroy(gameObject);
    }
}

public class MortarEventArgs
{
    public Vector3 CollisionPoint { get; set; }
}
