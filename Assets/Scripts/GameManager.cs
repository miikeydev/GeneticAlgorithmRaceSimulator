using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true;
    public GeneticAlgorithm geneticAlgorithm;
    public TruckController truckController;
    public NeuralNetController neuralNetController;
    public CheckpointManager checkpointManager;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public int numberOfIndividuals = 20;
    private int currentIndividualIndex = 0;
    private float checkpointTimer = 0f;
    private const float maxCheckpointTime = 5f;

    void Start()
    {
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

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

    void SaveCurrentCarData()
    {
        float fitnessScore = neuralNetController.GetFitnessScore();
        Debug.Log("Score de fitness � la fin de l'�pisode : " + fitnessScore);
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore, currentIndividualIndex);
    }
}
