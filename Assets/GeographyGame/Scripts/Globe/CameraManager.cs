using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{

    public class CameraManager : MonoBehaviour, ICameraManager
    {
        private GameManager gameManager;
        private GlobeManager globeManager;
        private WorldMapGlobe worldGlobeMap;
        private IErrorHandler errorHandler;

        // Start is called before the first frame update
        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            globeManager = FindObjectOfType<GlobeManager>();
            worldGlobeMap = globeManager.worldGlobeMap;
            errorHandler = gameManager.ErrorHandler;
        }

        // Update is called once per frame
        private void Update()
        {
            //Check for camera movement
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                worldGlobeMap.DragTowards(Vector2.up);
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                worldGlobeMap.DragTowards(Vector2.down);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                worldGlobeMap.DragTowards(Vector2.left);
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                worldGlobeMap.DragTowards(Vector2.right);
        }

        public void OrientOnLocation(Vector3 vectorLocation)
        {
            worldGlobeMap.FlyToLocation(vectorLocation, 1.5F, 0.05F, 0.01F, 0);
            worldGlobeMap.pitch = 0;
            worldGlobeMap.yaw = 0;
        }
    }
}
