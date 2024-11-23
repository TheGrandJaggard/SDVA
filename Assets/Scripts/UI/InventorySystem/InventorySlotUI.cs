using UnityEngine;
using System.Linq;
using SDVA.InventorySystem;
using SDVA.Utils.UI.ItemMovement;

namespace SDVA.UI.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IItemTooltipProvider, IItemContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon;

        // STATE

        private int slot;
        private Inventory inventory;

        // PUBLIC

        public void Setup(Inventory inventory, int slot)
        {
            this.inventory = inventory;
            this.slot = slot;
            icon.SetItem(inventory.GetSlotItem(slot), inventory.GetSlotNumber(slot));
        }

        public int MaxAcceptable(BaseItem item)
        {
            if (item == GetItem())
            {
                return item.GetMaxStackSize() - GetNumber();
            }
            else if (GetItem() == null)
            {
                return item.GetMaxStackSize();
            }
            else
            {
                return 0;
            }
        }

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);

        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);

        public IItemSource<BaseItem>[] GetRelatedSources()
        {
            var related = transform.parent.GetComponentsInChildren<IItemSource<BaseItem>>().ToList();
            related.Remove(this);
            return related.ToArray();
        }
    }
}
