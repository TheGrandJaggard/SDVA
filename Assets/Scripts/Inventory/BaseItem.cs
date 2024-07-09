using System.Collections.Generic;
using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public abstract class BaseItem : ScriptableObject, ISerializationCallbackReceiver
    {
        // CONFIG DATA
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] string itemID;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] string displayName;
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField][TextArea] string description;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField] Sprite icon;
        [Tooltip("The maximum number of these items that can be held in one slot.")]
        [SerializeField] int stackSize = 50;
        
        [Tooltip("The default price for this item.")]
        [SerializeField] int sellPrice = 0;

        // STATE
        static Dictionary<string, BaseItem> itemLookupCache;

        // PUBLIC

        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static BaseItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, BaseItem>();
                var itemList = Resources.LoadAll<BaseItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError($"Looks like there's a duplicate SDVA.InventorySystem ID for objects: {itemLookupCache[item.itemID]} and {item}");
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }

        ///<returns>The UUID of this item.</returns>
        public string GetItemID() => itemID;

        ///<returns>The name of this item.</returns>
        public string GetDisplayName() => displayName;

        ///<returns>The description of this item.</returns>
        public string GetDescription() => description;

        ///<returns>The icon of this item.</returns>
        public Sprite GetIcon() => icon;

        ///<returns>The animation frames of this item.</returns>
        public Sprite[] GetAnimation() => new Sprite[] { icon };

        ///<returns>The max number of items of this type that can be held in a single stack by default.</returns>
        public int GetMaxStackSize() => stackSize;

        ///<returns>The base sell price of this item.</returns>
        public int GetSellPrice() => sellPrice;

        ///<returns>The type of this item.</returns>
        public abstract string GetItemType();

        /// <summary>
        /// The action performed on a left-click (by default).
        /// </summary>
        /// <param name="caller">The inventory (player) that called this action.</param>
        public abstract void PrimaryAction(Inventory caller); // Should this really take an inventory?

        /// <summary>
        /// The action performed on a right-click (by default).
        /// </summary>
        /// <param name="caller">The inventory (player) that called this action.</param>
        public abstract void SecondaryAction(Inventory caller); // Should this really take an inventory?

        #region UUID Serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Required by the ISerializationCallbackReceiver
        }
        #endregion
    }
}
