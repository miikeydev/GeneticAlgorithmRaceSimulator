using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // D�termine si la voiture est contr�l�e manuellement ou par le r�seau de neurones
    public GeneticAlgorithm geneticAlgorithm; // R�f�rence au script GeneticAlgorithm
    public TruckController truckController; // R�f�rence � l'objet TruckController (la voiture)
    public NeuralNetController neuralNetController; // R�f�rence au NeuralNetController
    public CheckpointManager checkpointManager; // R�f�rence au CheckpointManager

    private Vector3 initialPosition; // Position initiale de la voiture
    private Quaternion initialRotation; // Rotation initiale de la voiture

    public int numberOfIndividuals = 20; // Nombre total d'individus dans une g�n�ration
    public float carLifetime = 5f; // Dur�e de vie de chaque voiture (5 secondes)

    private int currentIndividualIndex = 0; // Index de l'individu actuel
    private float carTimer;

    void Start()
    {
        // Enregistrer la position et la rotation initiales de la voiture
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

        ResetCar(); // Initialiser la premi�re voiture au d�but
    }

    void Update()
    {
        carTimer += Time.fixedDeltaTime; // Utiliser `Time.deltaTime` pour suivre le temps de jeu

        if (carTimer >= carLifetime)
        {
            // Sauvegarder les poids et le score de fitness de la voiture actuelle
            SaveCurrentCarData();

            currentIndividualIndex++;

            if (currentIndividualIndex >= numberOfIndividuals)
            {
                // S�lectionner les meilleurs individus apr�s la g�n�ration compl�te
                geneticAlgorithm.SelectTopIndividuals();
                currentIndividualIndex = 0; // R�initialiser pour la nouvelle g�n�ration
                Debug.Log("Nouvelle g�n�ration cr��e !");
            }

            // R�initialiser la voiture
            ResetCar();
        }
    }

    void ResetCar()
    {
        carTimer = 0f;

        // Remettre la voiture � sa position et rotation initiales
        truckController.transform.position = initialPosition;
        truckController.transform.rotation = initialRotation;
        truckController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        truckController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Initialiser les poids de la voiture existante
        neuralNetController.InitializeWeights(
            geneticAlgorithm.GenerateRandomWeights(neuralNetController.InputSize, neuralNetController.HiddenLayerSize),
            geneticAlgorithm.GenerateRandomBiases(neuralNetController.HiddenLayerSize)
        );

        // R�initialiser la fonction de fitness
        neuralNetController.ResetFitness(); // Cette ligne doit bien �tre ex�cut�e
        checkpointManager.ReactivateAllCheckpoints();
    }


    void SaveCurrentCarData()
    {
        // R�cup�rer le score de fitness actuel
        float fitnessScore = neuralNetController.GetFitnessScore();

        // Afficher la valeur de la fitness dans la console
        Debug.Log("Score de fitness � la fin de sa vie : " + fitnessScore);

        // Sauvegarder les poids et le score de fitness dans le script GeneticAlgorithm
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore);
    }
}
