using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QuestionData", menuName = "QuestionData")]
public class QtsData : ScriptableObject
{
    [System.Serializable]
    public struct Question
    {
        public string questionText;         // Le texte de la question
        public string[] replies;            // Les réponses possibles
        public string replyDescription;     // Description associée à la réponse correcte
        public int correctReplyIndex;       // L'indice de la réponse correcte
    }

    public Question[] questions;            // Tableau des questions
}
