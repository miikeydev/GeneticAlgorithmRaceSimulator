using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    private List<GameObject> checkpoints = new List<GameObject>(); // List to store all checkpoint objects

    void Start()
    {
        // Retrieve all checkpoints in the scene and add them to the list
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            checkpoints.Add(checkpoint);
        }
    }

    // Method to reactivate all checkpoints
    public void ReactivateAllCheckpoints()
    {
        foreach (GameObject checkpoint in checkpoints)
        {
            checkpoint.SetActive(true);
        }
    }
}
