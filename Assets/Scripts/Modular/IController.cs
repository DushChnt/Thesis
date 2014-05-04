using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;

public interface IController {

    void Activate(IBlackBox brain, GameObject target);
    void Stop();
    float GetFitness();

}
