using System.Collections;
using UnityEngine;

public class Head : MonoBehaviour
{
    public Transform mHandMesh;  // Mesh de la main à positionner
    public QuestionManager questionManager;  // Gestionnaire des questions
    private Collider2D headCollider;  // Collider attaché à la tête

    private void Start()
    {
        // Récupération du collider attaché à ce GameObject
        headCollider = GetComponent<Collider2D>();

        // Si le QuestionManager n'est pas assigné dans l'inspecteur, le chercher dans la scène
        if (questionManager == null)
        {
            questionManager = FindObjectOfType<QuestionManager>();
            if (questionManager == null)
            {
                Debug.LogError("QuestionManager non trouvé !");
            }
        }

        // Désactiver le rendu et le collider au démarrage et commencer la coroutine pour les réactiver
        StartCoroutine(DeactivateAndReactivateHead(13f));
    }

    private void Update()
    {
        // Positionner le mesh de la main en interpolant vers la position actuelle de la tête
        mHandMesh.position = Vector3.Lerp(mHandMesh.position, transform.position, Time.deltaTime * 15.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifier quelle zone a été touchée par la tête en fonction de la balise du GameObject
        if (collision.gameObject.CompareTag("ZoneGauche"))
        {
            Debug.Log("Collision avec ZoneGauche");
            CheckReply(0); // Supposant que ZoneGauche correspond à la première réponse
        }
        else if (collision.gameObject.CompareTag("ZoneDroite"))
        {
            Debug.Log("Collision avec ZoneDroite");
            CheckReply(1); // Supposant que ZoneDroite correspond à la deuxième réponse
        }
    }

    private void CheckReply(int replyIndex)
    {
        // Vérifier si le QuestionManager est assigné
        if (questionManager != null)
        {
            // Appeler la méthode CheckReply du QuestionManager avec l'index de réponse fourni
            questionManager.CheckReply(replyIndex);
        }
        else
        {
            Debug.LogError("QuestionManager n'est pas assigné !");
        }
    }

    public IEnumerator DeactivateAndReactivateHead(float delay)
    {
        // Désactiver le rendu et les collisions
        headCollider.enabled = false;

        // Attendre pendant le délai spécifié
        yield return new WaitForSeconds(delay);

        // Réactiver le rendu et les collisions
        headCollider.enabled = true;
    }
}
