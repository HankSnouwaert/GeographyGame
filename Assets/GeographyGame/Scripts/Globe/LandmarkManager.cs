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
        private IGlobeInfo globeInfo;
        //Private Variables
        private bool started = false;
        //Path of Landmark Assets
        private string landmarkFilePath = "Prefabs/Landmarks/";
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (globeManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                globeInfo = globeManager.GlobeInfo;
                if (globeInfo == null)
                    errorHandler.ReportError("Globe Info missing", ErrorState.restart_scene);

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
            globeInfo.MappedObjects.Add(landmarkID, landmarkComponent);
            globeInfo.CulturalLandmarks.Add(landmarkID, landmarkComponent);
            globeInfo.CulturalLandmarksByName.Add(landmarkComponent.ObjectName, landmarkComponent);

            return true;
        }
    }
}


