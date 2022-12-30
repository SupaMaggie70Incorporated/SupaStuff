using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SupaStuff.Unity.UI
{
    public class UIScript : MonoBehaviour
    {
        [NonSerialized]
        public CanvasMainScript canvas;
        [NonSerialized]
        public UIScript parent = null;
        public bool CloseWithEscape = false;
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
