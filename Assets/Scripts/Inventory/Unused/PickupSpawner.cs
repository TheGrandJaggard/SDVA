// using UnityEngine;
// using SDVA.Saving;
// using Newtonsoft.Json.Linq;

// namespace SDVA.InventorySystem
// {
//     /// <summary>
//     /// Spawns pickups that should exist on first load in a level. This
//     /// automatically spawns the correct prefab for a given inventory item.
//     /// </summary>
//     public class PickupSpawner : MonoBehaviour, IJsonSaveable
//     {
//         // CONFIG DATA
//         [SerializeField] BaseItem item = null;
//         [SerializeField] int number = 1;

//         // LIFECYCLE METHODS
//         private void Awake()
//         {
//             // Spawn in Awake so can be destroyed by save system after.
//             SpawnPickup();
//         }

//         // PUBLIC

//         /// <summary>
//         /// Returns the pickup spawned by this class if it exists.
//         /// </summary>
//         /// <returns>Returns null if the pickup has been collected.</returns>
//         public Pickup GetPickup() => GetComponentInChildren<Pickup>();

//         /// <summary>
//         /// True if the pickup was collected.
//         /// </summary>
//         public bool IsCollected() => GetPickup() == null;

//         // PRIVATE

//         private void SpawnPickup()
//         {
//             var spawnedPickup = item.SpawnPickup(transform.position, number);
//             spawnedPickup.transform.SetParent(transform);
//         }

//         private void DestroyPickup()
//         {
//             if (GetPickup())
//             {
//                 Destroy(GetPickup().gameObject);
//             }
//         }

//         #region Saving
//         public JToken CaptureAsJToken() => JToken.FromObject(IsCollected());

//         public void RestoreFromJToken(JToken state)
//         {
//             bool shouldBeCollected = state.ToObject<bool>();

//             if (shouldBeCollected && !IsCollected())
//             {
//                 DestroyPickup();
//             }

//             if (!shouldBeCollected && IsCollected())
//             {
//                 SpawnPickup();
//             }
//         }
//         #endregion
//     }
// }