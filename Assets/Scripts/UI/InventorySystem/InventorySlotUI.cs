using UnityEngine;
using SDVA.Utils.UI.Dragging;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;
        Inventory inventory;

        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        public int MaxAcceptable(BaseItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(BaseItem item, int number)
        {
            inventory.AddItemsToSlot(index, item, number);
        }
        
        public int GetNumber() => inventory.GetNumberInSlot(index);

        public BaseItem GetItem() => inventory.GetItemInSlot(index);
        
        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }

    }
}