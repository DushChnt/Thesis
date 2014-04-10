using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;

namespace SharpNEAT.Core
{
    class UnityParallelListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {

        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        ParallelStartEvaluation _ps;

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
        /// </summary>
        public UnityParallelListEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                         IPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                          ParallelStartEvaluation ps)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _ps = ps;
        }

        #endregion

        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        public IEnumerator Evaluate(IList<TGenome> genomeList)
        {
            yield return Coroutiner.StartCoroutine(evaluateList(genomeList));
        }

        private IEnumerator evaluateList(IList<TGenome> genomeList)
        {
            Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome>();
            foreach (TGenome genome in genomeList)
            {
                TPhenome phenome = _genomeDecoder.Decode(genome);
                if (null == phenome)
                {   // Non-viable genome.
                    genome.EvaluationInfo.SetFitness(0.0);
                    genome.EvaluationInfo.AuxFitnessArr = null;
                }
                else
                {
                    dict.Add(genome, phenome);
                    Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(phenome));

                    
                }
            }

            yield return new WaitForSeconds(_ps.Duration); 
            foreach (TGenome genome in dict.Keys)
            {
                TPhenome phenome = dict[genome];
                if (phenome != null)
                {
                    FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness(phenome);

                    genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                }
            }
        }

        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }
    }
}
