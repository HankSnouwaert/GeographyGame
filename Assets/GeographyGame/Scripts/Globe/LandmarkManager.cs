using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class LandmarkManager : MonoBehaviour, ILandmarkManager
    {
        //Internal Interface References
        private WorldMapGlobe worldMapGlobe;
        private IGlobeManager globeManager;
        private IMappablesManager mappablesManager;
        //Private Variables
        private bool started = false;
        //Public Variables
        public Dictionary<string, Landmark> CulturalLandmarks { get; set; } = new Dictionary<string, Landmark>();
        public Dictionary<string, Landmark> CulturalLandmarksByName { get; set; } = new Dictionary<string, Landmark>();
        //Path of Landmark Assets
        private string landmarkFilePath = "Prefabs/Landmarks/";
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
                mappablesManager = GetComponent(typeof(IMappablesManager)) as IMappablesManager;
                if (mappablesManager == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        private void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Landmark Manager missing component", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                started = true;
            }
        }

        public bool InstantiateCulturalLandmark(MountPoint mountPoint)
        {
            if (!started)
                Start();

            string mountPointName = mountPoint.name;
            if (mountPointName == null)
            {
                errorHandler.ReportError("Cultural mount point name missing", ErrorState.close_window);
                return false;
            }

            string tempName = mountPointName.Replace("The", "");
            tempName = tempName.Replace(" ", "");
            var model = Resources.Load<GameObject>(landmarkFilePath + tempName);
            if (model == null)
            {
                errorHandler.ReportError("Unable to load cultural landmark model", ErrorState.close_window);
                return false;
            }

            var modelClone = Instantiate(model);
            if (modelClone == null)
            {
                errorHandler.ReportError("Failed to instantiate " + mountPointName, ErrorState.close_window);
                return false;
            }
                
            Landmark landmarkComponent = modelClone.GetComponent(typeof(Landmark)) as Landmark;
            if (landmarkComponent == null)
            {
                errorHandler.ReportError(mountPointName + " landmark component missing", ErrorState.close_window);
                return false;
            } 

            landmarkComponent.MountPoint = mountPoint;
            landmarkComponent.ObjectName = mountPointName;
            landmarkComponent.CellIndex = worldMapGlobe.GetCellIndex(mountPoint.localPosition);
            if (landmarkComponent.CellIndex < 0 || landmarkComponent.CellIndex > worldMapGlobe.cells.Length)
            {
                errorHandler.ReportError("Invalid cell index for " + mountPointName, ErrorState.close_window);
                return false;
            } 
            
            landmarkComponent.CellLocation = worldMapGlobe.cells[landmarkComponent.CellIndex];
            landmarkComponent.CellLocation.canCross = false;
            worldMapGlobe.AddMarker(modelClone, mountPoint.localPosition, 0.001f, false, -5.0f, true, true);
            landmarkComponent.CellLocation.occupants.Add(landmarkComponent);
            string landmarkID = landmarkComponent.GetInstanceID().ToString();
            mappablesManager.MappedObjects.Add(landmarkID, landmarkComponent);
            CulturalLandmarks.Add(landmarkID, landmarkComponent);
            CulturalLandmarksByName.Add(landmarkComponent.ObjectName, landmarkComponent);

            return true;
        }
    }
}


