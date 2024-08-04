using System;
using SDVA.InventorySystem;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class OtherInventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventorySlotUI inventoryItemPrefab;

        // CACHE
        private Inventory inventory;

        // LIFECYCLE METHODS

        public void Setup(Inventory inventory)
        {
            this.inventory = inventory;
            inventory.InventoryUpdated += Redraw;
            Redraw();
        }

        public void ShutDown()
        {
            inventory.InventoryUpdated -= Redraw;
        }

        // PRIVATE

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < inventory.GetSize(); i++)
            {
                var itemUI = Instantiate(inventoryItemPrefab, transform);
                itemUI.Setup(inventory, i);
            }
        }
    }
}