using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    // Référence au Transform représentant le mesh de la main
    public Transform mHandMesh;
    // Référence au collider de la main
    private Collider2D handCollider;

    // Méthode Start appelée une fois au début
    private void Start()
    {
        // Obtenir le Collider2D attaché à cet objet
        handCollider = GetComponent<Collider2D>();

        // Désactiver le collider au démarrage et commencer la coroutine pour le réactiver après un délai
        StartCoroutine(DeactivateAndReactivateHand(5f));
    }

    // Méthode Update appelée une fois par frame
    private void Update()
    {
        // Interpoler la position du mesh de la main vers la position actuelle de l'objet
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    // Méthode appelée lors de la collision avec un autre collider 2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifier le tag de l'objet avec lequel la collision a eu lieu et changer de scène en conséquence
        if (collision.gameObject.CompareTag("Bar"))
        {
            GameBehaviour.Instance.sceneToMoveToBar();
        }
        else if (collision.gameObject.CompareTag("Dorade"))
        {
            GameBehaviour.Instance.sceneToMoveToDorade();
        }
        else if (collision.gameObject.CompareTag("Grondin"))
        {
            GameBehaviour.Instance.sceneToMoveToGrondin();
        }
        else if (collision.gameObject.CompareTag("Sar"))
        {
            GameBehaviour.Instance.sceneToMoveToSar();
        }
        else if (collision.gameObject.CompareTag("Syngnathe"))
        {
            GameBehaviour.Instance.sceneToMoveToSyngnathe();
        }
        else if (collision.gameObject.CompareTag("BtnStart"))
        {
            Debug.Log("collision bouton");
            GameBehaviour.Instance.sceneToMoveToTransition();
        }
    }

    // Coroutine pour désactiver et réactiver le collider de la main après un délai
    public IEnumerator DeactivateAndReactivateHand(float delay)
    {
        // Désactiver le collider
        handCollider.enabled = false;

        // Attendre le délai spécifié
        yield return new WaitForSeconds(delay);

        // Réactiver le collider des collisions
        handCollider.enabled = true;
    }
}
