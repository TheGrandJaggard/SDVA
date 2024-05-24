using UnityEngine;
// // Tracks the drops for saving and restoring. (no longer)
// using SDVA.Saving;
// using Newtonsoft.Json.Linq;
// using UnityEngine.SceneManagement;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// </summary>
    public class ItemDropper : MonoBehaviour//, IJsonSaveable
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

        #region Saving
        
        // // STATE
        // private List<Pickup> droppedItems = new();
        // private List<OtherSceneDropRecord> otherSceneDrops = new();
        
        // class OtherSceneDropRecord
        // {
        //     public string id;
        //     public int number;
        //     public Vector3 location;
        //     public int scene; // I'm not sure if this will work if scenes are loaded at runtime
        // }

        // List<OtherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        // {
        //     List<OtherSceneDropRecord> result = new();
        //     result.AddRange(otherSceneDrops);
        //     foreach (var item in droppedItems)
        //     {
        //         result.Add(new OtherSceneDropRecord
        //         {
        //             id = item.GetItem().GetItemID(),
        //             number = item.GetNumber(),
        //             location = item.transform.position,
        //             scene = SceneManager.GetActiveScene().buildIndex
        //         });
        //     }
        //     return result;
        // }

        // private void ClearExistingDrops()
        // {
        //     foreach (var oldDrop in droppedItems)
        //     {
        //         if (oldDrop != null) Destroy(oldDrop.gameObject);
        //     }
        //     otherSceneDrops.Clear();
        // }

        // /// <summary>
        // /// Remove any drops in the world that have subsequently been picked up.
        // /// </summary>
        // private void RemoveDestroyedDrops()
        // {
        //     var newList = new List<Pickup>();
        //     foreach (var item in droppedItems)
        //     {
        //         if (item != null)
        //         {
        //             newList.Add(item);
        //         }
        //     }
        //     droppedItems = newList;
        // }

        // public JToken CaptureAsJToken()
        // {
        //     RemoveDestroyedDrops();
        //     var drops = MergeDroppedItemsWithOtherSceneDrops();
        //     JArray state = new();
        //     IList<JToken> stateList = state;
        //     foreach (var drop in drops)
        //     {
        //         JObject dropState = new JObject();
        //         IDictionary<string, JToken> dropStateDict = dropState;
        //         dropStateDict["id"] = JToken.FromObject(drop.id);
        //         dropStateDict["number"] = drop.number;
        //         dropStateDict["location"] = drop.location.ToToken();
        //         dropStateDict["scene"] = drop.scene;
        //         stateList.Add(dropState);
        //     }

        //     return state;
        // }
        
        // public void RestoreFromJToken(JToken state)
        // {
        //     if (state is JArray stateArray)
        //     {
        //         int currentScene = SceneManager.GetActiveScene().buildIndex;
        //         IList<JToken> stateList = stateArray;
        //         ClearExistingDrops();
        //         foreach (var entry in stateList)
        //         {
        //             if (entry is JObject dropState)
        //             {
        //                 IDictionary<string, JToken> dropStateDict = dropState;
        //                 int scene = dropStateDict["scene"].ToObject<int>();
        //                 BaseItem item = BaseItem.GetFromID(dropStateDict["id"].ToObject<string>());
        //                 int number = dropStateDict["number"].ToObject<int>();
        //                 Vector3 location = dropStateDict["location"].ToVector3();
        //                 if (scene == currentScene)
        //                 {
        //                     SpawnPickup(item, location, number);
        //                 }
        //                 else
        //                 {
        //                     otherSceneDrops.Add(new OtherSceneDropRecord
        //                     {
        //                         id = item.GetItemID(),
        //                         number = number,
        //                         location = location,
        //                         scene = scene
        //                     });
        //                 }
        //             }
        //         }
        //     }
        // }
        #endregion
    }
}