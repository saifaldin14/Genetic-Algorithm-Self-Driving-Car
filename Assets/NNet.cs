using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;

public class NNet : MonoBehaviour {
    public Matrix<float> input = Matrix<float>.Build.Dense(1, 3);

    public List<Matrix<float>> hidden = new List<Matrix<float>>();

    public Matrix<float> output = Matrix<float>.Build.Dense(1, 2);

    public List<Matrix<float>> weights = new List<Matrix<float>>();

    public List<float> biases = new List<float>();

    public float fitness;

    public void Init (int hiddenLayerCount, int hiddenNeuronCount) {
        // Initialize the hidden layers
        // The hidden layers are a list of matrices
        // Each matrix is a layer of neurons
        // Each layer of neurons is a matrix of 1 row and the number of neurons in the layer
        input.Clear();
        hidden.Clear();
        output.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++) {

            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);

            hidden.Add(f);

            biases.Add(Random.Range(-1f, 1f));

            //WEIGHTS
            if (i == 0) {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(3, hiddenNeuronCount);
                weights.Add(inputToH1);
            }

            Matrix<float> HiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(HiddenToHidden);
        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f, 1f));

        RandomizeWeights();
    }

    public NNet CreateCopy (int hiddenLayerCount, int hiddenNeuronCount) {
        // Initialize the hidden layers
        // The hidden layers are a list of matrices
        // Each matrix is a layer of neurons
        // Each layer of neurons is a matrix of 1 row and the number of neurons in the layer
        NNet n = new NNet();

        List<Matrix<float>> newWeights = new List<Matrix<float>>();

        for (int i = 0; i < this.weights.Count; i++) {
            Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

            for (int x = 0; x < currentWeight.RowCount; x++) {
                for (int y = 0; y < currentWeight.ColumnCount; y++) {
                    currentWeight[x, y] = weights[i][x, y];
                }
            }

            newWeights.Add(currentWeight);
        }

        List<float> newBiases = new List<float>();

        newBiases.AddRange(biases);

        n.weights = newWeights;
        n.biases = newBiases;

        n.CreateHidden(hiddenLayerCount, hiddenNeuronCount);

        return n;
    }

    public void CreateHidden (int hiddenLayerCount, int hiddenNeuronCount) {
        // Initialize the hidden layers
        // The hidden layers are a list of matrices
        // Each matrix is a layer of neurons
        // Each layer of neurons is a matrix of 1 row and the number of neurons in the layer
        input.Clear();
        hidden.Clear();
        output.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i ++) {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hidden.Add(newHiddenLayer);
        }

    }

    public void RandomizeWeights() {
        // Randomise the weights
        // The weights are a list of matrices
        // Each matrix is a layer of weights
        // Each layer of weights is a matrix of the number of neurons in the previous layer and the number of neurons in the next layer
        for (int i = 0; i < weights.Count; i++) {
            for (int x = 0; x < weights[i].RowCount; x++) {
                for (int y = 0; y < weights[i].ColumnCount; y++) {
                    weights[i][x, y] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public (float, float) RunNetwork (float a, float b, float c) {
        // Run the network
        // The input layer is a matrix of 1 row and 3 columns
        // The first column is the x position of the player
        // The second column is the y position of the player
        // The third column is the z position of the target
        // The output layer is a matrix of 1 row and 2 columns
        input[0, 0] = a;
        input[0, 1] = b;
        input[0, 2] = c;

        input = input.PointwiseTanh();

        hidden[0] = ((input * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hidden.Count; i++) {
            hidden[i] = ((hidden[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        output = ((hidden[hiddenLayers.Count-1]*weights[weights.Count-1])+biases[biases.Count-1]).PointwiseTanh();

        //First output is acceleration and second output is steering
        return (SigmoidActivationFunction(output[0,0]), (float)Math.Tanh(output[0,1]));
    }

    private float SigmoidActivationFunction (float s) {
        // Sigmoid function
        // Used to normalise the output of the network
        // The output of the network is a number between -1 and 1
        // The sigmoid function maps the output to a number between 0 and 1
        // This is used to control the acceleration of the player
        return (1 / (1 + Mathf.Exp(-s)));
    }

}
