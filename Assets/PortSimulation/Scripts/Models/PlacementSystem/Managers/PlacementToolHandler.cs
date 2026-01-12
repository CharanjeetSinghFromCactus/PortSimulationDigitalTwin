using DataBindingFramework;
using PortSimulation.Data;
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
        public PlacementToolHandler(MonoBehaviour monoBehaviour, Camera camera, ToolConfig toolConfig)
        {
            _observerManager = ServiceLocator.Current.Get<IObserverManager>();
            _setTargetObserver = _observerManager.GetOrCreateObserver<IPlaceable>(ObserverNameConstants.SetPlacementTarget);
            setCurrentPlacemetTool = _observerManager.GetOrCreateObserver<string>(ObserverNameConstants.SendSelectedToolName);
            setCurrentPlacemetTool.Bind(monoBehaviour, OnSetTool);

            _setTargetObserver.Bind(monoBehaviour, OnSetTarget);

            panTool = new MoveTool(camera, toolConfig.PlacementPanToolData.PlacementAreaMask, toolConfig.PlacementPanToolData.MaxRaycastDistance, toolConfig.PlacementPanToolData.LerpSpeed);
            rotateTool = new RotateTool(toolConfig.PlacementRotateToolData.RotateSpeed);
            ScaleTool = new ScaleTool(toolConfig.PlacementScaleToolData.Speed, toolConfig.PlacementScaleToolData.MaxScale, toolConfig.PlacementScaleToolData.MinScale);

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