using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SupaStuff.Unity.UI
{
    public class UIScript : MonoBehaviour
    {
        public CanvasMainScript canvas;
        public UIScript parent = null;
        public virtual void Awake()
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
