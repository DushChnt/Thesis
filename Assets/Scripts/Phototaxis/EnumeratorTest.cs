using UnityEngine;
using System.Collections;

public class EnumeratorTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine( Run() );
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private IEnumerator Run()
    {
        print("Running at " + System.DateTime.Now);
        yield return StartCoroutine(evaluate());
        print("Stopping at " + System.DateTime.Now);
    }

    private IEnumerator evaluate()
    {
        for (int i = 0; i < 5; i++)
        {
            print("Running iteration " + i + " at " + System.DateTime.Now);
            yield return new WaitForSeconds(2);
        }
    }
}
