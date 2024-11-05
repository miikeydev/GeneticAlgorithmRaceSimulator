using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // Détermine si la voiture est contrôlée manuellement ou par le réseau de neurones
    public GeneticAlgorithm geneticAlgorithm; // Référence au script GeneticAlgorithm
    public TruckController truckController; // Référence à l'objet TruckController (la voiture)
    public NeuralNetController neuralNetController; // Référence au NeuralNetController
    public CheckpointManager checkpointManager; // Référence au CheckpointManager

    private Vector3 initialPosition; // Position initiale de la voiture
    private Quaternion initialRotation; // Rotation initiale de la voiture

    public int numberOfIndividuals = 20; // Nombre total d'individus dans une génération
    public float carLifetime = 5f; // Durée de vie de chaque voiture (5 secondes)

    private int currentIndividualIndex = 0; // Index de l'individu actuel
    private float carTimer;

    void Start()
    {
        // Enregistrer la position et la rotation initiales de la voiture
        initialPosition = truckController.transform.position;
        initialRotation = truckController.transform.rotation;

        ResetCar(); // Initialiser la première voiture au début
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
                // Sélectionner les meilleurs individus après la génération complète
                geneticAlgorithm.SelectTopIndividuals();
                currentIndividualIndex = 0; // Réinitialiser pour la nouvelle génération
                Debug.Log("Nouvelle génération créée !");
            }

            // Réinitialiser la voiture
            ResetCar();
        }
    }

    void ResetCar()
    {
        carTimer = 0f;

        // Remettre la voiture à sa position et rotation initiales
        truckController.transform.position = initialPosition;
        truckController.transform.rotation = initialRotation;
        truckController.GetComponent<Rigidbody>().velocity = Vector3.zero;
        truckController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // Initialiser les poids de la voiture existante
        neuralNetController.InitializeWeights(
            geneticAlgorithm.GenerateRandomWeights(neuralNetController.InputSize, neuralNetController.HiddenLayerSize),
            geneticAlgorithm.GenerateRandomBiases(neuralNetController.HiddenLayerSize)
        );

        // Réinitialiser la fonction de fitness
        neuralNetController.ResetFitness(); // Cette ligne doit bien être exécutée
        checkpointManager.ReactivateAllCheckpoints();
    }


    void SaveCurrentCarData()
    {
        // Récupérer le score de fitness actuel
        float fitnessScore = neuralNetController.GetFitnessScore();

        // Afficher la valeur de la fitness dans la console
        Debug.Log("Score de fitness à la fin de sa vie : " + fitnessScore);

        // Sauvegarder les poids et le score de fitness dans le script GeneticAlgorithm
        geneticAlgorithm.SaveCarData(neuralNetController.GetWeights(), fitnessScore);
    }
}
