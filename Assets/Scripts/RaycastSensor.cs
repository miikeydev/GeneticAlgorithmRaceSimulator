using UnityEngine;

public class RaycastSensor : MonoBehaviour
{
    [Header("Paramètres des Raycasts")]
    public float rayDistance = 10f; // Distance maximale des raycasts
    public LayerMask detectionLayer; // Layer des objets que le raycast doit détecter
    public float rayAngle = 15f; // Angle d'inclinaison en degrés

    private float[] raycastData; // Tableau pour stocker les distances et les valeurs de tags

    void Start()
    {
        // Initialiser le tableau avec le double du nombre de raycasts (pour inclure les distances et les tags)
        raycastData = new float[16]; // 8 raycasts * 2 (distance + tag)
    }

    void Update()
    {
        // Calcul des directions inclinées vers le bas
        Vector3[] directions = {
            Quaternion.Euler(-rayAngle, -30, 0) * transform.forward, // Avant gauche
            Quaternion.Euler(-rayAngle, 0, 0) * transform.forward,   // Avant centre
            Quaternion.Euler(-rayAngle, 30, 0) * transform.forward,  // Avant droite
            Quaternion.Euler(-rayAngle, -90, 0) * transform.forward, // Gauche 1
            Quaternion.Euler(-rayAngle, -120, 0) * transform.forward,// Gauche 2
            Quaternion.Euler(-rayAngle, 90, 0) * transform.forward,  // Droite 1
            Quaternion.Euler(-rayAngle, 120, 0) * transform.forward, // Droite 2
            Quaternion.Euler(-rayAngle, 180, 0) * transform.forward  // Arrière
        };

        // Effectue les raycasts et met à jour les données
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit hit;
            UpdateRaycastData(i * 2, directions[i], out hit); // Mise à jour avec distance et tag
        }

        // Visualisation des raycasts avec Debug.DrawRay (si besoin de les voir pour le débogage)
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i] * rayDistance, Color.red);
        }
    }

    void UpdateRaycastData(int index, Vector3 direction, out RaycastHit hit)
    {
        if (Physics.Raycast(transform.position, direction, out hit, rayDistance, detectionLayer))
        {
            raycastData[index] = hit.distance;
            raycastData[index + 1] = GetTagValue(hit.collider.tag);

            // Debug.Log pour afficher la distance et le tag détecté
            // Debug.Log($"Raycast {index / 2}: Distance = {hit.distance}, Tag = {hit.collider.tag}");
        }
        else
        {
            raycastData[index] = rayDistance; // Si rien n'est détecté, on enregistre la distance max
            raycastData[index + 1] = 0f; // Aucun tag

            // Debug.Log pour afficher qu'aucun obstacle n'a été détecté
            // Debug.Log($"Raycast {index / 2}: Distance = {rayDistance} (aucun obstacle détecté)");
        }
    }

    // Méthode pour obtenir une valeur basée sur le tag détecté
    private float GetTagValue(string tag)
    {
        switch (tag)
        {
            case "Circuit":
                return 1f; // Exemple de valeur pour le tag "Circuit"
            case "Border":
                return -1f; // Exemple de valeur pour le tag "Border"
            case "Checkpoint":
                return 2f; // Exemple de valeur pour le tag "Checkpoint"
            default:
                return 0f; // Valeur par défaut si aucun tag pertinent n'est détecté
        }
    }

    // Getter pour accéder aux distances et valeurs des raycasts
    public float[] GetRaycastData()
    {
        return raycastData;
    }
}