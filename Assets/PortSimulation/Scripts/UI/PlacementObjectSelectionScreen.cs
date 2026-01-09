using System;
using DataBindingFramework;
using EnhancedUI.EnhancedScroller;
using PortSimulation.PlacementSystem;
using UISystem;
using Unity.Properties;
using UnityEngine;

namespace PortSimulation.UI
{
    public class PlacementObjectSelectionScreen : UISystem.Screen,IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller scroller;
        [SerializeField] private PlacementItemCellView placementCategoryCellViewPrefab;
        [SerializeField] private PlacementDataContainer placementDataContainer;
        private string selectedCategoryName;

        private IPropertyManager _propertyManager;
        private Property<string> GetCurrentCategoryName;

        private PlacementCategory _category;
        private void OnEnable()
        {
            _propertyManager = ServiceLocatorFramework.ServiceLocator.Current.Get<IPropertyManager>();
            GetCurrentCategoryName = _propertyManager.GetOrCreateProperty<string>(PropertyNameConstants.PlacementCategoryNameProperty);
            GetCurrentCategoryName.Bind(this,GetSelectedCategory);
        }

        private void OnDisable()
        {
            GetCurrentCategoryName.Unbind(GetSelectedCategory);
            _propertyManager.RemoveProperty<string>(PropertyNameConstants.PlacementCategoryNameProperty);
        }

        public override void Show()
        {
            scroller.Delegate = this;
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _category.Data.Length;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 180;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            PlacementItemCellView cellView = scroller.GetCellView(placementCategoryCellViewPrefab) as PlacementItemCellView;
            cellView.SetData(_category.Data[dataIndex]);
            return cellView;
        }

        public void OnBackButtonClick()
        {
            UISystem.ViewController.Instance.ChangeScreen(ScreenName.PlacementCategoryScreen);
        }

        private void GetSelectedCategory(string categoryName)
        {
            this.selectedCategoryName = categoryName;
            CoroutineExtension.ExecuteAfterFrame(this, () =>
            {
                _category = placementDataContainer.GetCategory(categoryName);
                scroller.ReloadData();
            });
            Debug.Log($"CategoryName in Placement Screen {categoryName}");
        }
    }
}