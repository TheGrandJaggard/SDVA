using UnityEngine;
using SDVA.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// This component should be generic enough to be placed on chests
    /// </summary>
    public class Inventory : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [Tooltip("Default size")]
        [SerializeField] int inventorySize = 20;

        // STATE
        InventorySlot[] slots;

        public struct InventorySlot
        {
            public BaseItem item;
            public int number;
        }

        // STATIC METHODS

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player"); // TODO Multiplayer Problem
            return player.GetComponent<Inventory>();
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
                if (ReferenceEquals(GetItemInSlot(i), item))
                {
                    total += GetNumberInSlot(i);
                }
            }
            return total;
        }

        /// <summary>
        /// How many more items of type item can fit in this inventory?
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The max number of items that this inventory can recive.</returns>
        public int GetMaxAcceptable(BaseItem item)
        {
            if (!HasSpaceFor(item)) { return 0; }

            var maxAcceptable = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(GetItemInSlot(i), item))
                {
                    maxAcceptable += GetStackSpaceRemaining(i);
                }
                else if (GetItemInSlot(i) == null)
                {
                    maxAcceptable += item.GetMaxStackSize();
                }
            }

            return maxAcceptable;
        }

        /// <returns>How many more items this slot can hold.</returns>
        public int GetStackSpaceRemaining(int slot)
        {
            if (GetItemInSlot(slot) == null) { return 0; }

            return GetItemInSlot(slot).GetMaxStackSize() - GetNumberInSlot(slot);
        }

        /// <returns>The item type in the given slot.</returns>
        public BaseItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        /// <returns>The number of items in the given slot.</returns>
        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
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
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="number">The number to add.</param>
        /// <returns>Number of items added to slot.</returns>
        public int AddToAnySlot(BaseItem item, int number)
        {
            int slot = FindSlot(item);
            if (slot < 0) { return 0; }

            return AddToSlot(slot, item, number);
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
            if (GetItemInSlot(slot) != null)
            {
                if (ReferenceEquals(GetItemInSlot(slot), item))
                {
                    var itemsAdded = Mathf.Clamp(number, 0, GetStackSpaceRemaining(slot));

                    slots[slot].number += itemsAdded;
                    InventoryUpdated?.Invoke();
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
                
                slots[slot].item = item;
                slots[slot].number = itemsAdded;
                InventoryUpdated?.Invoke();
                return itemsAdded;
            }
        }

        /// <summary>
        /// Remove a number of items from the given slot. Will never remove more
        /// that there are.
        /// </summary>
        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;
            if (slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].item = null;
            }
            InventoryUpdated?.Invoke();
        }

        // LIFECYCLE METHODS

        private void Awake()
        {
            slots = new InventorySlot[inventorySize];
        }

        // PRIVATE

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
                if (slots[i].item == null)
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
                if (ReferenceEquals(slots[i].item, item)
                    && GetNumberInSlot(i) < item.GetMaxStackSize())
                {
                    return i;
                }
            }
            return -1;
        }
    
        #region Saving
        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].item != null)
                {
                    JObject itemState = new JObject();
                    IDictionary<string, JToken> itemStateDict = itemState;
                    itemState["item"] = JToken.FromObject(slots[i].item.GetItemID());
                    itemState["number"] = JToken.FromObject(GetNumberInSlot(i));
                    stateDict[i.ToString()] = itemState;
                }
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                slots = new InventorySlot[inventorySize];
                IDictionary<string, JToken> stateDict = stateObject;
                for (int i = 0; i < inventorySize; i++)
                {
                    if (stateDict.ContainsKey(i.ToString()) && stateDict[i.ToString()] is JObject itemState)
                    {
                        IDictionary<string, JToken> itemStateDict = itemState;
                        slots[i].item = BaseItem.GetFromID(itemStateDict["item"].ToObject<string>());
                        slots[i].number = itemStateDict["number"].ToObject<int>();
                    }
                }
                InventoryUpdated?.Invoke();
            }
        }
        #endregion
    }
}
