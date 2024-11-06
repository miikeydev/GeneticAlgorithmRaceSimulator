using UnityEngine;

public class RaycastSensor : MonoBehaviour
{
    [Header("Paramètres des Raycasts")]
    public Transform raycastOrigin; // GameObject enfant pour lancer les rayons autour du cube
    public float rayDistance = 10f; // Distance maximale des raycasts
    public LayerMask detectionLayer; // Layer des objets que le raycast doit détecter

    private float[] raycastData; // Tableau pour stocker les distances et les valeurs de tags

    void Start()
    {
        // Initialiser le tableau avec le double du nombre de raycasts (pour inclure les distances et les tags)
        raycastData = new float[16]; // 8 raycasts * 2 (distance + tag)
    }

    void Update()
    {
        // Directions horizontales autour du cube (0° d'inclinaison)
        Vector3[] directions = {
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -30, 0) * Vector3.forward), // Avant gauche
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 0, 0) * Vector3.forward),    // Avant centre
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 30, 0) * Vector3.forward),   // Avant droite
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -90, 0) * Vector3.forward),  // Gauche 1
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -120, 0) * Vector3.forward), // Gauche 2
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 90, 0) * Vector3.forward),   // Droite 1
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 120, 0) * Vector3.forward),  // Droite 2
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 180, 0) * Vector3.forward)   // Arrière
        };

        // Effectue les raycasts et met à jour les données
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit hit;
            UpdateRaycastData(i * 2, directions[i], out hit);

            // Choisir la couleur du rayon en fonction de la détection
            Color rayColor = hit.collider != null ? GetRayColor(hit.collider.tag) : Color.red;

            // Dessiner le rayon jusqu'au point d'impact ou jusqu'à sa distance maximale
            Vector3 endPosition = hit.collider != null ? hit.point : raycastOrigin.position + directions[i] * rayDistance;
            Debug.DrawLine(raycastOrigin.position, endPosition, rayColor);
        }
    }

    void UpdateRaycastData(int index, Vector3 direction, out RaycastHit hit)
    {
        if (Physics.Raycast(raycastOrigin.position, direction, out hit, rayDistance, detectionLayer))
        {
            raycastData[index] = hit.distance;
            raycastData[index + 1] = GetTagValue(hit.collider.tag);
        }
        else
        {
            raycastData[index] = rayDistance; // Si rien n'est détecté, on enregistre la distance max
            raycastData[index + 1] = 0f; // Aucun tag
        }
    }

    private float GetTagValue(string tag)
    {
        switch (tag)
        {
            case "Circuit":
                return 1f;
            case "Border":
                return -1f;
            case "Checkpoint":
                return 2f;
            default:
                return 0f;
        }
    }

    private Color GetRayColor(string tag)
    {
        switch (tag)
        {
            case "Circuit":
                return Color.green;
            case "Border":
                return Color.yellow;
            case "Checkpoint":
                return Color.blue;
            default:
                return Color.red;
        }
    }

    public float[] GetRaycastData()
    {
        return raycastData;
    }
}
