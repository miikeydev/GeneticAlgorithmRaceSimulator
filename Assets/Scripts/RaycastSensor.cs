using UnityEngine;

public class RaycastSensor : MonoBehaviour
{
    [Header("Raycast Parameters")]
    public Transform raycastOrigin; // Child GameObject to emit rays around the cube
    public float rayDistance = 10f; // Maximum raycast distance
    public LayerMask detectionLayer; // Layer to detect with the raycasts

    private float[] raycastData; // Array to store distances and tag values of detected objects

    void Start()
    {
        // Initialize the array with twice the number of raycasts (for distance and tag values)
        raycastData = new float[16]; // 8 raycasts * 2 (distance + tag)
    }

    void Update()
    {
        // Set horizontal directions around the cube (0° vertical angle)
        Vector3[] directions = {
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -30, 0) * Vector3.forward), // Front left
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 0, 0) * Vector3.forward),    // Front center
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 30, 0) * Vector3.forward),   // Front right
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -90, 0) * Vector3.forward),  // Left 1
            raycastOrigin.TransformDirection(Quaternion.Euler(0, -120, 0) * Vector3.forward), // Left 2
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 90, 0) * Vector3.forward),   // Right 1
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 120, 0) * Vector3.forward),  // Right 2
            raycastOrigin.TransformDirection(Quaternion.Euler(0, 180, 0) * Vector3.forward)   // Rear
        };

        // Perform raycasts and update data
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit hit;
            UpdateRaycastData(i * 2, directions[i], out hit);

            // Select ray color based on detected object tag
            Color rayColor = hit.collider != null ? GetRayColor(hit.collider.tag) : Color.red;

            // Draw the ray up to the hit point or to max distance if no collision
            Vector3 endPosition = hit.collider != null ? hit.point : raycastOrigin.position + directions[i] * rayDistance;
            Debug.DrawLine(raycastOrigin.position, endPosition, rayColor);
        }
    }

    void UpdateRaycastData(int index, Vector3 direction, out RaycastHit hit)
    {
        // Perform raycast from raycastOrigin in the specified direction
        if (Physics.Raycast(raycastOrigin.position, direction, out hit, rayDistance, detectionLayer))
        {
            raycastData[index] = hit.distance; // Store distance of hit
            raycastData[index + 1] = GetTagValue(hit.collider.tag); // Store value associated with detected tag
        }
        else
        {
            raycastData[index] = rayDistance; // Max distance if no hit
            raycastData[index + 1] = 0f; // Default value if no tag detected
        }
    }

    private float GetTagValue(string tag)
    {
        // Return a specific value for each tag
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
        // Return a specific color for each tag
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
        // Public method to access the raycast data
        return raycastData;
    }
}
