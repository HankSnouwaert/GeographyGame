using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class CameraManager : MonoBehaviour, ICameraManager
    {
        [SerializeField]
        private GameObject cameraTutorialObject;
        //Public Variables
        public GameObject CameraGameObject { get; protected set; }
        //FIX: This may need to be in a shared header file
        public const int None = 0;
        public const int OrientOnLocationTutorial = 1;
        public int ActiveTutorial { get; set; } = 0; 
        //Local Interface References
        private IGameManager gameManager;
        private IGlobeManager globeManager;
        private ICameraTutorial cameraTutorial;
        //World Map Globe does not use an interface
        private WorldMapGlobe worldMapGlobe;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;
        private GameSettings gameSettings;

        // Start is called before the first frame update
        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameSettings = FindObjectOfType<GameSettings>();
            if (interfaceFactory == null || gameSettings == null)
                gameObject.SetActive(false);
            try
            {
                cameraTutorial = cameraTutorialObject.GetComponent(typeof(ICameraTutorial)) as ICameraTutorial;
                if (cameraTutorial == null)
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
            gameManager = interfaceFactory.GameManager;
            if (globeManager == null || errorHandler == null || gameManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (componentMissing)
                {
                    errorHandler.ReportError("Camera Manager component missing", ErrorState.restart_scene);
                    return;
                }

                worldMapGlobe = globeManager.WorldMapGlobe;
                if (worldMapGlobe == null)
                    errorHandler.ReportError("World Map Globe missing", ErrorState.restart_scene);
                else
                    if (gameSettings.TutorialActive == true)
                        LockCamera();
            }
        }

        private void Update()
        {
            //Check for camera movement
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                worldMapGlobe.DragTowards(Vector2.up);
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                worldMapGlobe.DragTowards(Vector2.down);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                worldMapGlobe.DragTowards(Vector2.left);
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                worldMapGlobe.DragTowards(Vector2.right);
        }

        public void OrientOnLocation(Vector3 vectorLocation)
        {
            worldMapGlobe.FlyToLocation(vectorLocation, 1.5F, 0.05F, 0.01F, 0);
            worldMapGlobe.pitch = 0;
            worldMapGlobe.yaw = 0;
            if (ActiveTutorial == 1)
            {
                cameraTutorial.TutorialActionComplete = true;
                ActiveTutorial = 0;
            }
        }

        public void LockCamera()
        {
            worldMapGlobe.allowUserKeys = false;
            worldMapGlobe.allowUserRotation = false;
            worldMapGlobe.allowUserZoom = false;
        }

        public void UnlockCamera()
        {
            worldMapGlobe.allowUserKeys = true;
            worldMapGlobe.allowUserRotation = true;
            worldMapGlobe.allowUserZoom = true;
        }
    }
}
