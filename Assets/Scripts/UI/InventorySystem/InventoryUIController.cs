using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] GameObject guiParent;
        private Dictionary<IGuiProvider, GameObject> guis;
        
        private void Awake()
        {
            guis = new();
        }

        /// <summary>
        /// Adds a peice of GUI to the left side of the screen.
        /// </summary>
        /// <param name="provider">The provider for the GUI to be added.</param>
        public void AddGuiToOtherTab(IGuiProvider provider)
        {
            // Debug.Log("Adding GUI to other tab");
            var gui = provider.SetupGui(guiParent.transform);
            guis.Add(provider, gui);
        }

        /// <summary>
        /// Removes a peice of GUI from the left side of the screen.
        /// </summary>
        /// <param name="provider">The provider for the GUI to be removed.</param>
        public void RemoveGuiFromOtherTab(IGuiProvider provider)
        {
            // Debug.Log("Removing GUI from other tab");
            provider.ShutDownGui();
            guis.Remove(provider);
        }

        /// <returns>
        /// All inventories in left GUI pannels.
        /// </returns>
        public InventoryUI[] GetInventoriesInGui()
        {
            var guisObjects = guis.Select(gui => gui.Value);
            var inventories = guisObjects.SelectMany(guiObject => guiObject.GetComponentsInChildren<InventoryUI>());
            return inventories.ToArray();
        }
    }
}
