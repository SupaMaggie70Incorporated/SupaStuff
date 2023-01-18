using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SupaStuff.Unity.UI
{
    public class CanvasMainScript : MonoBehaviour
    {
        public UIScript CurrentScreen = null;
        // Start is called before the first frame update
        void Awake()
        {
            SwitchToScreen(CurrentScreen, true);
        }

        // Update is called once per frame
        void Update()
        {
            if(CurrentScreen.CloseWithEscape && Input.GetKeyDown(KeyCode.Escape)) CurrentScreen.ExitToParent();
        }
        public void SwitchToScreen(UIScript obj, bool shouldSetParent)
        {
            if (CurrentScreen != null) CurrentScreen.gameObject.SetActive(false);

            if(obj != null)
            {
                if (shouldSetParent) obj.parent = CurrentScreen;
                obj.gameObject.SetActive(true);
            }

            CurrentScreen = obj;

        }
    }
}
