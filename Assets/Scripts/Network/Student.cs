using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

namespace Mirror.Examples.Basic
{
    public class Student : NetworkBehaviour
    {
        [Header("Player Components")]
        public RectTransform rectTransform;
        public Image image;

        [Header("Child Text Objects")]
        public Text playerNameText;
        public Text playerDataText;
    
        [ClientRpc]
        public void RpcCreateWorld(string mapSettings)
        {
            if (isLocalPlayer)
            {
                File.WriteAllText(Application.dataPath + "/student.txt", mapSettings);
                SceneManager.LoadScene("StudentGame", LoadSceneMode.Single);
                
                //Use this code if I want scene to load in background
                //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StudentGame", LoadSceneMode.Additive);
                //SceneManager.SetActiveScene(SceneManager.GetSceneByName("StudentGame"));
            }
        }
    }
}
