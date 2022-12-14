using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;


public class GeneticManager : MonoBehaviour {

    // Used to count the number of mutations
    public static int mutationCounter = 0;


    public CarController controller;

    public int initPop = 85; // Initial Population

    public float mutation = 0.055f; // Mutation Rate

    public int bestAgent = 8;
    public int worstAgent = 3;
    public int crossoverNum;

    private List<int> genePool = new List<int>();

    private int natSelect; // Naturally selected values

    private NNet[] population;

    public int currGen; // Current Generation
    public int currGenome = 0;

    private void Start() {
        // Create the initial population
        CreatePopulation();
    }

    private void CreatePopulation() {
        // Create the population array
        // The population array is the same size as the initial population
        // The population array is an array of neural networks

        population = new NNet[initPop];
        FillPopulationWithInitialValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome() {
        // Reset the car to the current genome
        controller.ResetWithNetwork(population[currGenome]);
    }

    private void FillPopulationWithInitialValues (NNet[] newPopulation, int startingIndex) {
        // Fill the population with random values
        // The starting index is used to fill the population with new values

        while (startingIndex < initPop) {
            newPopulation[startingIndex] = new NNet();
            newPopulation[startingIndex].Initialize(controller.LAYERS, controller.NEURONS);
            startingIndex++;
        }
    }

    public void Death (float fitness, NNet network) {
        // The car has died
        // The fitness is the overall fitness of the car

        if (currGenome < population.Length -1) {
            population[currGenome].fitness = fitness;
            currGenome++;
            ResetToCurrentGenome();
        } else {
            RePopulate();
        }

    }

    private void RePopulate() {
        // Repopulate the population
        // The population is repopulated by selecting the best genomes and breeding them
        // The best genomes are selected by their fitness
        // The worst genomes are selected by their fitness
        genePool.Clear();
        currGen++;
        natSelect = 0;
        SortPopulation();

        NNet[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithInitialValues(newPopulation, natSelect);

        population = newPopulation;

        currGenome = 0;

        ResetToCurrentGenome();

    }

    private void Mutate (NNet[] newPopulation) {
        // Mutate the population
        //  The mutation rate is the chance that a gene will be mutated
        //  The mutation rate is a value between 0 and 1
        for (int i = 0; i < natSelect; i++) {
            for (int c = 0; c < newPopulation[i].weights.Count; c++) {
                if (Random.Range(0.0f, 1.0f) < mutation) {
                    mutationCounter += 1;
                    newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                }
            }
        }
    }

    Matrix<float> MutateMatrix (Matrix<float> A) {
        // Mutate a matrix
        int mutationConstant = 7;
        int randomPoints = Random.Range(1, (A.RowCount * A.ColumnCount) / mutationConstant);

        Matrix<float> C = A;

        for (int i = 0; i < randomPoints; i++) {
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return C;
    }


    //used to update the mutationCounterUI
    public static int getMutationCounter(){
        return mutationCounter;
    }

    private void Crossover (NNet[] newPopulation) {
        // Crossover the population
        // The crossover rate is a value between 0 and 1
        // The number of genomes to crossover is a value between 0 and the initial population
        // The number of genomes to crossover is the number of genomes that will be bred

        for (int i = 0; i < crossoverNum; i+=2) {
            int AIndex = i;
            int BIndex = i + 1;

            if (genePool.Count >= 1) {
                for (int l = 0; l < 100; l++) {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                        break;
                }
            }

            NNet Child1 = new NNet();
            NNet Child2 = new NNet();

            Child1.Init(controller.LAYERS, controller.NEURONS);
            Child2.Init(controller.LAYERS, controller.NEURONS);

            Child1.fitness = 0;
            Child2.fitness = 0;


            for (int w = 0; w < Child1.weights.Count; w++) {
                if (Random.Range(0.0f, 1.0f) < 0.5f) {
                    Child1.weights[w] = population[AIndex].weights[w];
                    Child2.weights[w] = population[BIndex].weights[w];
                } else {
                    Child2.weights[w] = population[AIndex].weights[w];
                    Child1.weights[w] = population[BIndex].weights[w];
                }
            }

            for (int w = 0; w < Child1.biases.Count; w++) {
                if (Random.Range(0.0f, 1.0f) < 0.5f) {
                    Child1.biases[w] = population[AIndex].biases[w];
                    Child2.biases[w] = population[BIndex].biases[w];
                } else {
                    Child2.biases[w] = population[AIndex].biases[w];
                    Child1.biases[w] = population[BIndex].biases[w];
                }

            }

            newPopulation[natSelect] = Child1;
            natSelect++;

            newPopulation[natSelect] = Child2;
            natSelect++;
        }
    }

    private NNet[] PickBestPopulation() {
        // Pick the best population
        // The best population is the best genomes from the previous population
        // The best population is the same size as the initial population
        // The best population is an array of neural networks
        // The number of genomes to naturally select is the number of genomes that will be selected
        // The number of genomes to naturally select is a value between 0 and the initial population
        NNet[] newPopulation = new NNet[initPop];

        for (int i = 0; i < bestAgent; i++) {
            newPopulation[natSelect] = population[i].CreateCopy(controller.LAYERS, controller.NEURONS);
            newPopulation[natSelect].fitness = 0;
            natSelect++;

            int f = Mathf.RoundToInt(population[i].fitness * 10);

            for (int c = 0; c < f; c++) {
                genePool.Add(i);
            }
        }

        for (int i = 0; i < worstAgent; i++) {
            int last = population.Length - 1;
            last -= i;

            int f = Mathf.RoundToInt(population[last].fitness * 10);

            for (int c = 0; c < f; c++) {
                genePool.Add(last);
            }

        }

        return newPopulation;
    }

    private void SortPopulation() {
        // Sort the population
        // The population is sorted by their fitness
        // The population is sorted from best to worst
        // The population is sorted in descending order
        // The population is sorted in an array of neural networks

        for (int i = 0; i < population.Length; i++) {
            for (int j = i; j < population.Length; j++) {
                if (population[i].fitness < population[j].fitness) {
                    NNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }
    }
}
