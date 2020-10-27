﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class InventoryItem : SelectableObject
    {
        public Sprite inventoryIcon;
        public int inventorySpace = 1;
        public int inventoryLocation;
        protected InventoryGUI inventoryGUI;

        public override void Start()
        {
            base.Start();
            inventoryGUI = FindObjectOfType<InventoryGUI>();
        }
    }
}
