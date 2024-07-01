using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;       // Utilisation de TextMeshProUGUI pour le texte de la question
    public TextMeshProUGUI replyDescription;   // Utilisation de TextMeshProUGUI pour la description de la réponse
    public Button[] replyButtons;              // Tableau des boutons de réponses
    public QtsData qtsData;                    // Données des questions
    public GameObject StartQuiz;               // Panel de début de quiz
    public GameObject Right;                   // Panel pour la réponse correcte
    public GameObject Wrong;                   // Panel pour la réponse incorrecte
    public GameObject GameFinished;            // Panel pour la fin du jeu
    public TextMeshProUGUI timerText;          // Texte pour le timer

    private int currentQuestion = 0;           // Indice de la question actuelle

    // Méthode appelée au démarrage
    void Start()
    {
        SetQuestion(currentQuestion);          // Définit la première question
        Right.SetActive(false);                // Cache le panel de la réponse correcte
        Wrong.SetActive(false);                // Cache le panel de la réponse incorrecte
        GameFinished.SetActive(false);         // Cache le panel de fin du jeu
        replyDescription.gameObject.SetActive(false);  // Cache la description de la réponse au démarrage

        StartCoroutine(TransitionQuiz());      // Lance la coroutine pour la transition du quiz
    }

    // Coroutine pour la transition du quiz
    IEnumerator TransitionQuiz()
    {
        yield return new WaitForSeconds(5.0f); // Attendre 5 secondes
        StartQuiz.SetActive(false);            // Cache le panel de début du quiz
    }

    // Définit la question actuelle
    void SetQuestion(int questionIndex)
    {
        questionText.text = qtsData.questions[questionIndex].questionText;  // Définit le texte de la question

        // Supprime les anciens listeners avant d'en ajouter de nouveaux
        foreach (Button r in replyButtons)
        {
            r.onClick.RemoveAllListeners();
        }

        // Configure les boutons de réponse
        for (int i = 0; i < replyButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = replyButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = qtsData.questions[questionIndex].replies[i]; // Définit le texte des réponses
            }
            int replyIndex = i;
            replyButtons[i].onClick.AddListener(() =>
            {
                CheckReply(replyIndex); // Ajoute le listener pour vérifier la réponse
            });
        }
    }

    // Vérifie la réponse donnée
    public void CheckReply(int replyIndex)
    {
        // Affiche la description de la réponse
        replyDescription.text = qtsData.questions[currentQuestion].replyDescription;
        replyDescription.gameObject.SetActive(true);

        if (replyIndex == qtsData.questions[currentQuestion].correctReplyIndex)
        {
            Right.SetActive(true); // Active le panel de la réponse correcte
        }
        else
        {
            Wrong.SetActive(true); // Active le panel de la réponse incorrecte
        }

        // Désactive les colliders de tous les boutons de réponse
        foreach (Button r in replyButtons)
        {
            BoxCollider2D collider = r.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        // Lance la coroutine pour réactiver les colliders après 15 secondes et mettre à jour le timer
        StartCoroutine(ReEnableCollidersAfterDelay(20.0f));

        // Lance la coroutine pour passer à la question suivante
        StartCoroutine(Next());
    }

    // Coroutine pour réactiver les colliders après un délai
    IEnumerator ReEnableCollidersAfterDelay(float delay)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            int remainingSeconds = Mathf.CeilToInt(delay - elapsedTime);
            timerText.text = remainingSeconds.ToString(); // Met à jour le texte du timer
            yield return null; // Attend la prochaine frame
        }

        // Réactive les colliders de tous les boutons de réponse
        foreach (Button r in replyButtons)
        {
            BoxCollider2D collider = r.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        // Efface le texte du timer
        timerText.text = "";
    }

    // Coroutine pour passer à la question suivante
    IEnumerator Next()
    {
        yield return new WaitForSeconds(10); // Temps d'affichage des panels Right/Wrong

        currentQuestion++; // Passe à la question suivante

        if (currentQuestion < qtsData.questions.Length)
        {
            Reset(); // Réinitialise l'interface utilisateur et active tous les boutons de réponse
        }
        else
        {
            StartCoroutine(ShowGameFinishedAndChangeScene()); // Affiche le panel de fin du jeu et change de scène
        }
    }

    // Coroutine pour afficher la fin du jeu et changer de scène
    IEnumerator ShowGameFinishedAndChangeScene()
    {
        timerText.gameObject.SetActive(false); // Désactive le texte du timer
        GameFinished.SetActive(true);          // Active le panel de fin du jeu
        yield return new WaitForSeconds(3);    // Attend 3 secondes
        GameBehaviour.Instance.sceneToMoveToTransition(); // Change de scène
    }

    // Réinitialise l'interface utilisateur pour la question suivante
    public void Reset()
    {
        Right.SetActive(false);                // Cache le panel de la réponse correcte
        Wrong.SetActive(false);                // Cache le panel de la réponse incorrecte
        replyDescription.gameObject.SetActive(false); // Cache la description de la réponse
        SetQuestion(currentQuestion);          // Définit la prochaine question
    }
}
