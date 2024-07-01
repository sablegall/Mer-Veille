using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Nom de la prochaine scène à charger, configurable dans l'inspecteur
    public string nextSceneName;

    // Délai avant de changer de scène, configurable dans l'inspecteur
    public float delay = 2f;

    // Méthode Start appelée au début, démarrant la coroutine pour changer de scène après un délai
    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    // Coroutine pour attendre le délai spécifié avant de changer de scène
    IEnumerator ChangeSceneAfterDelay()
    {
        // Attendre le délai spécifié avant de continuer
        yield return new WaitForSeconds(delay);
        
        // Changer la scène en utilisant le nom spécifié
        SceneManager.LoadScene(nextSceneName);
    }
}
