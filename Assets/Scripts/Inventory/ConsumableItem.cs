using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// An inventory item that can be used by the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = "SDVA/InventorySystem/ConsumableItem")]
    public class ConsumableItem : BaseItem
    {
        // CONFIG DATA
        [SerializeField] string consumeVerb = "eat";

        // OVERRIDES
        public override string GetItemType() => "Consumable";

        public override void PrimaryAction(Inventory caller)
        {
            if (caller.gameObject.CompareTag("Player"))
            {
                Debug.Log("Swinging tool");
                // TODO 
                // var something caller.gameobject.GetComponent<Something>();
                // something.Swing(this)
            }
        }

        public override void SecondaryAction(Inventory caller)
        {
            if (caller.gameObject.CompareTag("Player"))
            {
                Debug.Log("Secondary Action on tool");
                // TODO 
                // var something caller.gameobject.GetComponent<Something>();
                // something.Swing(this)
            }
        }
    }
}