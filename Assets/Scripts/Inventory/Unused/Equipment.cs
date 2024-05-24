// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using SDVA.Saving;
// using Newtonsoft.Json.Linq;

// namespace SDVA.InventorySystem
// {
//     /// <summary>
//     /// Provides a store for the items equipped to a player. Items are stored by
//     /// their equip locations.
//     /// 
//     /// This component should be placed on the GameObject tagged "Player".
//     /// </summary>
//     public class Equipment : MonoBehaviour, IJsonSaveable
//     {
//         // STATE
//         Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

//         // PUBLIC

//         /// <summary>
//         /// Broadcasts when the items in the slots are added/removed.
//         /// </summary>
//         public event Action EquipmentUpdated;

//         /// <summary>
//         /// Return the item in the given equip location.
//         /// </summary>
//         public EquipableItem GetItemInSlot(EquipLocation equipLocation)
//         {
//             if (!equippedItems.ContainsKey(equipLocation))
//             {
//                 return null;
//             }

//             return equippedItems[equipLocation];
//         }

//         /// <summary>
//         /// Add an item to the given equip location. Do not attempt to equip to
//         /// an incompatible slot.
//         /// </summary>
//         public void AddItem(EquipLocation slot, EquipableItem item)
//         {
//             Debug.Assert(item.GetAllowedEquipLocation() == slot);

//             equippedItems[slot] = item;

//             if (EquipmentUpdated != null)
//             {
//                 EquipmentUpdated();
//             }
//         }

//         /// <summary>
//         /// Remove the item for the given slot.
//         /// </summary>
//         public void RemoveItem(EquipLocation slot)
//         {
//             equippedItems.Remove(slot);
//             EquipmentUpdated?.Invoke();
//         }

//         /// <summary>
//         /// Enumerate through all the slots that currently contain items.
//         /// </summary>
//         public IEnumerable<EquipLocation> GetAllPopulatedSlots()
//         {
//             return equippedItems.Keys;
//         }

//         // PRIVATE

//         #region Saving
//         public JToken CaptureAsJToken()
//         {
//             JObject state = new JObject();
//             IDictionary<string, JToken> stateDict = state;
//             foreach (var pair in equippedItems)
//             {
//                 stateDict[pair.Key.ToString()] = JToken.FromObject(pair.Value.GetItemID());
//             }
//             return state;
//         }

//         public void RestoreFromJToken(JToken state)
//         {
//             if(state is JObject stateObject)
//             {
//                 equippedItems.Clear();
//                 IDictionary<string, JToken> stateDict = stateObject;
//                 foreach (var pair in stateObject)
//                 {
//                     if (Enum.TryParse(pair.Key, true, out EquipLocation key))
//                     {
//                         if (BaseItem.GetFromID(pair.Value.ToObject<string>()) is EquipableItem item)
//                         {
//                             equippedItems[key] = item;
//                         }
//                     }
//                 }
//             }
//             EquipmentUpdated?.Invoke();
//         }
//         #endregion
//     }
// }