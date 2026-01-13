using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace PortSimulation.UI
{
    public class HomeScreen : UISystem.Screen
    {

        public void OnExplorePortClicked()
        {
            // Navigate to the Explore Port / RTSP Player Screen
            ViewController.Instance.ChangeScreen(ScreenName.RTSPPlayerScreen);
        }

        public void OnPlaceObjectClicked()
        {
            // Navigate to the Placement Category Screen
            ViewController.Instance.ChangeScreen(ScreenName.PlacementCategoryScreen);
        }

        public void OnConstructRoadClicked()
        {
            // Navigate to the Road Selection Screen
            ViewController.Instance.ChangeScreen(ScreenName.RoadSelectionScreen);
        }
        
    }
}
