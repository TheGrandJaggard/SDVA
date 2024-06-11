using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// Spawns pickups in the world on game start.
    /// Used primarily for testing purposes.
    /// </summary>
    public class StartGamePickupSpawner : ItemDropper
    {
        [SerializeField] BaseItem[] items;

        private void Start()
        {
            SpawnAll();
        }

        public void SpawnAll()
        {
            foreach (var item in items)
            {
                DropItem(item);
            }
        }
    }
}