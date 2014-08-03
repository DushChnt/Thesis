using UnityEngine;
using System.Collections;

public class MortarHit : Photon.MonoBehaviour {

    public delegate void MortarEventHandler(object sender, MortarEventArgs args);    

    public event MortarEventHandler MortarCollision;
    public GameObject Bloom;

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

    [RPC]
    protected void GetPosition(Vector3 position, float time)
    {
        transform.position = position;
    }

    void OnCollisionEnter(Collision collision)
    {
        MortarEventArgs args = new MortarEventArgs
        {
            CollisionPoint = this.transform.position
        };
        Instantiate(Bloom, transform.position, Quaternion.identity);
        OnMortarCollision(args);
        Destroy(gameObject);
    }
}

public class MortarEventArgs
{
    public Vector3 CollisionPoint { get; set; }
}
