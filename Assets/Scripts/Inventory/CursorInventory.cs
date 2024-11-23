using System.Collections.Generic;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    public class CursorInventory : MonoBehaviour, IItemContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon;

        // STATE

        private int slot;
        private Inventory inventory;

        // PUBLIC

        public int MaxAcceptable(BaseItem item) => item != null ? item.GetMaxStackSize() : 0;

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);
        
        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);

        public IItemSource<BaseItem>[] GetRelatedSources() => new IItemSource<BaseItem>[0];

        // PRIVATE

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
    }
}