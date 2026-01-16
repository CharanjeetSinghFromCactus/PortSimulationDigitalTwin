using EnhancedUI.EnhancedScroller;
using PortSimulation.RoadSystem.Data;
using DataBindingFramework;
using ServiceLocatorFramework;
using PortSimulation.UI.CellViews;
using UISystem;
using UnityEngine;

namespace PortSimulation.UI
{
    public class RoadSelectionScreen : UISystem.Screen, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private RoadDataCellView roadDataCellViewPrefab;
        [SerializeField] private RoadDataContainer roadDataContainer;

        private IObserver<bool> canBuildRoadObserver;
        private IObserver<string> undoRoadObserver;
        private IObserver<bool> demolishRoadObserver;

        private bool isDemolishActive = false;

        private void Start()
        {
            var observerManager = ServiceLocatorFramework.ServiceLocator.Current.Get<DataBindingFramework.IObserverManager>();
            canBuildRoadObserver = observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.CanBuildRoad);
            undoRoadObserver = observerManager.GetOrCreateObserver<string>(ObserverNameConstants.UndoRoad);
            demolishRoadObserver = observerManager.GetOrCreateObserver<bool>(ObserverNameConstants.DemolishRoad);
            saveRoadObserver = observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SaveRoad);

        }

        private IObserver<string> saveRoadObserver;

        public void DISCLAIMED() // Placeholder to match the pattern, user will assign button event
        { }

        public void OnSaveClicked()
        {
            saveRoadObserver?.Notify("");
        }

        public void OnStopClicked()
        {
            canBuildRoadObserver?.Notify(false);
            // Also reset demolish mode on stop
            isDemolishActive = false;
            demolishRoadObserver?.Notify(false);
        }

        public void OnUndoClicked()
        {
            undoRoadObserver?.Notify("");
        }

        public void OnDemolishClicked()
        {
            isDemolishActive = !isDemolishActive;
            demolishRoadObserver?.Notify(isDemolishActive);

            // Optional: Visual change for button
        }

        public override void Show()
        {
            scroller.Delegate = this;
            base.Show();
            scroller.ReloadData();
            // canBuildRoadObserver?.Notify(true); -> Now handled by cell click
        }

        public override void Hide()
        {
            canBuildRoadObserver?.Notify(false);
            base.Hide();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return roadDataContainer != null && roadDataContainer.Roads != null ? roadDataContainer.Roads.Length : 0;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 100f; // Adjust size as needed
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            RoadDataCellView cellView = scroller.GetCellView(roadDataCellViewPrefab) as RoadDataCellView;
            cellView.SetData(roadDataContainer.Roads[dataIndex]);
            return cellView;
        }

        public void OnBackButtonClick()
        {
            OnStopClicked();
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.HomeScreen); // Example fallback
        }

    }
}
