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
    public List<(float[] weights, float score)> SelectTopIndividuals(int topCount = 2)
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

        // Crée une roulette pour la sélection pondérée des parents
        float totalFitness = topIndividuals.Sum(individual => individual.score);
        List<float> probabilities = topIndividuals.Select(individual => individual.score / totalFitness).ToList();

        while (newGeneration.Count < newPopulationSize)
        {
            // Sélection des parents
            float[] parent1 = SelectParent(topIndividuals, probabilities);
            float[] parent2 = SelectParent(topIndividuals, probabilities);

            // Croisement alterné pour obtenir deux enfants
            var (child1, child2) = Crossover(parent1, parent2);

            // Mutation des enfants
            Mutate(child1);
            Mutate(child2);

            // Ajoute les enfants à la nouvelle génération
            newGeneration.Add(child1);
            if (newGeneration.Count < newPopulationSize)
                newGeneration.Add(child2); // Ajoute seulement si la taille n'est pas encore atteinte
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
    // Croisement alterné pour produire deux enfants
    private (float[], float[]) Crossover(float[] parent1, float[] parent2)
    {
        float[] child1 = new float[parent1.Length];
        float[] child2 = new float[parent1.Length];

        for (int i = 0; i < parent1.Length; i++)
        {
            if (i % 2 == 0)
            {
                child1[i] = parent1[i]; // Enfant 1 : indices pairs de parent1
                child2[i] = parent2[i]; // Enfant 2 : indices pairs de parent2
            }
            else
            {
                child1[i] = parent2[i]; // Enfant 1 : indices impairs de parent2
                child2[i] = parent1[i]; // Enfant 2 : indices impairs de parent1
            }
        }
        return (child1, child2);
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