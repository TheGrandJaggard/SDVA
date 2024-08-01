using UnityEngine;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// An inventory item that can be used by the player. Weapons could be a
    /// subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = "SDVA/InventorySystem/MaterialItem")]
    public class MaterialItem : BaseItem
    {
        // CONFIG DATA

        // PUBLIC
        public override string GetItemType() => "Material";

        public override void PrimaryAction(Inventory caller)
        {
        }

        public override void SecondaryAction(Inventory caller)
        {
        }
    }
}