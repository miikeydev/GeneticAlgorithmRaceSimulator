using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{
    public List<float[]> weightsStorage = new List<float[]>(); // Store the weights of each car
    public List<float> fitnessScores = new List<float>(); // Store the fitness scores
    public bool IsFirstGeneration { get; private set; } = true; // Indicator for the first generation
    public float mutationRate = 0.05f; // Mutation rate (5%)

    private int inputSize;
    private int hiddenLayerSize;
    private int outputSize;

    public void InitializeFirstGeneration(int populationSize, int inputSize, int hiddenLayerSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenLayerSize = hiddenLayerSize;
        this.outputSize = outputSize;

        weightsStorage.Clear();
        fitnessScores.Clear();

        for (int i = 0; i < populationSize; i++)
        {
            // Generate random weights for each individual
            float[] weights = GenerateRandomWeightsAsArray(inputSize, hiddenLayerSize, outputSize);
            weightsStorage.Add(weights);
            fitnessScores.Add(0f); // Initialize fitness scores to 0
        }

        IsFirstGeneration = false; // After initialization, it's no longer the first generation
    }

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

    // Method to generate a new population from the best individuals
    public void GenerateNewPopulation()
    {
        // Select the best individuals
        var topIndividuals = SelectTopIndividuals();

        // Create a new generation using crossover and mutation
        var newGeneration = CrossoverAndMutate(topIndividuals, weightsStorage.Count);

        weightsStorage.Clear();
        fitnessScores.Clear();

        foreach (var weights in newGeneration)
        {
            weightsStorage.Add(weights);
            fitnessScores.Add(0f); // Reset fitness scores
        }

        Debug.Log("New generation successfully created!");
    }

    // Save the weights and fitness scores of individuals
    public void SaveCarData(float[] weights, float fitnessScore, int index)
    {
        // Overwrite the weights and fitness of the current individual
        weightsStorage[index] = weights;
        fitnessScores[index] = fitnessScore;
    }

    // Select the top 10 individuals
    public List<(float[] weights, float score)> SelectTopIndividuals(int topCount = 10)
    {
        // Associate fitness scores with weights
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

        for (int i = 0; i < newPopulationSize; i++)
        {
            // Select parents
            float[] parent1 = SelectParent(topIndividuals, probabilities);
            float[] parent2 = SelectParent(topIndividuals, probabilities);

            // Perform single-point crossover
            float[] childWeights = Crossover(parent1, parent2);

            // Apply random mutation
            Mutate(childWeights);

            newGeneration.Add(childWeights);
        }

        return newGeneration;
    }

    // Weighted selection of a parent
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
        return individuals[0].weights; // Fallback value
    }

    // Single-point crossover
    private float[] Crossover(float[] parent1, float[] parent2)
    {
        int crossoverPoint = Random.Range(0, parent1.Length);
        float[] child = new float[parent1.Length];
        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = (i < crossoverPoint) ? parent1[i] : parent2[i];
        }
        return child;
    }

    // Apply mutation
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

    // Ajoute cette méthode pour obtenir le meilleur individu
    public float[] GetBestWeights()
    {
        // Associe les scores de fitness aux poids
        var combinedData = weightsStorage
            .Select((weights, index) => (weights: weights, score: fitnessScores[index]))
            .ToList();

        // Trie les individus par score de fitness décroissant
        combinedData.Sort((a, b) => b.score.CompareTo(a.score));

        // Retourne les poids du meilleur individu
        return combinedData[0].weights;
    }


}
