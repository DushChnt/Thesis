using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using SharpNeat.Core;

public class PhotoTaxisEvaluator : IPhenomeEvaluator<IBlackBox>
{
    ulong _evalCount;
    bool _stopConditionSatisfied;
    StartEvaluation se;
    FitnessInfo fitness;
    
    public ulong EvaluationCount
    {
        get { return _evalCount; }
    }

    public bool StopConditionSatisfied
    {
        get { return _stopConditionSatisfied; }
    }

    public PhotoTaxisEvaluator(StartEvaluation se)
    {
        this.se = se;
    }

    public IEnumerator Evaluate(IBlackBox box)
    {
        if (se != null)
        {
            se.Evaluate(box);
            yield return new WaitForSeconds(se.Duration);
            se.StopEvaluation();
            float fit = se.GetFitness();
            if (fit > se.StoppingFitness)
            {
                _stopConditionSatisfied = true;
            }
            this.fitness = new FitnessInfo(fit, fit);
            
        }
    }
    
    public void Reset()
    {
        this.fitness = FitnessInfo.Zero;
        
    }

    public FitnessInfo GetLastFitness()
    {
        return this.fitness;
    }
}
