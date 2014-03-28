using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using System.Collections.Generic;

public class ParallelEvaluator : IPhenomeEvaluator<IBlackBox>
{
    ulong _evalCount;
    bool _stopConditionSatisfied;
    ParallelStartEvaluation se;
    FitnessInfo fitness;

    Dictionary<IBlackBox, FitnessInfo> dict = new Dictionary<IBlackBox, FitnessInfo>();

    public ulong EvaluationCount
    {
        get { return _evalCount; }
    }

    public bool StopConditionSatisfied
    {
        get { return _stopConditionSatisfied; }
    }

    public ParallelEvaluator(ParallelStartEvaluation se)
    {
        this.se = se;
    }

    public IEnumerator Evaluate(IBlackBox box)
    {
        if (se != null)
        {

            se.Evaluate(box);
            yield return new WaitForSeconds(se.Duration);
            se.StopEvaluation(box);
            float fit = se.GetFitness(box);
            if (fit > se.StoppingFitness)
            {
                _stopConditionSatisfied = true;
            }
            FitnessInfo fitness = new FitnessInfo(fit, fit);
            dict.Add(box, fitness);
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


    public FitnessInfo GetLastFitness(IBlackBox phenome)
    {
        if (dict.ContainsKey(phenome))
        {
            FitnessInfo fit = dict[phenome];
            dict.Remove(phenome);
            return fit;
        }
        return FitnessInfo.Zero;
    }
}
