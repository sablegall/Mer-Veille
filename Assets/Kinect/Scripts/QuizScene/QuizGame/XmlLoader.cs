using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlLoader : MonoBehaviour
{
    public TextAsset xmlFile;   // Fichier XML contenant les questions
    public QtsData qtsData;     // Référence à l'objet QtsData pour stocker les questions

    // Méthode appelée au démarrage
    void Start()
    {
        LoadXml();              // Charge le fichier XML
    }

    // Méthode pour charger et parser le fichier XML
    void LoadXml()
    {
        if (xmlFile == null)
        {
            Debug.LogError("XML file not assigned."); // Affiche une erreur si le fichier XML n'est pas assigné
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();      // Crée un nouvel objet XmlDocument
        xmlDoc.LoadXml(xmlFile.text);                // Charge le contenu du fichier XML dans l'objet XmlDocument

        XmlNodeList questionList = xmlDoc.GetElementsByTagName("Question"); // Récupère tous les nœuds "Question"
        List<QtsData.Question> questions = new List<QtsData.Question>();    // Crée une liste pour stocker les questions

        // Parcourt chaque nœud "Question"
        foreach (XmlNode questionInfo in questionList)
        {
            XmlNodeList questionContent = questionInfo.ChildNodes; // Récupère les nœuds enfants de la question
            QtsData.Question question = new QtsData.Question();    // Crée une nouvelle instance de QtsData.Question

            // Parcourt chaque nœud enfant de la question
            foreach (XmlNode qItem in questionContent)
            {
                if (qItem.Name == "QuestionText")
                {
                    question.questionText = qItem.InnerText; // Assigne le texte de la question
                }
                else if (qItem.Name == "Replies")
                {
                    List<string> replies = new List<string>(); // Crée une liste pour stocker les réponses
                    foreach (XmlNode reply in qItem.ChildNodes)
                    {
                        replies.Add(reply.InnerText); // Ajoute chaque réponse à la liste
                    }
                    question.replies = replies.ToArray(); // Convertit la liste en tableau et assigne les réponses
                }
                else if (qItem.Name == "ReplyDescription")
                {
                    question.replyDescription = qItem.InnerText; // Assigne la description de la réponse
                }
                else if (qItem.Name == "CorrectReplyIndex")
                {
                    question.correctReplyIndex = int.Parse(qItem.InnerText); // Assigne l'indice de la réponse correcte
                }
            }
            questions.Add(question); // Ajoute la question à la liste des questions
        }

        // Vérifie si des questions ont été chargées
        if (questions.Count > 0)
        {
            qtsData.questions = questions.ToArray(); // Convertit la liste en tableau et assigne les questions à QtsData
            Debug.Log("Questions loaded successfully. Total questions: " + questions.Count); // Affiche un message de succès
        }
        else
        {
            Debug.LogError("No questions loaded."); // Affiche une erreur si aucune question n'a été chargée
        }
    }
}
