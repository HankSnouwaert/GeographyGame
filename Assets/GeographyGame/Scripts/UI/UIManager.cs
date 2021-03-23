using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WPM
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [SerializeField] private GameObject errorPanelObject;
        public IErrorUI ErrorUI { get; set; }
        public bool CursorOverUI { get; set; }

        // Start is called before the first frame update
        void Awake()
        {
            ErrorUI = errorPanelObject.GetComponent(typeof(IErrorUI)) as IErrorUI;
        }

        void Update()
        {
            CursorOverUI = CheckForMouseOverPanel();
        }

        public bool CheckForMouseOverPanel()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
            for (int i = 0; i < raycastResultList.Count; i++)
            {
                if (raycastResultList[i].gameObject.GetComponent<IUIElement>() == null)
                {
                    raycastResultList.RemoveAt(i);
                    i--;
                }
            }
            return raycastResultList.Count > 0;
        }

    }
}
