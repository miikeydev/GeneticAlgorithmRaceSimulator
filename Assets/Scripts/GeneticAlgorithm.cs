using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{
    public List<float[]> weightsStorage = new List<float[]>(); // Stores the weights of each car
    public List<float> fitnessScores = new List<float>(); // Stores the fitness scores
    public bool IsFirstGeneration { get; private set; } = true; // Indicates if it's the first generation
    public float mutationRate = 0.05f; // Mutation rate (5%)

    private int inputSize;
    private int hiddenLayerSize;
    private int outputSize;

    // Initialize the first generation of individuals
    public void InitializeFirstGeneration(int populationSize, int inputSize, int hiddenLayerSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenLayerSize = hiddenLayerSize;
        this.outputSize = outputSize;

        weightsStorage.Clear();
        fitnessScores.Clear();

        // Generate random weights for each individual
        for (int i = 0; i < populationSize; i++)
        {
            float[] weights = GenerateRandomWeightsAsArray(inputSize, hiddenLayerSize, outputSize);
            weightsStorage.Add(weights);
            fitnessScores.Add(0f); // Initialize fitness scores to 0
        }

        IsFirstGeneration = false; // After initialization, it's no longer the first generation
    }

    // Generate a random array of weights based on input, hidden, and output layer sizes
    private float[] GenerateRandomWeightsAsArray(int inputSize, int hiddenLayerSize, int outputSize)
    {
        int totalWeights = (inputSize * hiddenLayerSize) + hiddenLayerSize + (hiddenLayerSize * outputSize) + outputSize;
        float[] weights = new float[totalWeights];

        for (int i = 0; i < totalWeights; i++)
        {
            weights[i] = Random.Range(-1f, 1f);
        }

        return weights;
    }

    // Generate a new population from the best individuals
    public void GenerateNewPopulation()
    {
        // Select the best individuals
        var topIndividuals = SelectTopIndividuals();

        // Create a new generation using crossover and mutation
        var newGeneration = CrossoverAndMutate(topIndividuals, weightsStorage.Count);

        weightsStorage.Clear();
        fitnessScores.Clear();

        // Add new generation weights to storage and reset fitness scores
        foreach (var weights in newGeneration)
        {
            weightsStorage.Add(weights);
            fitnessScores.Add(0f);
        }

        Debug.Log("New generation successfully created!");
    }

    // Save the weights and fitness scores of individuals
    public void SaveCarData(float[] weights, float fitnessScore, int index)
    {
        weightsStorage[index] = weights; // Update the weights of the individual
        fitnessScores[index] = fitnessScore; // Update the fitness score
    }

    // Select the top-performing individuals
    public List<(float[] weights, float score)> SelectTopIndividuals(int topCount = 2)
    {
        // Pair fitness scores with weights
        var combinedData = weightsStorage
            .Select((weights, index) => (weights: weights, score: fitnessScores[index]))
            .ToList();

        // Sort by descending fitness score
        combinedData.Sort((a, b) => b.score.CompareTo(a.score));

        // Select the 'topCount' best individuals
        List<(float[] weights, float score)> topIndividuals = combinedData.Take(topCount).ToList();

        // Display the scores of the best individuals
        Debug.Log("Selection of top individuals completed. Top scores: " +
                  string.Join(", ", topIndividuals.Select(individual => individual.score)));

        return topIndividuals;
    }

    // Perform crossover and mutation to generate new individuals
    public List<float[]> CrossoverAndMutate(List<(float[] weights, float score)> topIndividuals, int newPopulationSize)
    {
        List<float[]> newGeneration = new List<float[]>();

        // Create a roulette wheel for weighted parent selection
        float totalFitness = topIndividuals.Sum(individual => individual.score);
        List<float> probabilities = topIndividuals.Select(individual => individual.score / totalFitness).ToList();

        // Generate individuals until new population size is met
        while (newGeneration.Count < newPopulationSize)
        {
            // Select parents using weighted probabilities
            float[] parent1 = SelectParent(topIndividuals, probabilities);
            float[] parent2 = SelectParent(topIndividuals, probabilities);

            // Alternate crossover to produce two children
            var (child1, child2) = Crossover(parent1, parent2);

            // Apply mutation to both children
            Mutate(child1);
            Mutate(child2);

            // Add children to the new generation
            newGeneration.Add(child1);
            if (newGeneration.Count < newPopulationSize)
                newGeneration.Add(child2); // Only add second child if space is available
        }

        return newGeneration;
    }

    // Weighted selection of a parent using probabilities
    private float[] SelectParent(List<(float[] weights, float score)> individuals, List<float> probabilities)
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0f;
        for (int i = 0; i < individuals.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return individuals[i].weights;
            }
        }
        return individuals[0].weights; // Fallback value if selection fails
    }

    // Alternating crossover to produce two children
    private (float[], float[]) Crossover(float[] parent1, float[] parent2)
    {
        float[] child1 = new float[parent1.Length];
        float[] child2 = new float[parent1.Length];

        for (int i = 0; i < parent1.Length; i++)
        {
            if (i % 2 == 0)
            {
                child1[i] = parent1[i]; // Child 1: even indices from parent1
                child2[i] = parent2[i]; // Child 2: even indices from parent2
            }
            else
            {
                child1[i] = parent2[i]; // Child 1: odd indices from parent2
                child2[i] = parent1[i]; // Child 2: odd indices from parent1
            }
        }
        return (child1, child2);
    }

    // Apply mutation to randomly alter weights
    private void Mutate(float[] weights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                weights[i] = Random.Range(-1f, 1f); // Mutate to a random value
            }
        }
    }

    // Get the weights for a given individual
    public float[] GetWeightsForIndividual(int index)
    {
        return weightsStorage[index];
    }

    // Retrieve the weights of the best-performing individual
    public float[] GetBestWeights()
    {
        // Pair fitness scores with weights
        var combinedData = weightsStorage
            .Select((weights, index) => (weights: weights, score: fitnessScores[index]))
            .ToList();

        // Sort by descending fitness score
        combinedData.Sort((a, b) => b.score.CompareTo(a.score));

        // Return the weights of the best individual
        return combinedData[0].weights;
    }
}
