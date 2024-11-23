using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        private BaseItem item;
        private int numItemsContained;

        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(BaseItem item, int number = 1)
        {
            this.item = item;
            numItemsContained = number;

            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.sprite = this.item.GetIcon();
            }
        }

        public BaseItem GetItem() => item;

        public int GetNumber() => numItemsContained;

        public bool CanBePickedUp() => Inventory.GetPlayerInventory().HasSpaceFor(item);

        public void PickupItem()
        {
            if (CanBePickedUp() == false) { return; }
            var numItemsAdded = Inventory.GetPlayerInventory().AddToAnySlot(item, numItemsContained);
            numItemsContained -= numItemsAdded;
            if (numItemsContained <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
