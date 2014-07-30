using UnityEngine;
using System.Collections;

public class HammerAttack : MonoBehaviour {

    public GameObject Hammer;
    bool isAttacking;
    public Animator animator;

	// Use this for initialization
    void Start()
    {
        animator.enabled = false;
    }

    public void PerformAttack()
    {
        if (animator.enabled)
        {
            animator.SetTrigger("attack");
        }
    }

    public void ActivateAnimations()
    {
        animator.enabled = true;
        animator.Play("hammer idle");
    }

	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.H)) 
        //{
        //    PerformAttack();
        //}
    
	}
}
