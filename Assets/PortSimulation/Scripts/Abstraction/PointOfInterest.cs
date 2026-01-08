using System;
using UnityEngine;

namespace PortSimulation
{
    public abstract class PointOfInterest : MonoBehaviour,IClickable,IFocusable
    {
        public virtual void Start()
        {
            ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().Register(this);
        }

        private void OnDisable()
        {
            ServiceLocatorFramework.ServiceLocator.Current.Get<POIManager>().Unregister(this);
        }

        public abstract void OnClick();
        public abstract void OnFocus();
        public abstract void OnHide();
    }
}
