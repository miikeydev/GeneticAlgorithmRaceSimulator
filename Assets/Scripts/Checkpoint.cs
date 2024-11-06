using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager

    private void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object colliding has the tag "Truck"
        if (other.CompareTag("Truck"))
        {
            // Increment fitness via GameManager if available
            if (gameManager != null && gameManager.neuralNetController != null)
            {
                gameManager.neuralNetController.IncrementFitness();
                gameManager.OnCheckpointTouched(); // Reset the checkpoint timer in GameManager
            }

            // Disable the checkpoint to make it disappear
            gameObject.SetActive(false);
        }
    }
}
