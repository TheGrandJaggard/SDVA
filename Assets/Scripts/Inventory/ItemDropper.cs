using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// </summary>
    public class ItemDropper : MonoBehaviour
    {
        // CACHE MEMORY
        [SerializeField] GameObject pickupPrefab;
        // PUBLIC

        /// <summary>
        /// Create an item pickup in the world.
        /// </summary>
        /// <param name="item">The item contained in the pickup.</param>
        /// <param name="number">Number of items contained in the pickup. Defaults to 1.</param>
        /// <param name="position">Where to spawn the pickup. Defaults to position of spawner.</param>
        /// <returns>Reference to the pickup object spawned.</returns>

        public Pickup DropItem(BaseItem item, int number = 1, Vector2? position = null)
        {
            if (pickupPrefab.GetComponent<Pickup>() == null) { return null; }
            var pickup = Instantiate(pickupPrefab).GetComponent<Pickup>();

            pickup.transform.position = position ?? GetDropLocation();
            pickup.Setup(item, number);
            return pickup;
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector2 GetDropLocation()
        {
            return transform.position;
        }
    }
}