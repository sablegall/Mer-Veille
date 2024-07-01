using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBehaviour : MonoBehaviour
{
    // Déclaration d'une instance statique de GameBehaviour
    public static GameBehaviour Instance;

    // Méthode Awake appelée au moment où l'objet est instancié
    private void Awake()
    {
        // Vérifie si l'instance est déjà définie
        if (Instance == null)
        {
            // Si non, définit cette instance comme l'instance unique
            Instance = this;
        }
        else if (Instance != this)
        {
            // Si une autre instance existe déjà, détruit cet objet pour garantir un singleton
            Destroy(gameObject);
        }
    }

    // Méthode pour charger la scène "Transition"
    public void sceneToMoveToTransition()
    {
        SceneManager.LoadScene("Transition");
    }

    // Méthode pour charger la scène "BarScene"
    public void sceneToMoveToBar()
    {
        SceneManager.LoadScene("BarScene"); 
    }

    // Méthode pour charger la scène "DoradeScene"
    public void sceneToMoveToDorade()
    {
        SceneManager.LoadScene("DoradeScene"); 
    }

    // Méthode pour charger la scène "GrondinScene"
    public void sceneToMoveToGrondin()
    {
        SceneManager.LoadScene("GrondinScene"); 
    }

    // Méthode pour charger la scène "SarScene"
    public void sceneToMoveToSar()
    {
        SceneManager.LoadScene("SarScene"); 
    }

    // Méthode pour charger la scène "SyngnatheScene"
    public void sceneToMoveToSyngnathe()
    {
        SceneManager.LoadScene("SyngnatheScene");
    }
}
