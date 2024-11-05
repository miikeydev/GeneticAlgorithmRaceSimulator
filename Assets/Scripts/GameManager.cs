using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // Si vrai, contrôle manuel ; sinon, contrôle par le réseau de neurones
    public TruckController truckController;

    void Update()
    {
        // Exemple de basculement entre les modes avec la touche 'P'
        if (Input.GetKeyDown(KeyCode.P))
        {
            playable = !playable;

            if (playable)
            {
                // Réinitialiser les inputs du TruckController
                truckController.ResetInputs();
            }
        }
    }
}
