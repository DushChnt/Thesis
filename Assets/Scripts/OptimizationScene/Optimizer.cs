using UnityEngine;
using System.Collections;
using SharpNeat.Domains;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using SharpNeat.Phenomes;
using System.Collections.Generic;
using System.IO;

public class Optimizer : MonoBehaviour {

    OptimizationExperiment experiment;
    static NeatEvolutionAlgorithm<NeatGenome> _ea;
    public float Duration { get { return TrialDuration * Trials; } }
    public int Trials;
    public float TrialDuration;
    public float StoppingFitness;
    private DateTime startTime;
    public GameObject Robot;
    public GameObject Target;
    
    string filePath = @"Assets\Scripts\Populations\optimizerChamp.gnm.xml";
    string popFilePath;

    Dictionary<IBlackBox, RobotController> dict = new Dictionary<IBlackBox, RobotController>();
	// Use this for initialization
    void Start()
    {
        experiment = new OptimizationExperiment();
        XmlDocument xmlConfig = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load("phototaxis.config");
        //      xmlConfig.Load(OptimizerParameters.ConfigFile);
        xmlConfig.LoadXml(textAsset.text);
        experiment.SetOptimizer(this);
        experiment.Initialize(OptimizerParameters.Name, xmlConfig.DocumentElement, OptimizerParameters.NumInputs, OptimizerParameters.NumOutputs);
        //filePath = string.Format(@"Assets\Scripts\Populations\{0}Champ.gnm.xml", OptimizerParameters.Name);
        //popFilePath = string.Format(@"Assets\Scripts\Populations\{0}.pop.xml", OptimizerParameters.Name);


        filePath = Application.persistentDataPath + string.Format("/Populations/{0}Champ.gnm.xml", OptimizerParameters.Name);
        popFilePath = Application.persistentDataPath + string.Format("/Populations/{0}.pop.xml", OptimizerParameters.Name);
        popFilePath = Application.persistentDataPath + string.Format("/Populations/{0}.pop.xml", "MyPopulation3");
    }

    public void Evaluate(IBlackBox box)
    {
        // Random starting point in radius 20
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        Vector3 pos = dir.normalized * 20;
        pos.y = 1;
        GameObject obj = Instantiate(Robot, pos, Quaternion.identity) as GameObject;
        RobotController robo = obj.GetComponent<RobotController>();
        Target.transform.position = new Vector3(0, 1, 0);
        dict.Add(box, robo);
        robo.Activate(box, Target);
    }

    public void RunBest()
    {
        Time.timeScale = 1;
        NeatGenome genome = null;
        

        // Try to load the genome from the XML document.
        try
        {
            using (XmlReader xr = XmlReader.Create(filePath))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];
        }
        catch (Exception e1)
        {
            print(filePath + " Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message + "\nJoe: " + filePath);
            return;
        }

        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = experiment.CreateGenomeDecoder();

        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);

        // Reset();

        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        Vector3 pos = dir.normalized * 20;
        pos.y = 1;
        GameObject obj = Instantiate(Robot, pos, Quaternion.identity) as GameObject;
        RobotController robo = obj.GetComponent<RobotController>();
        robo.RunBestOnly = true;
   
        Target.GetComponent<TargetController>().Activate();
        robo.Activate(phenome, Target);
    }

    public void StopEvaluation(IBlackBox box)
    {
        RobotController robo = dict[box];
        robo.Stop();
        Destroy(robo.gameObject);
    }

    public float GetFitness(IBlackBox box)
    {
        if (dict.ContainsKey(box))
        {
            float fit = dict[box].GetFitness();
            // print("Fitness: " + fit);
            return fit;
        }
        return 0.0f;
    }

    public void StartEA()
    {
        Utility.DebugLog = true;
        Utility.Log("Starting PhotoTaxis experiment");        

        _ea = experiment.CreateEvolutionAlgorithm(popFilePath);

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);

        startTime = DateTime.Now;
        Time.timeScale = 15;
        Target.GetComponent<TargetController>().Activate();
        _ea.StartContinue();
    }

    public void StopEA()
    {
        Target.GetComponent<TargetController>().Stop();
        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
        {
            _ea.Stop();
        }
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
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + "Populations");
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory"); 
            dirInf.Create();
        }
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
