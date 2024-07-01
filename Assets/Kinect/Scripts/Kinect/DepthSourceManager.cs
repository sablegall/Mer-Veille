using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class DepthSourceManager : MonoBehaviour
{
    // Référence au capteur Kinect
    private KinectSensor _Sensor;
    // Lecteur de trame de profondeur
    private DepthFrameReader _Reader;
    // Tableau pour stocker les données de profondeur
    private ushort[] _Data;

    // Méthode pour obtenir les données de profondeur
    public ushort[] GetData()
    {
        return _Data;
    }

    // Méthode Start appelée une fois au début
    void Start()
    {
        // Obtenir le capteur Kinect par défaut
        _Sensor = KinectSensor.GetDefault();

        // Si un capteur est trouvé, initialiser le lecteur de trame de profondeur
        if (_Sensor != null)
        {
            // Ouvrir le lecteur de trame de profondeur
            _Reader = _Sensor.DepthFrameSource.OpenReader();
            // Initialiser le tableau de données de profondeur à la taille appropriée
            _Data = new ushort[_Sensor.DepthFrameSource.FrameDescription.LengthInPixels];
        }
    }

    // Méthode Update appelée une fois par frame
    void Update()
    {
        // Si le lecteur de trame de profondeur est initialisé
        if (_Reader != null)
        {
            // Acquérir la dernière trame de profondeur
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                // Copier les données de la trame dans le tableau de données
                frame.CopyFrameDataToArray(_Data);
                // Libérer les ressources associées à la trame
                frame.Dispose();
                frame = null;
            }
        }
    }

    // Méthode appelée lorsque l'application se ferme
    void OnApplicationQuit()
    {
        // Libérer les ressources du lecteur de trame de profondeur
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        // Libérer les ressources du capteur Kinect
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
