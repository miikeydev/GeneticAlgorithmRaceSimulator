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
            // Sauvegarde le meilleur mod�le apr�s chaque g�n�ration
            float[] bestWeights = geneticAlgorithm.GetBestWeights();
            weightManager.SaveWeights(bestWeights, currentIndividualIndex);


            geneticAlgorithm.GenerateNewPopulation();
            currentIndividualIndex = 0;
            Debug.Log("Nouvelle g�n�ration cr��e !");
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
        //Debug.Log("Collision avec le Border d�tect�e. Application de la p�nalit� et fin de l'�pisode.");
        neuralNetController.ApplyBorderPenalty();
        EndEpisode();
    }



    void SaveCurrentCarData()
    {
        float fitnessScore = neuralNetController.GetFitnessScore();
        //Debug.Log("Score de fitness � la fin de l'�pisode : " + fitnessScore);
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore, currentIndividualIndex);
    }
}
