using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class InterfaceFactory : MonoBehaviour
    {
        public GameObject gameManagerObject;
        //public IGameManager GameManager { get; set; }
        public GameObject uiManagerObject;
        public IUIManager UIManager { get; set; }
        public GameObject errorHandlerObject;
        public IErrorHandler ErrorHandler { get; set; }
        public GameObject globeManagerObject;
        public IGlobeManager GlobeManager { get; set; }
        
        void Awake()
        {
            //GameManager = gameManagerObject.GetComponent(typeof(IGameManager)) as IGameManager;
            UIManager = uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            ErrorHandler = errorHandlerObject.GetComponent(typeof(IErrorHandler)) as IErrorHandler;
            GlobeManager = globeManagerObject.GetComponent(typeof(IGlobeManager)) as IGlobeManager;
        }
    }
}
