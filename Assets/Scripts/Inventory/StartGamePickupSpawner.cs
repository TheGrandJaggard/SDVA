using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// Spawns pickups in the world on game start.
    /// </summary>
    public class StartGamePickupSpawner : ItemDropper
    {
        [SerializeField] BaseItem[] items;

        private void Start()
        {
            foreach (var item in items)
            {
                DropItem(item);
            }
        }
    }
}