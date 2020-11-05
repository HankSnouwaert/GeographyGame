using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{
    public class InventoryTourist : InventoryItem
    {
        private GameObject dialogPanel;
        private Text dialog;
        private string destinationName;
        private int destinationIndex;
        private Province[] possibleDestinations; 

        public virtual void Start()
        {
            base.Start();
            dialogPanel = gameManager.dialogPanel;
            Transform textObject = dialogPanel.transform.GetChild(0);
            dialog = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            //Get Random Tourist Destination
            int coinFlip = Random.Range(0, 2);
            if (coinFlip == 0)
            {
                //Province
                int countryIndex = gameManager.worldGlobeMap.GetCountryIndex("United States of America");
                possibleDestinations = gameManager.worldGlobeMap.countries[countryIndex].provinces;
                destinationIndex = Random.Range(0, possibleDestinations.Length);
                destinationName = possibleDestinations[destinationIndex].name;
            }
            else
            {
                //Landmark
                int landmarkIndex = Random.Range(0, gameManager.culturalLandmarks.Count);
                Landmark landmarkDestination = gameManager.culturalLandmarks[landmarkIndex];
                destinationName = landmarkDestination.name;
            }
        }

        public override void Selected()
        {
            base.Selected();
            int debug = inventoryLocation;
            dialogPanel.SetActive(true);
            dialog.text = "I want to go to " + destinationName;
        }

        public override void Deselected()
        {
            base.Deselected();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            dialogPanel.SetActive(false);
        }
    }
}
