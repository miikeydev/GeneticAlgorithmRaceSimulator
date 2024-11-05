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

            // Utilise la r�f�rence globale pour incr�menter la fonction de fitness
            if (gameManager != null && gameManager.neuralNetController != null)
            {
                gameManager.neuralNetController.IncrementFitness();
            }

            // Fait dispara�tre le checkpoint en le d�sactivant
            gameObject.SetActive(false);
        }
    }
}
