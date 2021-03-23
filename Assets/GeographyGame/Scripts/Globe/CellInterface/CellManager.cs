using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Used to hold interfaces related to cell interaction
    /// </summary>
    public class CellManager : MonoBehaviour, ICellManager
    {
        public ICellClicker CellClicker { get; set; } 
        public ICellEnterer CellEnterer { get; set; }
        public ICellExiter CellExiter { get; set; } 

        void Awake()
        {
            CellClicker = GetComponent(typeof(ICellClicker)) as ICellClicker;
            CellEnterer = GetComponent(typeof(ICellEnterer)) as ICellEnterer;
            CellExiter = GetComponent(typeof(ICellExiter)) as ICellExiter;
        }
    }
}
