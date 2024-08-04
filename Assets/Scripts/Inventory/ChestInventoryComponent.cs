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
        [SerializeField] GameObject guiPrefab;

        // STATE
        private GameObject guiObject;

        /// <summary>
        /// Sets the chest's state.
        /// </summary>
        /// <param name="isOpen">Whether the chest is open.</param>
        public void ChestOpen(bool isOpen)
        {
            if (isOpen)
            {
                Debug.Log("Open Chest"); // TODO
            }
            else
            {
                Debug.Log("Close Chest");
            }
        }

        public GameObject SetupGui(Transform parent)
        {
            guiObject = Instantiate(guiPrefab, parent);
            guiObject.GetComponentInChildren<OtherInventoryUI>().Setup(GetInventory());

            ChestOpen(true);
            return guiObject;
        }

        public void ShutDownGui()
        {
            ChestOpen(false);
            guiObject.GetComponentInChildren<OtherInventoryUI>().ShutDown();
            Destroy(guiObject);
        }
    }
}