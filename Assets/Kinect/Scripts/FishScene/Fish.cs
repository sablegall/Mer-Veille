using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    // Vitesse de déplacement du poisson
    public float speed = 2f;
    // Position de départ du poisson
    private Vector3 startPosition;
    // Limite gauche du déplacement
    private float leftLimit = -20f;
    // Limite droite du déplacement
    private float rightLimit = 20f;
    // Indicateur de direction du mouvement (vrai si le poisson se déplace vers la droite)
    private bool movingRight = true;

    // Méthode Start appelée une fois au début
    private void Start()
    {
        // Enregistrer la position de départ du poisson
        startPosition = transform.position;
    }

    // Méthode Update appelée une fois par frame
    private void Update()
    {
        // Calcul du mouvement horizontal en fonction de la vitesse et du temps écoulé
        float movement = speed * Time.deltaTime;
        if (movingRight)
        {
            // Déplacer le poisson vers la droite
            transform.Translate(Vector3.right * movement);
        }
        else
        {
            // Déplacer le poisson vers la gauche
            transform.Translate(Vector3.left * movement);
        }

        // Inverser la direction si le poisson atteint les limites
        if (transform.position.x >= rightLimit)
        {
            movingRight = false; // Changer de direction pour aller vers la gauche
        }
        else if (transform.position.x <= leftLimit)
        {
            movingRight = true; // Changer de direction pour aller vers la droite
        }
    }
}
