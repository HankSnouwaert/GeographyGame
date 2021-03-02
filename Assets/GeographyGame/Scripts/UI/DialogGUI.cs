using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WPM
{

    public class DialogGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ": I was entered!");
            gameManager.CursorOverUI = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ": I was exited!");
            gameManager.CursorOverUI = false;

        }
    }
}