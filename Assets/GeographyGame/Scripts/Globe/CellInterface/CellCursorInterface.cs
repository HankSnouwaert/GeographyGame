using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to hold interfaces related to cell interaction
    /// </summary>
    public class CellCursorInterface : MonoBehaviour, ICellCursorInterface
    {
        public ICellClicker CellClicker { get; set; } 
        public ICellEnterer CellEnterer { get; set; }
        public ICellExiter CellExiter { get; set; } 
        public int highlightedCellIndex { get; set; }
        public Cell highlightedCell { get; set; }
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
                CellClicker = GetComponent(typeof(ICellClicker)) as ICellClicker;
                CellEnterer = GetComponent(typeof(ICellEnterer)) as ICellEnterer;
                CellExiter = GetComponent(typeof(ICellExiter)) as ICellExiter;
                if (CellClicker == null || CellEnterer == null || CellExiter == null)
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
            if (errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                if (componentMissing == true)
                    errorHandler.ReportError("Cell Cursor Interface component missing", ErrorState.restart_scene);
            }
        }
    }
}
