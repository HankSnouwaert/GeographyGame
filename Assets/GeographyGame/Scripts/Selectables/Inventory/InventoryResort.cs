using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{

    public class InventoryResort : InventoryItem
    {
        private IGlobeManager globeManager;
        private WorldMapGlobe worldMapGlobe;
        public override void Start()
        {
            base.Start();
            globeManager = FindObjectOfType<InterfaceFactory>().GlobeManager;
            worldMapGlobe = globeManager.WorldMapGlobe;
        }
        public override void Select()
        {
            base.Select();
            int debug = inventoryLocation;
            /*
            var croppedTexture = new Texture2D((int)inventoryIcon.rect.width, (int)inventoryIcon.rect.height);
            var pixels = inventoryIcon.texture.GetPixels((int)inventoryIcon.textureRect.x,
                                                    (int)inventoryIcon.textureRect.y,
                                                    (int)inventoryIcon.textureRect.width,
                                                    (int)inventoryIcon.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            Cursor.SetCursor(croppedTexture, Vector2.zero, CursorMode.ForceSoftware);
            */
        }

        public override void Deselect()
        {
            base.Deselect();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public override void OnCellClick(int index)
        {
            if(worldMapGlobe.cells[index].tag == null)
            {
                //Create Resort
                var resortPrefab = Resources.Load<GameObject>("Prefabs/Selectables/Resort");
                var resortObject = Instantiate(resortPrefab);

                Resort resortComponent = resortObject.GetComponent(typeof(Resort)) as Resort;
                worldMapGlobe.AddMarker(resortObject, worldMapGlobe.cells[index].sphereCenter, 0.01f, false, 0.0f, true, true);

                Deselect();

                //Remove Resort from Inventory
                inventoryUI.RemoveItem(inventoryLocation);
            }
            else
            {
                //Cell Occupied
                Deselect();
            }
        }
    }
}
