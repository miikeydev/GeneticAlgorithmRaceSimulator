using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool playable = true; // Si vrai, contr�le manuel ; sinon, contr�le par le r�seau de neurones
    public TruckController truckController;

    void Update()
    {
        // Exemple de basculement entre les modes avec la touche 'P'
        if (Input.GetKeyDown(KeyCode.P))
        {
            playable = !playable;

            if (playable)
            {
                // R�initialiser les inputs du TruckController
                truckController.ResetInputs();
            }
        }
    }
}
