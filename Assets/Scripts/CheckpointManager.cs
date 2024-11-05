using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    private List<GameObject> checkpoints = new List<GameObject>();

    void Start()
    {
        // Récupère tous les checkpoints dans la scène et les ajoute à la liste
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            checkpoints.Add(checkpoint);
        }
    }

    // Méthode pour réactiver tous les checkpoints
    public void ReactivateAllCheckpoints()
    {
        foreach (GameObject checkpoint in checkpoints)
        {
            checkpoint.SetActive(true);
        }
    }
}
