using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Collections.Generic;

public class ParallelStartEvaluation : MonoBehaviour
{

    public GameObject Robot;
    public GameObject Target;
    //CarMove carMove;
    static NeatEvolutionAlgorithm<NeatGenome> _ea;
    ParallelExperiment experiment;
    string filePath = @"Assets\Scripts\Populations\phototaxisChampParallel.gnm.xml";
    public float Duration = 10f;
    public float StoppingFitness = 1f;
    Dictionary<IBlackBox, CarMove> dict = new Dictionary<IBlackBox, CarMove>();
    DateTime startTime;

    // Use this for initialization
    void Start()
    {

        if (Robot != null)
        {

        }
        //    Reset();
        Debug.Log("Starting PhotoTaxis experiment");
        experiment = new ParallelExperiment();

        XmlDocument xmlConfig = new XmlDocument();
        xmlConfig.Load(@"Assets\Scripts\phototaxis.config.xml");
        experiment.SetStartEvaluation(this);
        experiment.Initialize("PhotoTaxis", xmlConfig.DocumentElement);

        _ea = experiment.CreateEvolutionAlgorithm();

        _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);
        _ea.PausedEvent += new EventHandler(ea_PauseEvent);




    }

    public void StartEA()
    {
        Time.timeScale = 100;
        if (_ea != null)
        {
            startTime = DateTime.Now;
            _ea.StartContinue();
        }
    }

    static void ea_UpdateEvent(object sender, EventArgs e)
    {
        Debug.Log(string.Format("gen={0:N0} bestFitness={1:N6}",
                                _ea.CurrentGeneration, _ea.Statistics._maxFitness));
    }

    void ea_PauseEvent(object sender, EventArgs e)
    {
        Debug.Log("Done ea'ing (and neat'ing)");

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
        Debug.Log("Total time elapsed: " + (endTime - startTime));
    }

    // Update is called once per frame
    void Update()
    {

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
            print("Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message);
            return;
        }

        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = experiment.CreateGenomeDecoder();

        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);

       // Reset();

        GameObject obj = Instantiate(Robot, new Vector3(20, 1, 0), Quaternion.identity) as GameObject;
        CarMove car = obj.GetComponent<CarMove>();        
        car.Activate(phenome, Target);
    }

    public void Evaluate(IBlackBox box)
    {
        //Reset();
        GameObject obj = Instantiate(Robot, new Vector3(20, 1, 0), Quaternion.identity) as GameObject;
        CarMove car = obj.GetComponent<CarMove>();
        dict.Add(box, car);
        car.Activate(box, Target);
    }

    public void StopEvaluation(IBlackBox box)
    {
        Debug.Log("Stop evaluation");
        CarMove car = dict[box];
        car.Stop();
        Destroy(car.gameObject);
    }

    public float GetFitness(IBlackBox box)
    {
        if (dict.ContainsKey(box))
        {
            float fit = dict[box].GetFitness();
            print("Fitness: " + fit);
            return fit;
        }
        return 0.0f;
    }

    //private void Reset()
    //{
    //    print("Resetting");
    //    //   Robot.transform.position = new Vector3(20, 1, 0);
    //    //  Robot.transform.rotation = Quaternion.identity;
    //    if (carMove != null)
    //    {
    //       // StopEvaluation();
    //    }
    //    GameObject obj = Instantiate(Robot, new Vector3(20, 1, 0), Quaternion.identity) as GameObject;
    //    carMove = obj.GetComponent<CarMove>();
    //}
}
