﻿using UnityEngine;
using SDVA.Utils.UI.Dragging;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<BaseItem>
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
            icon.SetItem(inventory.GetItemInSlot(slot), inventory.GetNumberInSlot(slot));
        }

        public int MaxAcceptable(BaseItem item) => item.GetMaxStackSize();

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetNumberInSlot(slot);

        public BaseItem GetItem() => inventory.GetItemInSlot(slot);
        
        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(slot, number);
        }
    }
}
