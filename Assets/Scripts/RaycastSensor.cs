using UnityEngine;

public class RaycastSensor : MonoBehaviour
{
    [Header("Param�tres des Raycasts")]
    public float rayDistance = 10f; // Distance maximale des raycasts
    public LayerMask detectionLayer; // Layer des objets que le raycast doit d�tecter
    public float rayAngle = 15f; // Angle d'inclinaison en degr�s

    private float[] raycastData; // Tableau pour stocker les distances et les valeurs de tags

    void Start()
    {
        // Initialiser le tableau avec le double du nombre de raycasts (pour inclure les distances et les tags)
        raycastData = new float[16]; // 8 raycasts * 2 (distance + tag)
    }

    void Update()
    {
        // Calcul des directions inclin�es vers le bas
        Vector3[] directions = {
            Quaternion.Euler(-rayAngle, -30, 0) * transform.forward, // Avant gauche
            Quaternion.Euler(-rayAngle, 0, 0) * transform.forward,   // Avant centre
            Quaternion.Euler(-rayAngle, 30, 0) * transform.forward,  // Avant droite
            Quaternion.Euler(-rayAngle, -90, 0) * transform.forward, // Gauche 1
            Quaternion.Euler(-rayAngle, -120, 0) * transform.forward,// Gauche 2
            Quaternion.Euler(-rayAngle, 90, 0) * transform.forward,  // Droite 1
            Quaternion.Euler(-rayAngle, 120, 0) * transform.forward, // Droite 2
            Quaternion.Euler(-rayAngle, 180, 0) * transform.forward  // Arri�re
        };

        // Effectue les raycasts et met � jour les donn�es
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit hit;
            UpdateRaycastData(i * 2, directions[i], out hit); // Mise � jour avec distance et tag
        }

        // Visualisation des raycasts avec Debug.DrawRay (si besoin de les voir pour le d�bogage)
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

            // Debug.Log pour afficher la distance et le tag d�tect�
            // Debug.Log($"Raycast {index / 2}: Distance = {hit.distance}, Tag = {hit.collider.tag}");
        }
        else
        {
            raycastData[index] = rayDistance; // Si rien n'est d�tect�, on enregistre la distance max
            raycastData[index + 1] = 0f; // Aucun tag

            // Debug.Log pour afficher qu'aucun obstacle n'a �t� d�tect�
            // Debug.Log($"Raycast {index / 2}: Distance = {rayDistance} (aucun obstacle d�tect�)");
        }
    }

    // M�thode pour obtenir une valeur bas�e sur le tag d�tect�
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
                return 0f; // Valeur par d�faut si aucun tag pertinent n'est d�tect�
        }
    }

    // Getter pour acc�der aux distances et valeurs des raycasts
    public float[] GetRaycastData()
    {
        return raycastData;
    }
}