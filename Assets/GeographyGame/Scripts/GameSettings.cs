using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class GameSettings : MonoBehaviour, IGameSettings
    {
        public bool TutorialActive { get; set; } = false;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}


