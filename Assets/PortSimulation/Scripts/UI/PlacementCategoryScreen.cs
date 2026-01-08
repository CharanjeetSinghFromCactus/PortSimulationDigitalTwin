using System;
using EnhancedUI.EnhancedScroller;
using PortSimulation.PlacementSystem;
using UISystem;
using UnityEngine;

namespace PortSimulation.UI
{
    public class PlacementCategoryScreen : UISystem.Screen,IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private PlacementCategoryCellView placementCategoryCellViewPrefab;
        [SerializeField] private PlacementDataContainer placementDataContainer;

        private void Start()
        {
            scroller.Delegate = this;
        }

        public override void Show()
        {
            scroller.ReloadData();
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return placementDataContainer.PlacementCategories.Length;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 180;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            PlacementCategoryCellView cellView = scroller.GetCellView(placementCategoryCellViewPrefab) as PlacementCategoryCellView;
            cellView.SetData(placementDataContainer.PlacementCategories[dataIndex]);
            return cellView;
        }
    }
}