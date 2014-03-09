using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NeuralNetwork : BaseBehaviour {

	// Use this for initialization
	void Start () {
        Neuron in1 = new Neuron();
        in1.Type = NeuronType.Sensor;
        Neuron in2 = new Neuron();
        in2.Type = NeuronType.Sensor;
        Neuron hidden1 = new Neuron();
        hidden1.Type = NeuronType.Neuron;
        Neuron hidden2 = new Neuron();
        hidden2.Type = NeuronType.Neuron;
        Neuron out1 = new Neuron();
        out1.Type = NeuronType.Neuron;

        CreateSynapse(in1, hidden1, 1, false);
        CreateSynapse(in1, hidden2, 1, false);
        CreateSynapse(in2, hidden1, 2, false);
        CreateSynapse(in2, hidden2, 2, false);
        CreateSynapse(hidden1, out1, 0.4f, false);
        CreateSynapse(hidden2, out1, 0.5f, false);

        List<Neuron> inputNeurons = new List<Neuron>() { in1, in2 };
        List<float> inputValues = new List<float>() { 600.7f, -100.1f };

        List<float> outputValues = Fire(inputNeurons, inputValues);
        foreach (float f in outputValues)
        {
            print("Output: " + f);
        }
	}

    public List<float> Fire(List<Neuron> inputNeurons, List<float> inputValues)
    {
        for (int i = 0; i < inputNeurons.Count; i++)
        {
            inputNeurons[i].Input = inputValues[i];
        }
        List<Neuron> currentNeurons = inputNeurons;
        List<float> retval = new List<float>();
        
        while (currentNeurons != null && currentNeurons.Count > 0)
        {
            List<Neuron> nextNeurons = new List<Neuron>();
            retval = new List<float>();

            foreach (Neuron n in currentNeurons)
            {
                n.ComputeOutput();
                retval.Add(n.Output); 
                foreach (Synapse s in n.OutgoingSynapses)
                {
                    Neuron c = s.Out;
                    if (!nextNeurons.Contains(c))
                    {
                        nextNeurons.Add(c);
                    }
                }
            }
            currentNeurons = nextNeurons;
        }
        return retval;
    }

    private void CreateSynapse(Neuron n1, Neuron n2, float weight, bool recurrent)
    {
        Synapse s = new Synapse(n1, n2, weight, recurrent);
        n1.OutgoingSynapses.Add(s);
        n2.IncomingSynapses.Add(s);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class Neuron
{
    public List<Synapse> IncomingSynapses { get; set; }
    public List<Synapse> OutgoingSynapses { get; set; }
    public NeuronType Type { get; set; }

    public float Output { get; set; }
    public float Input { get; set; }

    public Neuron()
    {
        IncomingSynapses = new List<Synapse>();
        OutgoingSynapses = new List<Synapse>();
    }

    private float sigmoid(double d)
    {
        return (float) (1.0 / (1.0 + Math.Pow(Math.E, -1.0 * d)));
    }

    public float ComputeOutput()
    {
        if (this.Type == NeuronType.Sensor)
        {
            this.Output = Input;
        }
        else
        {
            float input = computeInput();
            this.Input = input;
            this.Output = sigmoid(input);
        }
        return this.Output;
    }

    private float computeInput()
    {
        float input = 0;
        foreach (Synapse s in IncomingSynapses)
        {
            input += s.Weight * s.In.Output;
        }
        return input;
    }
}

public class Synapse
{
    public Neuron In { get; set; }
    public Neuron Out { get; set; }
    public float Weight { get; set; }
    public bool Recurrent { get; set; }

    public Synapse(Neuron i, Neuron o, float w, bool r)
    {
        this.In = i;
        this.Out = o;
        this.Weight = w;
        this.Recurrent = r;
    }
}

public enum NeuronType { Neuron, Sensor }
