using System;
using Games.CameraManager;
using UISystem;
using UnityEngine;
using UnityEngine.UI;
using Sttplay.MediaPlayer;
using PortSimulation.POI;

namespace PortSimulation.UI
{
    public class RTSPPlayerScreen : UISystem.Screen
    {
        public RawImage rawImage;
        [SerializeField] private UnitySCPlayerPro mediaPlayer;
        private CCTVData currentCCTVData;

        private void Start()
        {
            canvas.worldCamera = ServiceLocatorFramework.ServiceLocator.Current.Get<ICameraManager>().GetCamera();
        }

        public override void Show()
        {
            currentCCTVData = ServiceLocatorFramework.ServiceLocator.Current.Get<CCTVData>();
            if (currentCCTVData != null)
            {
                mediaPlayer.Open(MediaType.Link, currentCCTVData.rtspUrl);
            }
            CoroutineExtension.ExecuteAfterFrame(this, () =>
            {
                ServiceLocatorFramework.ServiceLocator.Current.Unregister<CCTVData>();
            });
            base.Show();
        }

        public override void Hide()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Close();
            }
            base.Hide();
            rawImage.texture = null;
        }
        
        public void OnCloseButtonClick()
        {
            var poi = ServiceLocatorFramework.ServiceLocator.Current.Get<POIReference>().CurrentPOI;
            if (poi != null)
            {
                poi.OnHide();
            }
            UISystem.ViewController.Instance.ChangeScreen(UISystem.ScreenName.HomeScreen);
        }
    }
}