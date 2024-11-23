using UnityEngine;
using SDVA.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable number of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// This component should be generic enough to be placed on chests.
    /// </summary>
    public class InventoryComponent : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [Tooltip("Modifiers can effect the actual size (but not yet)")]
        [SerializeField] int baseInventorySize = 20;

        // STATE
        private Inventory inv;

        private void Awake()
        {
            inv = new Inventory(baseInventorySize);
        }

        public Inventory GetInventory() => inv;

        #region Saving
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            for (int i = 0; i < inv.GetSize(); i++)
            {
                if (inv.GetSlotItem(i) != null)
                {
                    JObject itemState = new JObject();
                    IDictionary<string, JToken> itemStateDict = itemState;
                    itemState["item"] = JToken.FromObject(inv.GetSlotItem(i).GetItemID());
                    itemState["number"] = JToken.FromObject(inv.GetSlotNumber(i));
                    stateDict[i.ToString()] = itemState;
                }
            }
            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JObject stateObject)
            {
                IDictionary<string, JToken> stateDict = stateObject;
                inv = new Inventory(Mathf.Max(stateDict.Count, baseInventorySize));

                for (int i = 0; i < inv.GetSize(); i++)
                {
                    if (stateDict.ContainsKey(i.ToString()) && stateDict[i.ToString()] is JObject itemState)
                    {
                        IDictionary<string, JToken> itemStateDict = itemState;
                        inv.SetSlotItem(i, BaseItem.GetFromID(itemStateDict["item"].ToObject<string>()));
                        inv.SetSlotNumber(i, itemStateDict["number"].ToObject<int>());
                    }
                }
                inv.InvokeInventoryUpdateEvent();
            }
        }
        #endregion
    }
}