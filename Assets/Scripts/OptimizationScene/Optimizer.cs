using UnityEngine;
using System.Collections;
using SharpNeat.Domains;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using SharpNeat.Phenomes;

public class Optimizer : MonoBehaviour {

    OptimizationExperiment experiment;
    static NeatEvolutionAlgorithm<NeatGenome> _ea;
    public float Duration;
    public float StoppingFitness;
    private DateTime startTime;
    String filePath;
	// Use this for initialization
	void Start () {
	
	}

    public void Evaluate(IBlackBox box)
    {

    }

    public void StopEvaluation(IBlackBox box)
    {

    }

    public float GetFitness(IBlackBox box)
    {


        return 0;
    }

    public void StartEA()
    {
        Utility.Log("Starting PhotoTaxis experiment");
        experiment = new OptimizationExperiment();

        XmlDocument xmlConfig = new XmlDocument();
        xmlConfig.Load(@"Assets\Scripts\phototaxis.config.xml");
        experiment.SetOptimizer(this);
        experiment.Initialize("PhotoTaxis", xmlConfig.DocumentElement);

        _ea = experiment.CreateEvolutionAlgorithm();

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);

        startTime = DateTime.Now;
        Time.timeScale = 15;
        _ea.StartContinue();
    }

    static void ea_UpdateEvent(object sender, EventArgs e)
    {       
        Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));
    }

    void ea_PauseEvent(object sender, EventArgs e)
    {
        Utility.Log("Done ea'ing (and neat'ing)");

        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        // Save genomes to xml file.
        string popFilePath = @"Assets\Scripts\Populations\phototaxis.pop.xml";
        using (XmlWriter xw = XmlWriter.Create(popFilePath, _xwSettings))
        {
            experiment.SavePopulation(xw, _ea.GenomeList);
        }
        // Also save the best genome

        using (XmlWriter xw = XmlWriter.Create(filePath, _xwSettings))
        {
            experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
        }
        DateTime endTime = DateTime.Now;
        Utility.Log("Total time elapsed: " + (endTime - startTime));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
