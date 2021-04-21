using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class PlayerManager : MonoBehaviour, IPlayerManager
    {
        [SerializeField]
        private GameObject playerPrefab;

        public IPlayerCharacter Player { get; protected set; }
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
            Player = playerObject.GetComponent(typeof(IPlayerCharacter)) as IPlayerCharacter;
            if(Player == null)
            {
                errorHandler.ReportError("Player component missing", ErrorState.close_application);
                return false;
            }
            try
            {
                Player.CellLocation = startCell.index;
                Player.Latlon = startCell.latlon;
                Vector3 startingLocation = startCell.sphereCenter;
                Player.VectorLocation = startingLocation;
                float playerSize = Player.GetSize();
                worldMapGlobe.AddMarker(playerObject, startingLocation, playerSize, false, 0.0f, true, true);
                startCell.occupants.Add(Player);
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


