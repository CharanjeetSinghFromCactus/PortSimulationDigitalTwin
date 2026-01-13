using EnhancedUI.EnhancedScroller;
using PortSimulation.RoadSystem.Data;
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

        public override void Show()
        {
            scroller.Delegate = this;
            base.Show();
            scroller.ReloadData();
        }

        public override void Hide()
        {
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
             UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementCategoryScreen); // Example fallback
        }
    }
}
