using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WPM
{
    public class GeoPosAnimator : MonoBehaviour, IGeoPosAnimator
    {

        public bool Auto { get; set; }

        // Public array field with latitude/longitude positions
        public List<Vector2> LatLon;
        public IMappableObject AnimatedObject { get; set; }
        public bool Stop { get; set; } = false; 
        public bool Moving { get; set; } = false; 
        //Internal Reference Interfaces
        private IPathfinder pathfinder;
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        private IPlayerManager playerManager;
        //private IPlayerCharacter playerCharacter;
        private IGameManager gameManager;
        //Private Variables
        private float[] stepLengths;
        private int latlonIndex;
        private float totalLength;
        private float currentProgress = 0;
        //private const float MOVE_SPEED = 0.06f;  //For Build
        private const float MOVE_SPEED = 0.01f;  //For Development
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
                /*
                playerManager = gameManager.PlayerManager;
                if (playerManager == null)
                    errorHandler.ReportError("Player Manager missing", ErrorState.restart_scene);
                else
                {
                    playerCharacter = playerManager.PlayerCharacter;
                    if (playerCharacter == null)
                        errorHandler.ReportError("Player Character missing", ErrorState.restart_scene);
                }
                */
            }
        }

        public void ComputePath()
        {
            if(LatLon == null)
            {
                errorHandler.ReportError("Latlon missing", ErrorState.close_window);
                return;
            }
            // Compute path length
            int steps = LatLon.Count;
            stepLengths = new float[steps];

            // Calculate total travel length
            totalLength = 0;
            for (int k = 0; k < steps - 1; k++)
            {
                stepLengths[k] = worldMapGlobe.calc.Distance(LatLon[k], LatLon[k + 1]);
                totalLength += stepLengths[k];
            }

            Debug.Log("Total path length = " + totalLength / 1000 + " km.");
        }

        /// <summary>
        /// Moves the gameobject obj onto the globe at the path given by latlon array and progress factor.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="progress">Progress expressed in 0..1.</param>
        public void MoveTo(float progress)
        {
            currentProgress = progress;  //This seems pointless
            if(latlonIndex < 0 || (latlonIndex + 1) > LatLon.Count)
            {
                errorHandler.ReportError("Attempting to move beyond latlon range", ErrorState.close_window);
                return;
            }
            try
            {
                Vector3 pos0 = Conversion.GetSpherePointFromLatLon(LatLon[latlonIndex]);
                Vector3 pos1 = Conversion.GetSpherePointFromLatLon(LatLon[latlonIndex + 1]);
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
            if (LatLon != null)
            {
                LatLon.Clear();
            }

            foreach (var hexIndex in pathIndices)
            {
                if (hexIndex < 0 || hexIndex >= worldMapGlobe.cells.Length)
                    errorHandler.ReportError("Attempted to genrate Latlon for invalid cell", ErrorState.close_window);
                else
                    LatLon.Add(worldMapGlobe.cells[hexIndex].latlonCenter);
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
                    if(latlonIndex >= LatLon.Count)
                    {
                        errorHandler.ReportError("Attempted to move to invalid latlon", ErrorState.close_window);
                        return;
                    }
                    int newCell = worldMapGlobe.GetCellIndex(LatLon[latlonIndex]);
                    AnimatedObject.UpdateLocation(newCell);
                    currentProgress = 0;
                    if(latlonIndex >= LatLon.Count - 1  || Stop)
                    {
                        latlonIndex = 0;
                        LatLon.Clear();
                        Auto = false;
                        pathfinder.FinishedPathFinding();
                    }
                    
                }
            }

        }

    }
}