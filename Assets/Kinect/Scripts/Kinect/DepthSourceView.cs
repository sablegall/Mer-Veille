using UnityEngine;
using System.Collections;
using Windows.Kinect;

public enum DepthViewMode
{
    SeparateSourceReaders,  // Mode utilisant des lecteurs de sources séparés
    MultiSourceReader,      // Mode utilisant un lecteur de sources multiples
}

public class DepthSourceView : MonoBehaviour
{
    public DepthViewMode ViewMode = DepthViewMode.SeparateSourceReaders;  // Mode de vue initial

    public GameObject ColorSourceManager;  // Référence au gestionnaire de la source de couleur
    public GameObject DepthSourceManager;  // Référence au gestionnaire de la source de profondeur
    public GameObject MultiSourceManager;  // Référence au gestionnaire de la source multiple

    private KinectSensor _Sensor;          // Capteur Kinect
    private CoordinateMapper _Mapper;      // Mapper de coordonnées Kinect
    private Mesh _Mesh;                    // Mesh pour l'affichage
    private Vector3[] _Vertices;           // Tableau des sommets du mesh
    private Vector2[] _UV;                 // Tableau des coordonnées UV du mesh
    private int[] _Triangles;              // Tableau des indices des triangles du mesh

    private const int _DownsampleSize = 4; // Facteur de réduction de la résolution
    private const double _DepthScale = 0.1f; // Échelle de profondeur
    private const int _Speed = 50;         // Vitesse de rotation

    private MultiSourceManager _MultiManager; // Gestionnaire de source multiple
    private ColorSourceManager _ColorManager; // Gestionnaire de source de couleur
    private DepthSourceManager _DepthManager; // Gestionnaire de source de profondeur

    void Start()
    {
        // Initialisation du capteur Kinect
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Mapper = _Sensor.CoordinateMapper; // Obtention du mapper de coordonnées
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription; // Description du cadre de profondeur

            // Création du mesh avec une résolution réduite
            CreateMesh(frameDesc.Width / _DownsampleSize, frameDesc.Height / _DownsampleSize);

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open(); // Ouverture du capteur si nécessaire
            }
        }
    }

    void CreateMesh(int width, int height)
    {
        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh; // Attacher le mesh au MeshFilter

        _Vertices = new Vector3[width * height]; // Initialiser les sommets
        _UV = new Vector2[width * height];       // Initialiser les coordonnées UV
        _Triangles = new int[6 * ((width - 1) * (height - 1))]; // Initialiser les triangles

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

                _Vertices[index] = new Vector3(x, -y, 0); // Positionner les sommets
                _UV[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height)); // Positionner les UV

                // Création des triangles sauf pour la dernière rangée/colonne
                if (x != (width - 1) && y != (height - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals(); // Recalculer les normales du mesh
    }

    void OnGUI()
    {
        // Afficher le mode de vue actuel sur l'écran
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        GUI.TextField(new Rect(Screen.width - 250, 10, 250, 20), "DepthMode: " + ViewMode.ToString());
        GUI.EndGroup();
    }

    void Update()
    {
        if (_Sensor == null)
        {
            return;
        }

        // Changer le mode de vue avec le bouton Fire1
        if (Input.GetButtonDown("Fire1"))
        {
            if (ViewMode == DepthViewMode.MultiSourceReader)
            {
                ViewMode = DepthViewMode.SeparateSourceReaders;
            }
            else
            {
                ViewMode = DepthViewMode.MultiSourceReader;
            }
        }

        // Rotation de l'objet
        float yVal = Input.GetAxis("Horizontal");
        float xVal = -Input.GetAxis("Vertical");

        transform.Rotate(
            (xVal * Time.deltaTime * _Speed),
            (yVal * Time.deltaTime * _Speed),
            0,
            Space.Self);

        // Gestion des différents modes de vue
        if (ViewMode == DepthViewMode.SeparateSourceReaders)
        {
            if (ColorSourceManager == null)
            {
                return;
            }

            _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
            if (_ColorManager == null)
            {
                return;
            }

            if (DepthSourceManager == null)
            {
                return;
            }

            _DepthManager = DepthSourceManager.GetComponent<DepthSourceManager>();
            if (_DepthManager == null)
            {
                return;
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();
            RefreshData(_DepthManager.GetData(),
                _ColorManager.ColorWidth,
                _ColorManager.ColorHeight);
        }
        else
        {
            if (MultiSourceManager == null)
            {
                return;
            }

            _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
            if (_MultiManager == null)
            {
                return;
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = _MultiManager.GetColorTexture();

            RefreshData(_MultiManager.GetDepthData(),
                        _MultiManager.ColorWidth,
                        _MultiManager.ColorHeight);
        }
    }

    private void RefreshData(ushort[] depthData, int colorWidth, int colorHeight)
    {
        var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        _Mapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        for (int y = 0; y < frameDesc.Height; y += _DownsampleSize)
        {
            for (int x = 0; x < frameDesc.Width; x += _DownsampleSize)
            {
                int indexX = x / _DownsampleSize;
                int indexY = y / _DownsampleSize;
                int smallIndex = (indexY * (frameDesc.Width / _DownsampleSize)) + indexX;

                double avg = GetAvg(depthData, x, y, frameDesc.Width, frameDesc.Height);

                avg = avg * _DepthScale;

                _Vertices[smallIndex].z = (float)avg;

                // Mettre à jour le mapping UV avec les coordonnées de l'espace couleur
                var colorSpacePoint = colorSpace[(y * frameDesc.Width) + x];
                _UV[smallIndex] = new Vector2(colorSpacePoint.X / colorWidth, colorSpacePoint.Y / colorHeight);
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals(); // Recalculer les normales du mesh
    }

    private double GetAvg(ushort[] depthData, int x, int y, int width, int height)
    {
        double sum = 0.0;

        // Calcul de la moyenne des valeurs de profondeur dans une zone de 4x4 pixels
        for (int y1 = y; y1 < y + 4; y1++)
        {
            for (int x1 = x; x1 < x + 4; x1++)
            {
                int fullIndex = (y1 * width) + x1;

                if (depthData[fullIndex] == 0)
                    sum += 4500; // Valeur de profondeur par défaut pour les pixels sans données
                else
                    sum += depthData[fullIndex];
            }
        }

        return sum / 16; // Retourner la moyenne des valeurs de profondeur
    }

    void OnApplicationQuit()
    {
        if (_Mapper != null)
        {
            _Mapper = null; // Libérer le mapper de coordonnées
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close(); // Fermer le capteur Kinect si nécessaire
            }

            _Sensor = null; // Libérer le capteur Kinect
        }
    }
}
