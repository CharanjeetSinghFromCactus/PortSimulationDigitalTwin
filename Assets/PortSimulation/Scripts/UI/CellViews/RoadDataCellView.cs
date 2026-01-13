using DataBindingFramework;
using EnhancedUI.EnhancedScroller;
using PortSimulation.RoadSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PortSimulation.UI.CellViews
{
    public class RoadDataCellView : EnhancedScrollerCellView
    {
        private RoadData _roadData;
        [SerializeField] private Image roadIcon;
        [SerializeField] private TMP_Text roadNameText;
        
        private IObserverManager observerManager;
        private IObserver<string> setRoadTypeObserver;

        private void OnEnable()
        {
            observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IObserverManager>();
            setRoadTypeObserver = observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SetRoadType);
        }

        private void OnDisable()
        {
            // No strict need to unbind/remove here if we only Notify, calling GetOrCreate is safe.
            // But good practice generally. However, we are only notifying, not observing ourselves.
        }

        public void SetData(RoadData item)
        {
            _roadData = item;
            if (roadIcon != null) roadIcon.sprite = item.RoadIcon;
            if (roadNameText != null) roadNameText.text = item.RoadName;
        }

        public void OnClick()
        {
            if (_roadData != null)
            {
                // We send the RoadName string, effectively selecting it.
                // RoadManager listens to this and picks the data from container.
                setRoadTypeObserver.Notify(_roadData.RoadName);
            }
        }
    }
}
