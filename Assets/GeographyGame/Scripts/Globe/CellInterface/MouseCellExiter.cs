using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to handle instances of the mouse moving over a cell on the world globe map
    /// </summary>
    public class MouseCellExiter : MonoBehaviour, ICellExiter
    {
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
            errorHandler = interfaceFactory.ErrorHandler;
            if (errorHandler == null)
                gameObject.SetActive(false);
        }

        /// <summary>
        ///  Called when the mouse cursor moves out of a cell on the world globe map
        /// </summary>
        /// <param name="cellIndex"></param> The cell the mouse cursor out of over>
        /// <returns></returns> 
        public void HandleOnCellExit(int cellIndex)
        {
            //Currently, there's nothing here that needs to be done
        }
    }
}

