using UnityEngine;
using System.Text;
using System.Collections;
using System.IO;
using SpeedTutorMainMenuSystem;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Components")]
        public WorldMapGlobe worldGlobeMap; 
        WorldMapGlobe map;

        void Start()
        {
            Debug.Log("Globe Loaded");
            ApplyGlobeSettings();
        }

        void ApplyGlobeSettings()
        {
            Debug.Log("Applying Globe Settings");
            if (File.Exists(Application.dataPath + "/student.txt"))
            {
                string savedMapSettings = File.ReadAllText(Application.dataPath + "/student.txt");
                SaveObject loadedMapSettings = JsonUtility.FromJson<SaveObject>(savedMapSettings);
                worldGlobeMap.showFrontiers = loadedMapSettings.climate;
            }
        }

        private void Update()
        {
            bool debug = worldGlobeMap.showFrontiers;
        }
    }
}
