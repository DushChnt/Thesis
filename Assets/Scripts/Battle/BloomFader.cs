using UnityEngine;
using System.Collections;

public class BloomFader : MonoBehaviour {

    public float ShowTime = 1f;
    float _timer;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        _timer += Time.deltaTime;
        float ratio = 1 - _timer / ShowTime;
        Color color = renderer.material.color;
        color.a = ratio;
        renderer.material.color = color;
        if (_timer > ShowTime)
        {
            Destroy(gameObject);
        }
	}
}
