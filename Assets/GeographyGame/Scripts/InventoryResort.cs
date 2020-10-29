using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{

    public class InventoryResort : InventoryItem
    {

        public override void Selected()
        {
            base.Selected();
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

        public override void Deselected()
        {
            base.Deselected();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public override void OnCellClick(int index)
        {
            if(gameManager.worldGlobeMap.cells[index].tag == null)
            {
                //Create Resort
                var resortPrefab = Resources.Load<GameObject>("Prefabs/Selectables/Resort");
                var resortObject = Instantiate(resortPrefab);

                Resort resortComponent = resortObject.GetComponent(typeof(Resort)) as Resort;
                gameManager.worldGlobeMap.AddMarker(resortObject, gameManager.worldGlobeMap.cells[index].sphereCenter, 0.01f, false, 0.0f, true, true);

                Deselected();

                //Remove Resort from Inventory
                inventoryGUI.RemoveItem(inventoryLocation);
            }
            else
            {
                //Cell Occupied
                Deselected();
            }
        }
    }
}
