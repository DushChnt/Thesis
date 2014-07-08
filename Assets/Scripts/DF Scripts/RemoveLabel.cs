using UnityEngine;
using System.Collections;

public class RemoveLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<dfTweenVector3>().TweenCompleted += new TweenNotification(RemoveLabel_TweenCompleted);
	}

    void RemoveLabel_TweenCompleted(dfTweenPlayableBase sender)
    {
        print("OnTweenCompleted");
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void onCompleted()
    {
       
        
    }

    
}
