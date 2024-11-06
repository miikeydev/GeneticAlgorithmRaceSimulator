using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // Trouve le GameManager dans la sc�ne
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet qui entre en collision a le tag "Truck"
        if (other.CompareTag("Truck"))
        {
            // Incr�menter la fonction de fitness via le GameManager
            if (gameManager != null && gameManager.neuralNetController != null)
            {
                gameManager.neuralNetController.IncrementFitness();
                gameManager.OnCheckpointTouched(); // R�initialise le timer dans le GameManager
            }

            // Fait dispara�tre le checkpoint en le d�sactivant
            gameObject.SetActive(false);
        }
    }
}
