using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class PlayerManager : MonoBehaviour, IPlayerManager
    {
        [SerializeField]
        private GameObject playerPrefab;

        public IPlayerCharacter PlayerCharacter { get; protected set; }
        //Local Interface References
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
            if (playerPrefab == null)
                componentMissing = true;
        }

        void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            if (globeManager == null || errorHandler == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (componentMissing)
                    errorHandler.ReportError("PlayerManager component missing", ErrorState.restart_scene);

                worldMapGlobe = globeManager.WorldMapGlobe;
                if(worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
            }
        }

        public bool IntantiatePlayer(Cell startCell)
        {
            GameObject playerObject = Instantiate(playerPrefab);
            if(playerObject == null)
            {
                errorHandler.ReportError("Player instantiation failed", ErrorState.restart_scene);
                return false;
            }
            PlayerCharacter = playerObject.GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
            if(PlayerCharacter == null)
            {
                errorHandler.ReportError("Player component missing", ErrorState.close_application);
                return false;
            }
            try
            {
                PlayerCharacter.CellLocation = startCell;
                PlayerCharacter.Latlon = startCell.latlon;
                Vector3 startingLocation = startCell.sphereCenter;
                PlayerCharacter.VectorLocation = startingLocation;
                float playerSize = PlayerCharacter.GetSize();
                worldMapGlobe.AddMarker(playerObject, startingLocation, playerSize, false, 0.0f, true, true);
                startCell.occupants.Add(PlayerCharacter);
                return true;
            }
            catch
            {
                errorHandler.ReportError("Invalid start cell for player", ErrorState.restart_scene);
                return false;
            }
        }
    }
}


