using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI.
    /// Handles spawning all the inventory slot prefabs.
    /// </summary>
    public class PlayerInventoryUI : InventoryUI
    {
        // LIFECYCLE METHODS

        private void Start()
        {
            Setup(Inventory.GetPlayerInventory());
        }
    }
}
