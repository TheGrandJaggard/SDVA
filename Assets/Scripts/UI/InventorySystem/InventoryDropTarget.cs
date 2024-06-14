using UnityEngine;
using SDVA.Utils.UI.Dragging;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// Handles spawning pickups when item dropped into the world.
    /// 
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space. 
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<BaseItem>
    {
        public void AddItems(BaseItem item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player"); // TODO Multiplayer Problem
            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int MaxAcceptable(BaseItem item)
        {
            return int.MaxValue;
        }
    }
}