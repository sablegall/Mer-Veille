using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText; // Référence au texte du timer affiché dans l'interface
    [SerializeField] float remainingTime; // Temps restant en secondes

    void Update()
    {
        // Réduire le temps restant en fonction du temps écoulé depuis la dernière frame
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            this.enabled = false; // Désactiver le script Timer pour arrêter la mise à jour dans Update
        }

        // Convertir le temps restant en secondes entières
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        // Afficher uniquement les secondes dans le texte du timer
        timerText.text = seconds.ToString("0");
    }
}
