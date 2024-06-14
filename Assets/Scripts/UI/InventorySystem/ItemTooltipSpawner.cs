using UnityEngine;
using SDVA.Utils.UI;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();
            if (!item) { return false; }
            else { return true; }
        }

        public override void UpdateTooltip(Tooltip tooltip)
        {
            if (tooltip.GetType() != typeof(ItemTooltip)) { return; }

            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            var item = GetComponent<IItemHolder>().GetItem();

            itemTooltip.SetupContent(item);
        }
    }
}