using UnityEngine;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour
{
    private KinectSensor _Sensor;        // Capteur Kinect
    private BodyFrameReader _Reader;     // Lecteur de trame des corps
    private Body[] _Data = null;         // Tableau pour stocker les données des corps

    // Méthode pour récupérer les données des corps
    public Body[] GetData()
    {
        return _Data;
    }

    // Méthode appelée au démarrage
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();    // Récupère le capteur Kinect par défaut

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();    // Ouvre un lecteur de trame pour les corps

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();     // Ouvre le capteur Kinect s'il n'est pas déjà ouvert
            }
        }
    }

    // Méthode appelée à chaque frame
    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();    // Récupère la dernière trame des corps

            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];    // Initialise le tableau des corps
                }

                frame.GetAndRefreshBodyData(_Data);    // Copie les données des corps dans le tableau _Data

                frame.Dispose();    // Libère les ressources de la trame
                frame = null;
            }
        }
    }

    // Méthode appelée à la fermeture de l'application
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();    // Libère les ressources du lecteur de trame des corps
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();    // Ferme le capteur Kinect s'il est ouvert
            }

            _Sensor = null;
        }
    }
}
