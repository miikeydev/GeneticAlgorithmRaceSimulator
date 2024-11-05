using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    private List<GameObject> checkpoints = new List<GameObject>();

    void Start()
    {
        // R�cup�re tous les checkpoints dans la sc�ne et les ajoute � la liste
        foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            checkpoints.Add(checkpoint);
        }
    }

    // M�thode pour r�activer tous les checkpoints
    public void ReactivateAllCheckpoints()
    {
        foreach (GameObject checkpoint in checkpoints)
        {
            checkpoint.SetActive(true);
        }
    }
}
