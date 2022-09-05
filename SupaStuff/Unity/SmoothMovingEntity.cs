using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupaStuff.Unity
{
    [UnitySpecific]
    public abstract class SmoothMovingEntity : MonoBehaviour
    {
        Vector3 currentPos;
        public Vector3 NextPosition;
        public Vector3 CurrentPosition;
        public Vector3 PreviousPosition;
        Vector3 currentRot;

        public Vector3 NextRotation;
        public Vector3 CurrentRotation;
        public Vector3 PreviousRotation;
        public Texture2D Icon;
        public float lastFixedUpdateTime = 0;
        public Vector3 RenderPosition
        {
            get
            {
                float amount = (Time.time - lastFixedUpdateTime) / Time.fixedDeltaTime;
                if (amount > 1) amount = 1;
                return Vector3.Lerp(PreviousPosition, CurrentPosition, amount);
            }
        }
        public Quaternion RenderRotation
        {
            get
            {
                float amount = (Time.time - lastFixedUpdateTime) / Time.fixedDeltaTime;
                if (amount > 1) amount = 1;
                return Quaternion.Euler(Vector3.Lerp(PreviousPosition, CurrentPosition, amount));
            }
        }

        public void Delete()
        {
            Destroy(gameObject);
        }
        public virtual Vector3 GetLocation()
        {
            return CurrentPosition;
        }
        public virtual void SetLocation(Vector3 pos)
        {
            CurrentPosition = pos;
        }
        public virtual void Update()
        {
            transform.position = RenderPosition;
            transform.rotation = RenderRotation;
        }
        public virtual void OnEnable()
        {

        }
        public virtual void OnDisable()
        {

        }
        public virtual void Start()
        {
        }
        public virtual void FixedUpdate()
        {
            //CurrentPosition = 
            lastFixedUpdateTime = Time.time;
            PreviousPosition = CurrentPosition;
            CurrentPosition = NextPosition;
            PreviousRotation = CurrentRotation;
            CurrentRotation = NextRotation;
            TickMovement();
        }
        public virtual void TickMovement()
        {

        }
    }
}