using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GeneticAlgorithm : MonoBehaviour
{
    public List<float[]> weightsStorage = new List<float[]>(); // Stocker les poids de chaque voiture
    public List<float> fitnessScores = new List<float>(); // Stocker les scores de fitness

    public void SaveCarData(float[] weights, float fitnessScore)
    {
        weightsStorage.Add(weights);
        fitnessScores.Add(fitnessScore);
    }

    public void ClearData()
    {
        weightsStorage.Clear();
        fitnessScores.Clear();
    }

    // M�thode pour g�n�rer de nouveaux poids al�atoires
    public float[,] GenerateRandomWeights(int inputSize, int outputSize)
    {
        float[,] weights = new float[inputSize, outputSize];
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weights[i, j] = Random.Range(-1f, 1f); // Poids initialis�s al�atoirement
            }
        }
        return weights;
    }

    public float[] GenerateRandomBiases(int size)
    {
        float[] biases = new float[size];
        for (int i = 0; i < size; i++)
        {
            biases[i] = Random.Range(-1f, 1f); // Biais initialis�s al�atoirement
        }
        return biases;
    }

    // M�thode pour s�lectionner les 10 meilleurs individus
    public List<float[]> SelectTopIndividuals(int topCount = 10)
    {
        // Associer les scores de fitness aux poids
        List<(float[] weights, float score)> combinedData = weightsStorage
            .Select((weights, index) => (weights, fitnessScores[index]))
            .ToList();

        // Trier les individus par score de fitness d�croissant
        combinedData.Sort((a, b) => b.score.CompareTo(a.score));

        // S�lectionner les 'topCount' meilleurs individus
        List<float[]> topIndividuals = combinedData
            .Take(topCount)
            .Select(individual => individual.weights)
            .ToList();

        // Afficher un log avec les scores des meilleurs individus
        Debug.Log("S�lection des meilleurs individus termin�e. Scores des meilleurs individus : " +
                  string.Join(", ", combinedData.Take(topCount).Select(individual => individual.score)));

        return topIndividuals;
    }
}
