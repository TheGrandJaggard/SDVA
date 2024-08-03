using SDVA.InventorySystem;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class PlayerInventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventorySlotUI inventoryItemPrefab;

        // CACHE
        private Inventory playerInventory;

        // LIFECYCLE METHODS

        private void Start()
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.InventoryUpdated += Redraw;
            Redraw();
        }

        // PRIVATE

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(inventoryItemPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }
    }
}