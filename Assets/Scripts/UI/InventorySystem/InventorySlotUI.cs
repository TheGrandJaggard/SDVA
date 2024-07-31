using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IItemContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int slot;
        Inventory inventory;

        // PUBLIC

        public void Setup(Inventory inventory, int slot)
        {
            this.inventory = inventory;
            this.slot = slot;
            icon.SetItem(inventory.GetSlotItem(slot), inventory.GetSlotNumber(slot));
        }

        public int MaxAcceptable(BaseItem item) => item.GetMaxStackSize();

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);

        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);

        public List<IItemSource<BaseItem>> GetRelatedSources()
        {
            var related = transform.parent.GetComponentsInChildren<IItemSource<BaseItem>>().ToList();
            related.Remove(this);
            return related;
        }
    }
}