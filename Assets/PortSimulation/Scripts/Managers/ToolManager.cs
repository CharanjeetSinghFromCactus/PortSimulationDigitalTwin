using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PortSimulation.Tools;
using ServiceLocatorFramework;

namespace PortSimulation.Managers
{
    public class ToolManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        

        private Dictionary<string, ITool> tools = new Dictionary<string, ITool>();
        private string currentToolName;
        private ITool activeTool;
        private void OnEnable()
        {
            ServiceLocator.Current.Register(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Current.Unregister<ToolManager>();
        }
        

        private void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (activeTool != null)
            {
                activeTool.UpdateTool();
            }
        }

        public void RegisterTool(string toolName, ITool tool,bool isInitTool = false)
        {
            if (!tools.ContainsKey(toolName))
            {
                tools.Add(toolName, tool);
            }
            else
            {
                tools[toolName] = tool;
            }
            
            if (isInitTool)
            {
                ActivateTool(toolName);
            }
        }

        public void UnregisterTool(string toolName)
        {
            if (tools.ContainsKey(toolName))
            {
                tools.Remove(toolName);
            }
        }

        public void ActivateTool(string toolName)
        {
            currentToolName = toolName;
            activeTool = null;
            

            if (tools.ContainsKey(toolName))
            {
                ITool tool = tools[toolName];
                activeTool = tool;
            }
        }

        public string GetCurrentTool()
        {
            return currentToolName;
        }
    }
}