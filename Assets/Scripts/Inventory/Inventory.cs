using UnityEngine;
using System;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    /// </summary>
    public class Inventory
    {
        // STATE
        InventorySlot[] slots;

        private struct InventorySlot
        {
            public BaseItem item;
            public int number;
        }

        // CONSTRUCTOR
        public Inventory(int inventorySize)
        {
            slots = new InventorySlot[inventorySize];
        }

        // STATIC METHODS

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player"); // TODO Multiplayer Problem
            return player.GetComponent<InventoryComponent>().GetInventory();
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action InventoryUpdated;

        /// <returns>Whether the given item can fit anywhere in the inventory.</returns>
        public bool HasSpaceFor(BaseItem item) => FindSlot(item) >= 0;

        /// <returns>Number of slots in inventory.</returns>
        public int GetSize() => slots.Length;

        /// <summary>
        /// How many items of type item are in the inventory?
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>Number of items of given type contained in inventory, 0 if none were found.</returns>
        public int GetItemsContained(BaseItem item)
        {
            var total = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(GetSlotItem(i), item))
                {
                    total += GetSlotNumber(i);
                }
            }
            return total;
        }

        /// <summary>
        /// How many more items of type item can fit in this inventory?
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The max number of items of type item that this inventory can recive.</returns>
        public int GetSpaceRemaining(BaseItem item)
        {
            if (!HasSpaceFor(item)) { return 0; }

            var maxAcceptable = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(GetSlotItem(i), item))
                {
                    maxAcceptable += GetStackSpaceRemaining(i);
                }
                else if (GetSlotItem(i) == null)
                {
                    maxAcceptable += item.GetMaxStackSize();
                }
            }

            return maxAcceptable;
        }

        /// <returns>How many more items this slot can hold.</returns>
        public int GetStackSpaceRemaining(int slot)
        {
            if (GetSlotItem(slot) == null) { return 0; }

            return GetSlotItem(slot).GetMaxStackSize() - GetSlotNumber(slot);
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If items
        /// cannot be added to the given slot then they will be added
        /// to a default slot instead.
        /// Sometimes not all items will be added to the inventory!
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>Number of items added to slot.</returns>
        public int AddToPreferredSlot(int slot, BaseItem item, int number)
        {
            var added = AddToSlot(slot, item, number);
            if (added < number)
            {
                added += AddToAnySlot(item, number - added);
            }
            return added;
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// Will recursively add items to available slots 
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Number of items added to slot.</returns>
        public int AddToAnySlot(BaseItem item, int number)
        {
            int totalAdded = 0;
            while (HasSpaceFor(item) && totalAdded < number)
            {
                int slot = FindSlot(item);
                totalAdded += AddToSlot(slot, item, number - totalAdded);
            }
            return totalAdded;
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>Number of items added to slot.</returns>
        public int AddToSlot(int slot, BaseItem item, int number)
        {
            if (GetSlotItem(slot) != null)
            {
                if (ReferenceEquals(GetSlotItem(slot), item))
                {
                    var itemsAdded = Mathf.Clamp(number, 0, GetStackSpaceRemaining(slot));

                    SetSlotNumber(slot, GetSlotNumber(slot) + itemsAdded);
                    InvokeInventoryUpdateEvent();
                    return itemsAdded;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var itemsAdded = Mathf.Clamp(number, 0, item.GetMaxStackSize());

                SetSlotItem(slot, item);
                SetSlotNumber(slot, itemsAdded);
                InvokeInventoryUpdateEvent();
                return itemsAdded;
            }
        }

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// than there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            SetSlotNumber(slot, GetSlotNumber(slot) - number);
            if (GetSlotNumber(slot) <= 0)
            {
                SetSlotNumber(slot, 0);
                SetSlotItem(slot, null);
            }
            InvokeInventoryUpdateEvent();
        }

        /// <returns>The item type in the given slot.</returns>
        public BaseItem GetSlotItem(int slot) => slots[slot].item;

        /// <returns>The number of items in the given slot.</returns>
        public int GetSlotNumber(int slot) => slots[slot].number;

        #region Internal
        /// <summary>
        /// Set the item in slot 'slot' to 'item'.
        /// </summary>
        internal void SetSlotItem(int slot, BaseItem item) => slots[slot].item = item;

        /// <summary>
        /// Set the number of items in slot 'slot' to 'number'.
        /// </summary>
        internal void SetSlotNumber(int slot, int number) => slots[slot].number = number;

        /// <summary>
        /// Invokes the InventoryUpdated event.
        /// </summary>
        internal void InvokeInventoryUpdateEvent() => InventoryUpdated?.Invoke();
        #endregion

        #region Private
        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// Will find a matching stack with space remaining or an empty slot.
        /// </summary>
        /// <returns>Slot index, or -1 if no slot is found.</returns>
        private int FindSlot(BaseItem item)
        {
            int i = FindStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>Slot index, or -1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (GetSlotItem(i) == null)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find an existing stack of this item type that has stack space remaining.
        /// </summary>
        /// <returns>Slot index, or -1 if no stack exists.</returns>
        private int FindStack(BaseItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(GetSlotItem(i), item)
                    && GetSlotNumber(i) < item.GetMaxStackSize())
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}
