using UnityEngine;
using SDVA.InventorySystem;
using SDVA.Utils;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class PlayerInventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventorySlotUI inventoryItemPrefab;

        // CACHE
        private Inventory playerInventory;

        // LIFECYCLE METHODS

        private void Start()
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.InventoryUpdated += Redraw;
            Redraw();
        }

        // PRIVATE

        private void Redraw()
        {
            var goI = 0;
            for (int invI = 0; invI < playerInventory.GetSize(); goI++)
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
                    itemUI.Setup(playerInventory, invI);
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
