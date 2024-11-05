using UnityEngine;

public class NeuralNetController : MonoBehaviour
{
    public TruckController truckController; // Référence au script TruckController
    public GameManager gameManager;

    // Configuration du réseau de neurones
    public int InputSize { get; private set; }
    public int HiddenLayerSize { get; private set; } = 64;
    public int OutputSize { get; private set; } = 8;

    // Modèle de réseau de neurones
    private float[,] weightsInputToHidden;
    private float[] biasesHidden;
    private float[,] weightsHiddenToOutput;
    private float[] biasesOutput;
    private float[] hiddenLayer;
    private float[] outputLayer;

    // Fonction de fitness
    private float fitnessScore = 0f;

    void Start()
    {
        float[] raycastData = truckController.GetRaycastData();
        InputSize = raycastData.Length + 1; // Nombre de raycasts + vitesse

        hiddenLayer = new float[HiddenLayerSize];
        outputLayer = new float[OutputSize];
    }

    // Méthode pour initialiser les poids depuis le GeneticAlgorithm
    public void InitializeWeights(float[] allWeights)
    {
        // Reconstruct the weights and biases from the flat array
        int index = 0;


        // Initialize weightsInputToHidden
        weightsInputToHidden = new float[InputSize, HiddenLayerSize];
        for (int i = 0; i < InputSize; i++)
        {
            for (int j = 0; j < HiddenLayerSize; j++)
            {
                weightsInputToHidden[i, j] = allWeights[index++];
            }
        }

        // Initialize biasesHidden
        biasesHidden = new float[HiddenLayerSize];
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            biasesHidden[i] = allWeights[index++];
        }

        // Initialize weightsHiddenToOutput
        weightsHiddenToOutput = new float[HiddenLayerSize, OutputSize];
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            for (int j = 0; j < OutputSize; j++)
            {
                weightsHiddenToOutput[i, j] = allWeights[index++];
            }
        }

        // Initialize biasesOutput
        biasesOutput = new float[OutputSize];
        for (int i = 0; i < OutputSize; i++)
        {
            biasesOutput[i] = allWeights[index++];
        }
    }


    // Méthodes pour gérer la fonction de fitness
    public void IncrementFitness()
    {
        fitnessScore += 4f; 
    }

    public void DecrementFitnessOnBorder()
    {
        fitnessScore -= 1f; // -1 point pour toucher le tag "Border"
    }

    public void ApplyEndEpisodePenalty()
    {
        fitnessScore -= 5f;
    }

    public void ResetFitness()
    {
        fitnessScore = 0f;
    }

    public float GetFitnessScore() // Changer en float
    {
        return fitnessScore;
    }

    // Méthode pour obtenir les poids sous forme de tableau à une dimension
    public float[] GetWeights()
    {
        // Convertir les poids en un tableau à une dimension pour le stockage
        int totalSize = (InputSize * HiddenLayerSize) + HiddenLayerSize + (HiddenLayerSize * OutputSize) + OutputSize;
        float[] allWeights = new float[totalSize];
        int index = 0;

        // Convertir weightsInputToHidden
        for (int i = 0; i < InputSize; i++)
        {
            for (int j = 0; j < HiddenLayerSize; j++)
            {
                allWeights[index++] = weightsInputToHidden[i, j];
            }
        }

        // Convertir biasesHidden
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            allWeights[index++] = biasesHidden[i];
        }

        // Convertir weightsHiddenToOutput
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            for (int j = 0; j < OutputSize; j++)
            {
                allWeights[index++] = weightsHiddenToOutput[i, j];
            }
        }

        // Convertir biasesOutput
        for (int i = 0; i < OutputSize; i++)
        {
            allWeights[index++] = biasesOutput[i];
        }

        return allWeights;
    }

    float[] CollectInputs()
    {
        float[] raycastData = truckController.GetRaycastData();
        float[] inputs = new float[InputSize];

        for (int i = 0; i < raycastData.Length; i++)
        {
            inputs[i] = raycastData[i];
        }

        inputs[raycastData.Length] = truckController.GetSpeed();
        return inputs;
    }

    float[] MatrixMultiply(float[] inputs, float[,] weights, float[] biases)
    {
        int rows = weights.GetLength(1);
        float[] output = new float[rows];

        for (int i = 0; i < rows; i++)
        {
            output[i] = biases[i]; // Commence par le biais
            for (int j = 0; j < inputs.Length; j++)
            {
                output[i] += inputs[j] * weights[j, i];
            }
        }
        return output;
    }

    void ApplyActivationFunction(float[] layer)
    {
        for (int i = 0; i < layer.Length; i++)
        {
            layer[i] = Mathf.Max(0, layer[i]); // ReLU pour la non-linéarité
        }
    }

    int SelectAction()
    {
        int actionIndex = 0;
        float maxOutput = outputLayer[0];
        for (int i = 1; i < outputLayer.Length; i++)
        {
            if (outputLayer[i] > maxOutput)
            {
                maxOutput = outputLayer[i];
                actionIndex = i;
            }
        }
        return actionIndex;
    }

    public int GetActionIndex()
    {
        float[] inputs = CollectInputs();

        // Propagation avant (Forward Propagation)
        hiddenLayer = MatrixMultiply(inputs, weightsInputToHidden, biasesHidden);
        ApplyActivationFunction(hiddenLayer);

        outputLayer = MatrixMultiply(hiddenLayer, weightsHiddenToOutput, biasesOutput);

        return SelectAction();
    }
}
