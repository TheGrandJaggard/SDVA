using UnityEngine;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    public class CursorInventory : MonoBehaviour, IItemTransitionContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] CursorItemIcon icon;

        // STATE
        int slot;
        Inventory inventory;

        // PUBLIC

        private void Awake()
        {
            inventory = new Inventory(1);
            slot = 0;

            inventory.InventoryUpdated += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            if (inventory.GetSlotItem(slot) == null)
            {
                icon.GetComponent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                icon.GetComponent<CanvasGroup>().alpha = 1f;
                icon.SetItem(inventory.GetSlotItem(slot), inventory.GetSlotNumber(slot));
            }
        }

        public int MaxAcceptable(BaseItem item) => item?.GetMaxStackSize() ?? 0;

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);
        
        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);
    }
}