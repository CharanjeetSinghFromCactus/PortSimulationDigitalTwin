using DataBindingFramework;
using PortSimulation.Managers;
using ServiceLocatorFramework;
using UnityEngine;

namespace PortSimulation.PlacementSystem
{
    public class PlacementToolHandler
    {
        private IPlacementTool panTool, rotateTool, ScaleTool;
        private IObserverManager _observerManager;
        private DataBindingFramework.IObserver<IPlaceable> _setTargetObserver;
        private DataBindingFramework.IObserver<string> setCurrentPlacemetTool;
        private ToolManager toolManager;
        IPlaceable target;
        public PlacementToolHandler(MonoBehaviour monoBehaviour,Camera camera,LayerMask placementAreaMask,float maxRaycastDistance,float rotateSpeed,float scaleSpeed,float minScale,float maxScale)
        {
            _observerManager = ServiceLocator.Current.Get<IObserverManager>();
            _setTargetObserver = _observerManager.GetOrCreateObserver<IPlaceable>(ObserverNameConstants.SetPlacementTarget);
            setCurrentPlacemetTool = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SendSelectedToolName);
            setCurrentPlacemetTool.Bind(monoBehaviour, OnSetTool);

            _setTargetObserver.Bind(monoBehaviour, OnSetTarget);

            panTool = new PanTool(camera, placementAreaMask, maxRaycastDistance);
            rotateTool = new RotateTool(rotateSpeed);
            ScaleTool = new ScaleTool(scaleSpeed, maxScale, minScale);

            toolManager = ServiceLocator.Current.Get<ToolManager>();
            toolManager.RegisterTool(ToolNameConstants.PlacementPanToolName,panTool);
            toolManager.RegisterTool(ToolNameConstants.PlacementRotateToolName,rotateTool);
            toolManager.RegisterTool(ToolNameConstants.PlacementScaleToolName,ScaleTool);
        }

        ~PlacementToolHandler()
        {
            toolManager.UnregisterTool(ToolNameConstants.PlacementPanToolName);
            toolManager.UnregisterTool(ToolNameConstants.PlacementRotateToolName);
            setCurrentPlacemetTool.Unbind(OnSetTool);
            _setTargetObserver?.Unbind(OnSetTarget);
            _observerManager.RemoveObserver(ObserverNameConstants.SendSelectedToolName);
            _observerManager.RemoveObserver(ObserverNameConstants.SetPlacementTarget);
        }

        private void OnSetTarget(IPlaceable target)
        {
            if (target != null)
            {
                this.target = target;
                panTool.SetTarget(target);
                rotateTool.SetTarget(target);
                ScaleTool.SetTarget(target);
            }
        }

        private void OnSetTool(string toolName)
        {
            Debug.Log($"Selected Tool {toolName}");
            toolManager.ActivateTool(toolName);
        }
        
    }
}