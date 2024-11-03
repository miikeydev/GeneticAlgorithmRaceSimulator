using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet qui entre en collision a le tag "Truck"
        if (other.CompareTag("Truck"))
        {
            // Fait dispara�tre le checkpoint en le d�sactivant
            gameObject.SetActive(false);

        }
    }
}
