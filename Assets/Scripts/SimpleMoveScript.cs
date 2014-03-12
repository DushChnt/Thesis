using UnityEngine;
using System.Collections;

public class SimpleMoveScript : MonoBehaviour {

    public GameObject Target;
    public float MoveSpeed = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Target != null) {
            var direction = Vector3.right;
            direction.y = 0;
            direction.Normalize();

            transform.position = transform.position + direction * MoveSpeed;
        }
	}
}
