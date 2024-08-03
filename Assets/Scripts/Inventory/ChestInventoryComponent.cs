using UnityEngine;
using SDVA.UI.InventorySystem;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// This component should be placed on chests
    /// </summary>
    public class ChestInventoryComponent : InventoryComponent, IGuiProvider
    {
        // CONFIG DATA
        [SerializeField] GameObject guiScreen;

        /// <summary>
        /// Sets the chest's state.
        /// </summary>
        /// <param name="isOpen">Whether the chest is open.</param>
        public void ChestOpen(bool isOpen)
        {
            if (isOpen)
            {
                Debug.Log("Open Chest");
            }
            else
            {
                Debug.Log("Close Chest");
            }
        }

        public GameObject SetupGui(Transform parent)
        {
            var gui = Instantiate(guiScreen, parent);
            gui.GetComponentInChildren<OtherInventoryUI>().Setup(GetInventory());

            ChestOpen(true);
            return gui;
        }
    }
}