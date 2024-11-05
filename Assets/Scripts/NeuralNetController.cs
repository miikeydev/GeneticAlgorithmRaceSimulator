using UnityEngine;

public class NeuralNetController : MonoBehaviour
{
    public TruckController truckController; // Référence au script TruckController
    public GameManager gameManager;

    // Configuration du réseau de neurones
    private int inputSize;
    private int hiddenLayerSize = 64;
    private int outputSize = 8;

    // Modèle de réseau de neurones
    private float[,] weightsInputToHidden;
    private float[] biasesHidden;
    private float[,] weightsHiddenToOutput;
    private float[] biasesOutput;
    private float[] hiddenLayer;
    private float[] outputLayer;

    void Start()
    {
        float[] raycastData = truckController.GetRaycastData();
        inputSize = raycastData.Length + 1; // Nombre de raycasts + vitesse

        hiddenLayer = new float[hiddenLayerSize];
        outputLayer = new float[outputSize];

        // Initialiser les poids et biais aléatoires
        weightsInputToHidden = InitializeWeights(inputSize, hiddenLayerSize);
        biasesHidden = InitializeBiases(hiddenLayerSize);
        weightsHiddenToOutput = InitializeWeights(hiddenLayerSize, outputSize);
        biasesOutput = InitializeBiases(outputSize);
    }



    float[] CollectInputs()
    {
        float[] raycastData = truckController.GetRaycastData();
        float[] inputs = new float[inputSize];

        // Assurez-vous que la longueur de raycastData est correcte avant l'assignation
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
        // Sélectionner l'action avec la valeur de sortie la plus élevée
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

    float[,] InitializeWeights(int inputSize, int outputSize)
    {
        float[,] weights = new float[inputSize, outputSize];
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weights[i, j] = Random.Range(-1f, 1f); // Poids initialisés aléatoirement
            }
        }
        return weights;
    }

    float[] InitializeBiases(int size)
    {
        float[] biases = new float[size];
        for (int i = 0; i < size; i++)
        {
            biases[i] = Random.Range(-1f, 1f); // Biais initialisés aléatoirement
        }
        return biases;
    }

    public int GetActionIndex()
    {
        // Récupérer les entrées
        float[] inputs = CollectInputs();

        // Propagation avant (Forward Propagation)
        hiddenLayer = MatrixMultiply(inputs, weightsInputToHidden, biasesHidden);
        ApplyActivationFunction(hiddenLayer); // Utilise ReLU pour les couches cachées

        outputLayer = MatrixMultiply(hiddenLayer, weightsHiddenToOutput, biasesOutput);

        // Choisir et retourner l'action
        int actionIndex = SelectAction();
        return actionIndex;
    }

}
