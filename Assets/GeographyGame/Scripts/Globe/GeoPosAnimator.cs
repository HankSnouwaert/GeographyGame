using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WPM
{
    public class GeoPosAnimator : MonoBehaviour, IGeoPosAnimator
    {

        [SerializeField]
        private AudioSource movementAudio;

        //Public Variables
        public bool Auto { get; set; }
        public IMappableObject AnimatedObject { get; set; }
        public bool Stop { get; set; } = false; 
        public bool Moving { get; set; } = false; 
        //Internal Reference Interfaces
        private IPathfinder pathfinder;
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        private IPlayerManager playerManager;
        private IGameManager gameManager;
        //Private Variables
        private float[] stepLengths;
        private int latlonIndex;
        private float totalLength;
        private float currentProgress = 0;
        private List<Vector2> latLon = new List<Vector2>(); // Array field with latitude/longitude positions
        private const float MOVE_SPEED = 0.06f;  //For Build
        //private const float MOVE_SPEED = 0.01f;  //For Development
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                AnimatedObject = GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
                if (AnimatedObject == null)
                    componentMissing = true;
                pathfinder = GetComponent(typeof(IPathfinder)) as IPathfinder;
                if (pathfinder == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        private void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            globeManager = interfaceFactory.GlobeManager;
            if (errorHandler == null || gameManager == null || globeManager == null)
                gameObject.SetActive(false);
            else
            {
                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
            }
        }

        public void Update()
        {
            if (Auto)
            {
                MoveTo(currentProgress);
                currentProgress += MOVE_SPEED;
                if (currentProgress > 1f)
                {
                    latlonIndex++;
                    if (latlonIndex >= latLon.Count || latlonIndex < 0)
                    {
                        errorHandler.ReportError("Attempted to move to invalid latlon", ErrorState.close_window);
                        return;
                    }
                    int newCell = worldMapGlobe.GetCellIndex(latLon[latlonIndex]);
                    AnimatedObject.UpdateLocation(newCell);
                    currentProgress = 0;
                    if (latlonIndex >= latLon.Count - 1 || Stop)
                        StopMovement();
                }
            }
        }

        private void StopMovement()
        {
            latlonIndex = 0;
            latLon.Clear();
            Auto = false;
            movementAudio.Pause();
            pathfinder.FinishedPathFinding();
        }

        public void InitiateMovement(List<int> pathIndices)
        {
            //Add latlon of each hex in path to animator's path
            GenerateLatLon(pathIndices);
            // Compute path length
            ComputePath();
            Auto = true;
            Moving = true;
            movementAudio.Play();
        }

        public void ComputePath()
        {
            if(latLon == null)
            {
                errorHandler.ReportError("Latlon missing", ErrorState.close_window);
                return;
            }
            try
            {
                // Compute path length
                int steps = latLon.Count;
                stepLengths = new float[steps];

                // Calculate total travel length
                totalLength = 0;
                for (int k = 0; k < steps - 1; k++)
                {
                    stepLengths[k] = worldMapGlobe.calc.Distance(latLon[k], latLon[k + 1]);
                    totalLength += stepLengths[k];
                }

                Debug.Log("Total path length = " + totalLength / 1000 + " km.");
            }
            catch (System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
            }
        }

        /// <summary>
        /// Moves the gameobject obj onto the globe at the path given by latlon array and progress factor.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="progress">Progress expressed in 0..1.</param>
        public void MoveTo(float progress)
        {
            currentProgress = progress;  //This seems pointless
            if(latlonIndex < 0 || (latlonIndex + 1) > latLon.Count)
            {
                errorHandler.ReportError("Attempting to move beyond latlon range", ErrorState.close_window);
                return;
            }
            try
            {
                Vector3 pos0 = Conversion.GetSpherePointFromLatLon(latLon[latlonIndex]);
                Vector3 pos1 = Conversion.GetSpherePointFromLatLon(latLon[latlonIndex + 1]);
                Vector3 pos = Vector3.Lerp(pos0, pos1, progress);
                pos = pos.normalized * 0.5f;
                float playerSize = AnimatedObject.Size;
                worldMapGlobe.AddMarker(gameObject, pos, playerSize, false);

                // Make it look towards destination
                Vector3 dir = (pos0 - pos1).normalized;
                Vector3 proj = Vector3.ProjectOnPlane(dir, pos0);
                transform.LookAt(worldMapGlobe.transform.TransformPoint(proj + pos0), worldMapGlobe.transform.transform.TransformDirection(pos0));
            }
            catch(System.Exception ex)
            {
                errorHandler.CatchException(ex, ErrorState.close_window);
            }
        }

        public void GenerateLatLon(List<int> pathIndices)
        {
            latLon.Clear();
            foreach (var hexIndex in pathIndices)
            {
                if (hexIndex < 0 || hexIndex >= worldMapGlobe.cells.Length)
                    errorHandler.ReportError("Attempted to genrate Latlon for invalid cell", ErrorState.close_window);
                else
                    latLon.Add(worldMapGlobe.cells[hexIndex].latlonCenter);
            }
        }
    }
}