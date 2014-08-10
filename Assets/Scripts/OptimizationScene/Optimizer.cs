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
    public GameObject HammerRobot;
    List<FightController> BestRunners = new List<FightController>();
    float evaluationStartTime;
    float maxFitness = 0;
    FitnessDetails bestFitness;
    public dfRichTextLabel DetailsLabel;

    float timeLeft, accum, updateInterval = 12;
    int frames;

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

    public float ProgressIteration
    {
        get
        {
            var pg = evaluationStartTime / TrialDuration;  
            var val = OptimizerGUI.CurrentIteration - 1 + pg;            
            return val > 0 ? val : 0;
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

	Dictionary<IBlackBox, TrainingController> dict = new Dictionary<IBlackBox, TrainingController>();
    Dictionary<TrainingController, TargetController> targetDict = new Dictionary<TrainingController, TargetController>();
    Dictionary<FightController, TargetController> bestTargetDict = new Dictionary<FightController, TargetController>();

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
	//	OptimizerParameters.MultipleTargets = false;

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
		TrainingController robo = obj.GetComponent<TrainingController>();
		Target.transform.position = new Vector3(0, 1, 0);
        Target.transform.localScale = new Vector3(Settings.Brain.TargetSize, 2, Settings.Brain.TargetSize);
		dict.Add(box, robo);

		if (Settings.Brain.MultipleTargets)
		{
			GameObject t = Instantiate(Target, new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            t.transform.localScale = new Vector3(1, 1, 1);
			TargetController tc = t.AddComponent<TargetController>();
			tc.Activate(obj.transform);
			targetDict.Add(robo, tc);
			robo.Activate(box, t);
		}
		else
		{
			robo.Activate(box, Target);
		}
        evaluationStartTime = 0;
	}

	public void RunBest()
	{
		Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
		NeatGenome genome = null;
		

		// Try to load the genome from the XML document.
		try
		{
			using (XmlReader xr = XmlReader.Create(champFileLoadPath))
				genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];

			
		}
		catch (Exception e1)
		{
			// print(champFileLoadPath + " Error loading genome from file!\nLoading aborted.\n"
			//						  + e1.Message + "\nJoe: " + champFileLoadPath);
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
		GameObject obj = Instantiate(HammerRobot, pos, Quaternion.identity) as GameObject;
        FightController robo = obj.GetComponent<FightController>();
	 //   robo.RunBestOnly = true;

		if (Settings.Brain.MultipleTargets)
		{
			GameObject t = Instantiate(Target, new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            t.transform.localScale = new Vector3(1, 1, 1);
			TargetController tc = t.AddComponent<TargetController>();
			tc.Activate(obj.transform);
			bestTargetDict.Add(robo, tc);
			robo.Activate(phenome, t);
		}
		else
		{

			Target.GetComponent<TargetController>().Activate();
			robo.Activate(phenome, Target);
		}
        BestRunners.Add(robo);
	}

    void CleanUpScene()
    {
        foreach (FightController robo in BestRunners)
        {
            if (Settings.Brain.MultipleTargets)
            {
                bestTargetDict[robo].Stop();
                Destroy(bestTargetDict[robo].gameObject);
            }
            robo.Stop();
            Destroy(robo.gameObject);

        }
        BestRunners = new List<FightController>();
    }

	public void StopEvaluation(IBlackBox box)
	{
		TrainingController robo = dict[box];
		robo.Stop();
		if (targetDict.ContainsKey(robo))
		{
			targetDict[robo].Stop();
            Destroy(targetDict[robo].gameObject);
            targetDict.Remove(robo);
		}
		Destroy(robo.gameObject);
	}

	public float GetFitness(IBlackBox box)
	{
		if (dict.ContainsKey(box))
		{
            FitnessDetails details = dict[box].GetFitness();
            float fit = details.Fitness;
			// print("Fitness: " + fit);
            if (fit > maxFitness)
            {
                maxFitness = fit;
                bestFitness = details;
            }
			return fit;
		}
		return 0.0f;
	}

	public void StartEA()
	{
        CleanUpScene();
		Utility.DebugLog = false;
		Utility.Log("Starting PhotoTaxis experiment");
		// print("Loading: " + popFileLoadPath);
		_ea = experiment.CreateEvolutionAlgorithm(popFileLoadPath);

		_ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
		_ea.PausedEvent += new EventHandler(ea_PauseEvent);
		
		startTime = DateTime.Now;
		int evoSpeed = PlayerPrefs.GetInt("Evolution Speed");
		if (evoSpeed < 6 || evoSpeed > 10)
		{
			evoSpeed = 8;
		}
        evoSpeed = 25;
       
        Time.fixedDeltaTime = 0.045f;
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

        WriteDetailsLabel();

		_generation = _ea.CurrentGeneration;
		LastFitness = _ea.Statistics._maxFitness;

		Utility.Log(string.Format("Moving average: {0}, N: {1}", _ea.Statistics._bestFitnessMA.Mean, _ea.Statistics._bestFitnessMA.Length));

		if (_generation == 1)
		{
			FirstFitness = LastFitness;
		}
        maxFitness = 0;
	}

    private void WriteDetailsLabel()
    {
        string details = "";

        if (Settings.Brain.MoveAround != 0)
        {
            string color = Settings.Brain.MoveAround < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Distance moved: {0:0.00}</span><br>", bestFitness.DistanceMoved, color);
        }

        if (Settings.Brain.ReachDistance != 0)
        {
            string color = Settings.Brain.ReachDistance < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Distance reached: {0:0.00}</span><br>", bestFitness.ReachDistance, color);
            
        }

        if (Settings.Brain.KeepDistance != 0)
        {
            string color = Settings.Brain.KeepDistance < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Keep distance: {0:0.00}</span><br>", bestFitness.KeepDistance, color);
            
        }

        if (Settings.Brain.FaceTarget != 0)
        {
            string color = Settings.Brain.FaceTarget < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Face target: {0:0.00}</span><br>", bestFitness.FaceTarget, color);           
        }

        if (Settings.Brain.MeleeAttacks != 0)
        {
            string color = Settings.Brain.MeleeAttacks < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Melee attacks: {0}</span><br>", bestFitness.MeleeAttacks, color);
           
        }

        if (Settings.Brain.MeleeHits != 0)
        {
            string color = Settings.Brain.MeleeHits < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Melee hits: {0}</span><br>", bestFitness.MeleeHits, color);
            
        }

        if (Settings.Brain.MeleePrecision != 0)
        {
            string color = Settings.Brain.MeleePrecision < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Melee precision: {0:0.00}%</span><br>", bestFitness.MeleePrecision, color);
            
        }

        if (Settings.Brain.RifleAttacks != 0)
        {
            string color = Settings.Brain.RifleAttacks < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Ranged attacks: {0}</span><br>", bestFitness.RangedAttacks, color);
            
        }

        if (Settings.Brain.RifleHits != 0)
        {
            string color = Settings.Brain.RifleHits < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Ranged hits: {0}</span><br>", bestFitness.RangedHits, color);
            
        }

        if (Settings.Brain.RiflePrecision != 0)
        {
            string color = Settings.Brain.RiflePrecision < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Ranged precision: {0:0.00}%</span><br>", bestFitness.RangedPrecision, color);
           
        }

        if (Settings.Brain.TurretFaceTarget != 0)
        {
            string color = Settings.Brain.TurretFaceTarget < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Aim turret: {0:0.00}</span><br>", bestFitness.AimTurret, color);
            
        }

        if (Settings.Brain.MortarAttacks != 0)
        {
            string color = Settings.Brain.MortarAttacks < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Mortar attacks: {0}</span><br>", bestFitness.MortarAttacks, color);
            
        }

        if (Settings.Brain.MortarHits != 0)
        {
            string color = Settings.Brain.MortarHits < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Mortar hits: {0}</span><br>", bestFitness.MortarHits, color);
          
        }

        if (Settings.Brain.MortarPrecision != 0)
        {
            string color = Settings.Brain.MortarPrecision < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Mortar precision: {0:0.00}%</span><br>", bestFitness.MortarPrecision, color);
           
        }

        if (Settings.Brain.MortarDamagePerHit != 0)
        {
            string color = Settings.Brain.MortarDamagePerHit < 0 ? "red" : "green";
            details += string.Format("<span style=\"color: {1}\">Mortar dmg/hit: {0:0.00}</span><br>", bestFitness.MortarDamagePerHit, color);
            
        }

        

        DetailsLabel.Text = details;
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
        evaluationStartTime += Time.deltaTime;

        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeLeft <= 0.0)
        {
            var fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
         //   print("FPS: " + fps);
            if (fps < 10)
            {
                Time.timeScale = Time.timeScale - 1;
                // print("Lowering time scale to " + Time.timeScale);
            }
        }
	}
}
