using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet qui entre en collision a le tag "Truck"
        if (other.CompareTag("Truck"))
        {
            // Fait disparaître le checkpoint en le désactivant
            gameObject.SetActive(false);

        }
    }
}
