using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;    // Référence au gestionnaire de source de corps
    public GameObject mJointObject;                 // Préfabriqué d'objet de joint

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();    // Dictionnaire pour suivre les objets de corps

    private List<JointType> _joints = new List<JointType>
    {
        JointType.Head,
    };

    void Update () 
    {
        // Récupère les données des corps depuis le gestionnaire de source de corps
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>(mBodies.Keys);

        // Parcourt les données des corps pour suivre les IDs de corps suivis
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

        // Supprime les objets de corps qui ne sont plus suivis
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                // Détruit l'objet de corps
                Destroy(mBodies[trackingId]);

                // Retire de la liste
                mBodies.Remove(trackingId);
            }
        }

        // Crée de nouveaux objets de corps pour les corps suivis
        foreach (var body in data)
        {
            if (body == null)
            { 
                continue;
            }

            if (body.IsTracked)
            {
                if (!mBodies.ContainsKey(body.TrackingId))
                {
                    // Crée un nouvel objet de corps s'il n'existe pas déjà
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                // Met à jour l'objet de corps existant avec les nouvelles données
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
    }

    // Crée un nouvel objet de corps avec des joints
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        // Crée les joints pour le corps
        foreach (JointType joint in _joints)
        {
            // Instancie un nouvel objet de joint à partir du préfabriqué
            GameObject newJoint = Instantiate(mJointObject);
            newJoint.name = joint.ToString();

            // Parente le joint à l'objet du corps
            newJoint.transform.parent = body.transform;
        }

        return body;
    }

    // Met à jour les positions des joints de l'objet de corps avec les données actuelles du corps
    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        foreach (JointType _joint in _joints)
        {
            // Récupère la position du joint depuis les données du corps Kinect
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z = 0;   // Ajuste la position en z (profondeur)

            // Trouve l'objet du joint dans l'objet de corps et met à jour sa position
            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }

    // Convertit un objet Joint en un Vector3 pour la position
    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 20, joint.Position.Y * 20, joint.Position.Z * 20);
    }
}
