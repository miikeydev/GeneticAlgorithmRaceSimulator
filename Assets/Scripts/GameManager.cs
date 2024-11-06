using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true;
    public GeneticAlgorithm geneticAlgorithm;
    public TruckController truckController;
    public NeuralNetController neuralNetController;
    public CheckpointManager checkpointManager;
    public WeightManipulation weightManager;



    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public int numberOfIndividuals = 20;
    private int currentIndividualIndex = 0;
    private float checkpointTimer = 0f;
    public float maxCheckpointTime = 3f;

    void Start()
    {
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

        weightManager = new WeightManipulation("best_weights.txt"); // Initialise WeightManipulation

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

        if (checkpointTimer >= maxCheckpointTime)
        {
            neuralNetController.ApplyEndEpisodePenalty();
            EndEpisode();
        }
    }


    public void OnCheckpointTouched()
    {
        checkpointTimer = 0f;
    }



    void EndEpisode()
    {
        SaveCurrentCarData();
        currentIndividualIndex++;

        if (currentIndividualIndex >= numberOfIndividuals)
        {
            // Sauvegarde le meilleur modèle après chaque génération
            float[] bestWeights = geneticAlgorithm.GetBestWeights();
            weightManager.SaveWeights(bestWeights, currentIndividualIndex);


            geneticAlgorithm.GenerateNewPopulation();
            currentIndividualIndex = 0;
            Debug.Log("Nouvelle génération créée !");
        }

        ResetCar();
    }

    void ResetCar()
    {
        checkpointTimer = 0f;

        truckController.transform.position = initialPosition;
        truckController.transform.rotation = initialRotation;
        truckController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        truckController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        float[] weights = geneticAlgorithm.GetWeightsForIndividual(currentIndividualIndex);
        neuralNetController.InitializeWeights(weights);

        neuralNetController.ResetFitness();
        checkpointManager.ReactivateAllCheckpoints();
    }

    public void HandleBorderCollision()
    {
        //Debug.Log("Collision avec le Border détectée. Application de la pénalité et fin de l'épisode.");
        neuralNetController.ApplyBorderPenalty();
        EndEpisode();
    }



    void SaveCurrentCarData()
    {
        float fitnessScore = neuralNetController.GetFitnessScore();
        //Debug.Log("Score de fitness à la fin de l'épisode : " + fitnessScore);
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore, currentIndividualIndex);
    }
}
