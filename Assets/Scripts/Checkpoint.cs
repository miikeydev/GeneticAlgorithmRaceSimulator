using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // Trouve le GameManager dans la scène
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet qui entre en collision a le tag "Truck"
        if (other.CompareTag("Truck"))
        {
            // Incrémenter la fonction de fitness via le GameManager
            if (gameManager != null && gameManager.neuralNetController != null)
            {
                gameManager.neuralNetController.IncrementFitness();
                gameManager.OnCheckpointTouched(); // Réinitialise le timer dans le GameManager
            }

            // Fait disparaître le checkpoint en le désactivant
            gameObject.SetActive(false);
        }
    }
}
