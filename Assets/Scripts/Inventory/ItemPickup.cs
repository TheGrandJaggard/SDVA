using UnityEngine;


namespace SDVA.InventorySystem
{
    public class ItemPickup : Pickup
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PickupItem();
            }
        }
    }
}