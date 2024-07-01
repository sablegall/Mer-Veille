using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceViewHand : MonoBehaviour 
{
    // Référence au gestionnaire de la source de corps
    public BodySourceManager mBodySourceManager;
    // Préfabriqué pour représenter les articulations
    public GameObject mJointObject;
    
    // Dictionnaire pour stocker les objets de corps actifs
    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();

    // Liste des types d'articulations suivies
    private List<JointType> _joints = new List<JointType>
    {
        JointType.HandRight,
    };

    // Méthode Update appelée une fois par frame
    void Update () 
    {
        #region Get Kinect Data
        // Récupérer les données de corps du gestionnaire
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            return;
        }

        // Liste des IDs des corps suivis
        List<ulong> trackedIds = new List<ulong>(mBodies.Keys);
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }
        #endregion

        #region Delete Kinect bodies
        // Supprimer les objets de corps qui ne sont plus suivis
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                // Détruire l'objet de corps
                Destroy(mBodies[trackingId]);
                // Retirer du dictionnaire
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect bodies
        // Créer ou mettre à jour les objets de corps pour les corps suivis
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                // Créer un nouvel objet de corps si ce corps n'est pas déjà suivi
                if (!mBodies.ContainsKey(body.TrackingId))
                {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                // Mettre à jour l'objet de corps avec les nouvelles données
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }

    // Créer un nouvel objet de corps
    private GameObject CreateBodyObject(ulong id)
    {
        // Créer un parent pour les articulations
        GameObject body = new GameObject("Body:" + id);

        // Créer des objets pour chaque articulation suivie
        foreach (JointType joint in _joints)
        {
            // Instancier un nouvel objet d'articulation
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = joint.ToString();

            // Définir le parent de l'articulation comme étant le corps
            newJoint.transform.parent = body.transform;
        }

        return body;
    }

    // Mettre à jour un objet de corps avec les nouvelles données
    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        // Mettre à jour les positions des articulations
        foreach (JointType _joint in _joints)
        {
            // Obtenir la nouvelle position cible
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0;

            // Obtenir l'objet de l'articulation et définir la nouvelle position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }

    // Convertir les coordonnées d'une articulation Kinect en Vector3
    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 20, joint.Position.Y * 20, joint.Position.Z * 20);
    }
}
