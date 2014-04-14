using UnityEngine;
using System.Collections;

public class OptimizerMenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
        var top = 10;
        var stationary_aprroach = "Stationary Approach";
        var buttonClicked = false;
        if (GUI.Button(new Rect(10, top, 200, 30), stationary_aprroach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = stationary_aprroach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var stationary_melee = "Stationary Melee";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), stationary_melee))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = stationary_melee;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Stationary;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        var simple_approach = "Simple Approach";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), simple_approach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = simple_approach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Simple;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var simple_attack = "Simple Attack";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), simple_attack))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = simple_attack;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Simple;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        var random_approach = "Random Approach";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), random_approach))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = random_approach;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Random;
            OptimizerParameters.WMeleeAttack = 0;
            OptimizerParameters.WApproach = 10;
            buttonClicked = true;
        }

        var random_attack = "Random Attack";
        top += 40;
        if (GUI.Button(new Rect(10, top, 200, 30), random_attack))
        {
            OptimizerParameters.Reset();
            OptimizerParameters.Name = random_attack;
            OptimizerParameters.TargetMoveStrategy = TargetMove.Random;
            OptimizerParameters.WMeleeAttack = 1;
            OptimizerParameters.WApproach = 1;
            buttonClicked = true;
        }

        if (buttonClicked)
        {
            Application.LoadLevel("Optimization scene");
        }
    }
}
