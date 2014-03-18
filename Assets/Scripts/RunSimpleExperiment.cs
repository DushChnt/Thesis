using UnityEngine;
using System.Collections;
using System.Xml;
using SharpNeat.Genomes.Neat;
using SharpNeat.EvolutionAlgorithms;
using System;

public class RunSimpleExperiment : MonoBehaviour {

    static NeatEvolutionAlgorithm<NeatGenome> _ea;

	// Use this for initialization
	void Start () {
        Debug.Log("Starting XOR experiment");
        SimpleExperiment experiment = new SimpleExperiment();

        XmlDocument xmlConfig = new XmlDocument();
        xmlConfig.Load(@"Assets\Scripts\xor.config.xml");
        experiment.Initialize("XOR", xmlConfig.DocumentElement);

        _ea = experiment.CreateEvolutionAlgorithm();

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

        _ea.StartContinue();
	}

    static void ea_UpdateEvent(object sender, EventArgs e)
    {
        Debug.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
                                _ea.CurrentGeneration, _ea.Statistics._maxFitness));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
