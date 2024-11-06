using UnityEngine;

public class NeuralNetController : MonoBehaviour
{
    public TruckController truckController; // Reference to the TruckController script
    public GameManager gameManager;

    // Neural network configuration
    public int InputSize { get; private set; }
    public int HiddenLayerSize { get; private set; } = 64;
    public int OutputSize { get; private set; } = 3;

    // Neural network model
    private float[,] weightsInputToHidden; // Weights between input and hidden layer
    private float[] biasesHidden; // Biases for hidden layer
    private float[,] weightsHiddenToOutput; // Weights between hidden and output layer
    private float[] biasesOutput; // Biases for output layer
    private float[] hiddenLayer; // Activations of hidden layer
    private float[] outputLayer; // Activations of output layer

    // Fitness function score
    private float fitnessScore = 0f;

    void Start()
    {
        float[] raycastData = truckController.GetRaycastData();
        InputSize = raycastData.Length + 1; // Number of raycasts + speed

        hiddenLayer = new float[HiddenLayerSize];
        outputLayer = new float[OutputSize];
    }

    // Method to initialize weights from the GeneticAlgorithm
    public void InitializeWeights(float[] allWeights)
    {
        // Reconstruct weights and biases from a flat array
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

    // Methods to manage fitness scoring
    public void IncrementFitness()
    {
        fitnessScore += 4f;
    }

    public void DecrementFitnessOnBorder()
    {
        fitnessScore -= 1f; // -1 point for touching the "Border" tag
    }

    public void ApplyEndEpisodePenalty()
    {
        fitnessScore -= 5f;
    }

    public void ApplyBorderPenalty()
    {
        fitnessScore -= 5f; // -5 points for a collision with the "Border"
        //Debug.Log("Penalty of -5 applied for Border collision. Current fitness score: " + fitnessScore);
    }

    public void ResetFitness()
    {
        fitnessScore = 0f;
    }

    public float GetFitnessScore() // Return fitness score
    {
        return fitnessScore;
    }

    // Method to obtain weights as a 1D array for storage
    public float[] GetWeights()
    {
        // Convert weights to a 1D array for storage
        int totalSize = (InputSize * HiddenLayerSize) + HiddenLayerSize + (HiddenLayerSize * OutputSize) + OutputSize;
        float[] allWeights = new float[totalSize];
        int index = 0;

        // Convert weightsInputToHidden
        for (int i = 0; i < InputSize; i++)
        {
            for (int j = 0; j < HiddenLayerSize; j++)
            {
                allWeights[index++] = weightsInputToHidden[i, j];
            }
        }

        // Convert biasesHidden
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            allWeights[index++] = biasesHidden[i];
        }

        // Convert weightsHiddenToOutput
        for (int i = 0; i < HiddenLayerSize; i++)
        {
            for (int j = 0; j < OutputSize; j++)
            {
                allWeights[index++] = weightsHiddenToOutput[i, j];
            }
        }

        // Convert biasesOutput
        for (int i = 0; i < OutputSize; i++)
        {
            allWeights[index++] = biasesOutput[i];
        }

        return allWeights;
    }

    // Collect inputs for the network (raycast data + speed)
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

    // Perform matrix multiplication for network layers
    float[] MatrixMultiply(float[] inputs, float[,] weights, float[] biases)
    {
        int rows = weights.GetLength(1);
        float[] output = new float[rows];

        for (int i = 0; i < rows; i++)
        {
            output[i] = biases[i]; // Start with bias
            for (int j = 0; j < inputs.Length; j++)
            {
                output[i] += inputs[j] * weights[j, i];
            }
        }
        return output;
    }

    // Apply ReLU activation function
    void ApplyActivationFunction(float[] layer)
    {
        for (int i = 0; i < layer.Length; i++)
        {
            layer[i] = Mathf.Max(0, layer[i]); // ReLU for non-linearity
        }
    }

    // Select action based on output layer values
    int SelectAction()
    {
        int actionIndex = 0;
        float maxOutput = outputLayer[0];
        for (int i = 1; i < 3; i++) // Limited to three actions
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

        // Forward propagation
        hiddenLayer = MatrixMultiply(inputs, weightsInputToHidden, biasesHidden);
        ApplyActivationFunction(hiddenLayer);

        outputLayer = MatrixMultiply(hiddenLayer, weightsHiddenToOutput, biasesOutput);

        return SelectAction();
    }
}
