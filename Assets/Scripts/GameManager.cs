using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // Indicates if the game is currently playable
    public GeneticAlgorithm geneticAlgorithm; // Reference to the GeneticAlgorithm
    public TruckController truckController; // Reference to the TruckController
    public NeuralNetController neuralNetController; // Reference to the NeuralNetController
    public CheckpointManager checkpointManager; // Reference to the CheckpointManager
    public WeightManipulation weightManager; // Manages weight saving and loading

    private Vector3 initialPosition; // Initial spawn position for the truck
    private Quaternion initialRotation; // Initial spawn rotation for the truck

    public int numberOfIndividuals = 20; // Population size per generation
    private int currentIndividualIndex = 0; // Index of the current individual in the population
    private float checkpointTimer = 0f; // Timer to track checkpoint interaction
    public float maxCheckpointTime = 3f; // Maximum time allowed between checkpoints

    void Start()
    {
        // Store initial position and rotation for reset purposes
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

        weightManager = new WeightManipulation("best_weights.txt"); // Initialize weight manager

        // Initialize the first generation of individuals in the genetic algorithm
        geneticAlgorithm.InitializeFirstGeneration(
            numberOfIndividuals,
            neuralNetController.InputSize,
            neuralNetController.HiddenLayerSize,
            neuralNetController.OutputSize
        );

        ResetCar();
    }

    void Update()
    {
        checkpointTimer += Time.deltaTime;

        // If checkpoint time exceeds the maximum allowed, end the episode
        if (checkpointTimer >= maxCheckpointTime)
        {
            neuralNetController.ApplyEndEpisodePenalty();
            EndEpisode();
        }
    }

    public void OnCheckpointTouched()
    {
        // Reset the checkpoint timer when a checkpoint is touched
        checkpointTimer = 0f;
    }

    void EndEpisode()
    {
        // Save the current car's data and increment individual index
        SaveCurrentCarData();
        currentIndividualIndex++;

        // Check if we've gone through all individuals in the population
        if (currentIndividualIndex >= numberOfIndividuals)
        {
            // Save the best model after each generation
            float[] bestWeights = geneticAlgorithm.GetBestWeights();
            weightManager.SaveWeights(bestWeights, currentIndividualIndex);

            // Generate a new population and reset individual index
            geneticAlgorithm.GenerateNewPopulation();
            currentIndividualIndex = 0;
            Debug.Log("New generation created!");
        }

        ResetCar();
    }

    void ResetCar()
    {
        // Reset the car's position, rotation, and physics
        checkpointTimer = 0f;

        truckController.transform.position = initialPosition;
        truckController.transform.rotation = initialRotation;
        truckController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        truckController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Retrieve weights for the current individual and initialize neural network weights
        float[] weights = geneticAlgorithm.GetWeightsForIndividual(currentIndividualIndex);
        neuralNetController.InitializeWeights(weights);

        // Reset fitness and reactivate checkpoints
        neuralNetController.ResetFitness();
        checkpointManager.ReactivateAllCheckpoints();
    }

    public void HandleBorderCollision()
    {
        // Apply a penalty and end the episode if there is a border collision
        neuralNetController.ApplyBorderPenalty();
        EndEpisode();
    }

    void SaveCurrentCarData()
    {
        // Save the fitness score and weights of the current individual
        float fitnessScore = neuralNetController.GetFitnessScore();
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore, currentIndividualIndex);
    }
}
