using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SupaStuff.Unity.UI
{
    public class UIScript : MonoBehaviour
    {
        public CanvasMainScript canvas;
        internal UIScript parent = null;
        public void Awake()
        {
            canvas = transform.parent.GetComponent<CanvasMainScript>();
            if (canvas.CurrentScreen != this) gameObject.SetActive(false);
        }
        public void ExitToParent()
        {
            canvas.SwitchToScreen(parent, false);
        }
    }
}
