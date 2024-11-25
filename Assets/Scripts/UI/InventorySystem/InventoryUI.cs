using SDVA.InventorySystem;
using SDVA.Utils;
using SDVA.Utils.UI.ItemMovement;
using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour, IItemDestination<BaseItem>
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

        // PUBLIC

        public Inventory GetInventory() => inventory;

        public int AddItems(BaseItem item, int number) => inventory.AddToAnySlot(item, number);

        public int MaxAcceptable(BaseItem item) => inventory.GetSpaceRemaining(item);

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
                    child.SetSiblingIndex(goI);
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
