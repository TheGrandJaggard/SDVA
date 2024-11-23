using System;
using SDVA.InventorySystem;
using SDVA.Utils;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class OtherInventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventorySlotUI inventoryItemPrefab;

        // CACHE
        private Inventory inventory;

        // LIFECYCLE METHODS

        public void Setup(Inventory inventory)
        {
            this.inventory = inventory;
            inventory.InventoryUpdated += Redraw;
            Redraw();
        }

        public void ShutDown()
        {
            inventory.InventoryUpdated -= Redraw;
        }

        // PRIVATE

        private void Redraw()
        {
            var goI = 0;
            for (int invI = 0; invI < inventory.GetSize(); goI++)
            {
                Transform child;
                try
                {
                    child = transform.GetChild(goI);
                    if (child.gameObject.IsDestroyed())
                    {
                        throw new UnityException();
                    }
                }
                catch (UnityException)
                {
                    child = Instantiate(inventoryItemPrefab, transform).transform;
                    child.SetAsFirstSibling();
                    goI++;
                }

                if (child.TryGetComponent<InventorySlotUI>(out var itemUI))
                {
                    itemUI.Setup(inventory, invI);
                    invI++;
                }
                else
                {
                    child.gameObject.Destroy();
                }
            }

            for (; goI < transform.childCount; goI++)
            {
                Transform child = transform.GetChild(goI);

                child.gameObject.Destroy();
            }
        }
    }
}
