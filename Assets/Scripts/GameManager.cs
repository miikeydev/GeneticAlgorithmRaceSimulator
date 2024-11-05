using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // Determines if the car is manually controlled or by the neural network
    public GeneticAlgorithm geneticAlgorithm; // Reference to the GeneticAlgorithm script
    public TruckController truckController; // Reference to the TruckController object (the car)
    public NeuralNetController neuralNetController; // Reference to the NeuralNetController
    public CheckpointManager checkpointManager; // Reference to the CheckpointManager

    private Vector3 initialPosition; // Initial position of the car
    private Quaternion initialRotation; // Initial rotation of the car

    public int numberOfIndividuals = 20; // Total number of individuals in a generation
    public float carLifetime = 5f; // Lifetime of each car (5 seconds)

    private int currentIndividualIndex = 0; // Index of the current individual
    private float carTimer;

    void Start()
    {
        // Record the initial position and rotation of the car
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

        // Initialize the first generation of random weights
        geneticAlgorithm.InitializeFirstGeneration(
            numberOfIndividuals,
            neuralNetController.InputSize,
            neuralNetController.HiddenLayerSize,
            neuralNetController.OutputSize
        );

        ResetCar(); // Initialize the first car
    }

    void Update()
    {
        carTimer += Time.deltaTime; // Use `Time.deltaTime` to track game time

        if (carTimer >= carLifetime)
        {
            // Save the weights and fitness score of the current car
            SaveCurrentCarData();

            currentIndividualIndex++;

            if (currentIndividualIndex >= numberOfIndividuals)
            {
                // Generate a new population after the complete generation
                geneticAlgorithm.GenerateNewPopulation();
                currentIndividualIndex = 0; // Reset for the new generation
                Debug.Log("New generation created!");
            }

            // Reset the car with new weights
            ResetCar();
        }
    }

    void ResetCar()
    {
        carTimer = 0f;

        // Reset the position and rotation of the car
        truckController.transform.position = initialPosition;
        truckController.transform.rotation = initialRotation;
        truckController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        truckController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Get the weights for the current individual
        float[] weights = geneticAlgorithm.GetWeightsForIndividual(currentIndividualIndex);
        neuralNetController.InitializeWeights(weights);

        // Reset the fitness
        neuralNetController.ResetFitness();
        checkpointManager.ReactivateAllCheckpoints();
    }

    void SaveCurrentCarData()
    {
        // Get the current fitness score
        float fitnessScore = neuralNetController.GetFitnessScore();

        // Display the fitness value in the console
        Debug.Log("Fitness score at the end of life: " + fitnessScore);

        // Save the weights and fitness score in the GeneticAlgorithm script
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore, currentIndividualIndex);
    }
}
