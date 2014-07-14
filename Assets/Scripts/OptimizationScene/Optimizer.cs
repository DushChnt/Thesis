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
using Parse;
using System.Threading.Tasks;

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

    bool EARunning;

    private uint _generation;
    public uint Generation
    {
        get
        {
            if (EARunning)
            {
                return (uint)(Settings.Brain.Generation + _generation);
            }
            else
            {
                return (uint)Settings.Brain.Generation;
            }
        }
    }
    public string Iteration
    {
        get
        {
            return string.Format("{0} / {1}", OptimizerGUI.CurrentIteration, Trials);
        }
    }
    public string Evolution
    {
        get
        {
            if (FirstFitness > -1)
            {
                double diff = LastFitness - FirstFitness;
                if (_ea != null)
                {
                    diff = _ea.Statistics._bestFitnessMA.Mean - FirstFitness;
                }

                if (diff < 0)
                {                    
                    return string.Format("- {0:0.00}", -diff);
                }
                else
                {
                    return string.Format("+ {0:0.00}", diff);
                }
            }
            return " - ";
        }
    }

    public double LastFitness, FirstFitness = -1;
    
    string champFileLoadPath = @"Assets\Scripts\Populations\optimizerChamp.gnm.xml";
    string popFileLoadPath;

    string popFileSavePath, champFileSavePath;

    Dictionary<IBlackBox, RobotController> dict = new Dictionary<IBlackBox, RobotController>();
    Dictionary<IBlackBox, TargetController> targetDict = new Dictionary<IBlackBox, TargetController>();

	// Use this for initialization
    void Start()
    {
        
        experiment = new OptimizationExperiment();
        XmlDocument xmlConfig = new XmlDocument();
        TextAsset textAsset = (TextAsset)Resources.Load("phototaxis.config");
        //      xmlConfig.Load(OptimizerParameters.ConfigFile);
        xmlConfig.LoadXml(textAsset.text);
        experiment.SetOptimizer(this);
        //experiment.Initialize(OptimizerParameters.Name, xmlConfig.DocumentElement, OptimizerParameters.NumInputs, OptimizerParameters.NumOutputs);
        experiment.Initialize(Settings.Brain.Name, xmlConfig.DocumentElement, Settings.Brain.NumInputs, Settings.Brain.NumOutputs);
        //filePath = string.Format(@"Assets\Scripts\Populations\{0}Champ.gnm.xml", OptimizerParameters.Name);
        //popFilePath = string.Format(@"Assets\Scripts\Populations\{0}.pop.xml", OptimizerParameters.Name);
        OptimizerParameters.MultipleTargets = false;

        //filePath = Application.persistentDataPath + string.Format("/Populations/{0}Champ.gnm.xml", OptimizerParameters.Name);
        //popFilePath = Application.persistentDataPath + string.Format("/Populations/{0}.pop.xml", OptimizerParameters.Name);
        //popFilePath = Application.persistentDataPath + string.Format("/Populations/{0}.pop.xml", "MyPopulation8");

        if (Settings.Brain.IsNewBrain && Settings.Brain.ParentId != null && !Settings.Brain.ParentId.Equals(""))
        {
            champFileLoadPath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ParentId);
            popFileLoadPath = Application.persistentDataPath + string.Format("/{0}/{1}.pop.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ParentId);
        }
        else
        {
            champFileLoadPath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ObjectId);
            popFileLoadPath = Application.persistentDataPath + string.Format("/{0}/{1}.pop.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ObjectId);
        }
        champFileSavePath = Application.persistentDataPath + string.Format("/{0}/{1}.champ.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ObjectId);
        popFileSavePath = Application.persistentDataPath + string.Format("/{0}/{1}.pop.xml", Parse.ParseUser.CurrentUser.Username, Settings.Brain.ObjectId);
        OptimizerGUI.MaxIterations = Trials;

        LastFitness = Settings.Brain.BestFitness;        
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

        if (OptimizerParameters.MultipleTargets)
        {
            GameObject t = Instantiate(Target, new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            TargetController tc = t.AddComponent<TargetController>();
            tc.Activate(obj.transform);
            targetDict.Add(box, tc);
            robo.Activate(box, t);
        }
        else
        {
            robo.Activate(box, Target);
        }
    }

    public void RunBest()
    {
        Time.timeScale = 1;
        NeatGenome genome = null;
        

        // Try to load the genome from the XML document.
        try
        {
            using (XmlReader xr = XmlReader.Create(champFileLoadPath))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];

            
        }
        catch (Exception e1)
        {
            print(champFileLoadPath + " Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message + "\nJoe: " + champFileLoadPath);
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

        if (OptimizerParameters.MultipleTargets)
        {
            GameObject t = Instantiate(Target, new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            TargetController tc = t.AddComponent<TargetController>();
            tc.Activate(obj.transform);
            targetDict.Add(phenome, tc);
            robo.Activate(phenome, t);
        }
        else
        {

            Target.GetComponent<TargetController>().Activate();
            robo.Activate(phenome, Target);
        }
    }

    public void StopEvaluation(IBlackBox box)
    {
        RobotController robo = dict[box];
        robo.Stop();
        if (targetDict.ContainsKey(box))
        {
            targetDict[box].Stop();
            Destroy(targetDict[box].gameObject);
            targetDict.Remove(box);
        }
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
        print("Loading: " + popFileLoadPath);
        _ea = experiment.CreateEvolutionAlgorithm(popFileLoadPath);

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);
        
        startTime = DateTime.Now;
        int evoSpeed = PlayerPrefs.GetInt("Evolution Speed");
        if (evoSpeed < 6 || evoSpeed > 10)
        {
            evoSpeed = 8;
        }
        Time.timeScale = evoSpeed;
        Target.GetComponent<TargetController>().Activate();
        _ea.StartContinue();
        EARunning = true;
    }

    public void StopEA()
    {
        
        if (_ea != null && _ea.RunState == SharpNeat.Core.RunState.Running)
        {
            _ea.Stop();
        }
    }

    void ea_UpdateEvent(object sender, EventArgs e)
    {       
        Utility.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
            _ea.CurrentGeneration, _ea.Statistics._maxFitness));
        OptimizerGUI.CurrentGeneration = _ea.CurrentGeneration;
        OptimizerGUI.BestFitness = _ea.Statistics._maxFitness;

        _generation = _ea.CurrentGeneration;
        LastFitness = _ea.Statistics._maxFitness;

        Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));

        if (_generation == 1)
        {
            FirstFitness = LastFitness;
        }
    }

    void ea_PauseEvent(object sender, EventArgs e)
    {
        Target.GetComponent<TargetController>().Stop();
        Utility.Log("Done ea'ing (and neat'ing)");

        XmlWriterSettings _xwSettings = new XmlWriterSettings();
        _xwSettings.Indent = true;
        // Save genomes to xml file.        
        DirectoryInfo dirInf = new DirectoryInfo(Application.persistentDataPath + "/" + Parse.ParseUser.CurrentUser.Username);
        if (!dirInf.Exists)
        {
            Debug.Log("Creating subdirectory"); 
            dirInf.Create();
        }
        using (XmlWriter xw = XmlWriter.Create(popFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, _ea.GenomeList);
        }
        // Also save the best genome

        using (XmlWriter xw = XmlWriter.Create(champFileSavePath, _xwSettings))
        {
            experiment.SavePopulation(xw, new NeatGenome[] { _ea.CurrentChampGenome });
        }
        DateTime endTime = DateTime.Now;
        Utility.Log("Total time elapsed: " + (endTime - startTime));

        System.IO.StreamReader stream = new System.IO.StreamReader(popFileSavePath);
        ParseFile file = new ParseFile(string.Format("{0}.pop.xml", Settings.Brain.ObjectId), stream.BaseStream);
        Task sTask = file.SaveAsync();

        Settings.Brain.Population = file;

        stream = new System.IO.StreamReader(champFileSavePath);
        ParseFile pfile = new ParseFile(string.Format("{0}.champ.xml", Settings.Brain.ObjectId), stream.BaseStream);
        Task task = pfile.SaveAsync();

        Settings.Brain.Generation = (int)Generation;
        Settings.Brain.BestFitness = (float)LastFitness;

        Settings.Brain.ChampionGene = pfile;
        Settings.Brain.IsNewBrain = false;
        Settings.Brain.SaveAsync();
        EAStopped(this, EventArgs.Empty);
        EARunning = false;
       
    }

    public delegate void EAStoppedEventHandler(object sender, EventArgs e);

    public event EAStoppedEventHandler EAStopped;
	
	// Update is called once per frame
	void Update () {
	
	}
}
