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
        void Start()
        {
            SwitchToScreen(CurrentScreen, true);
        }

        // Update is called once per frame
        void Update()
        {

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
