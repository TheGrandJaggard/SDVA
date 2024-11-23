using UnityEngine;
using SDVA.Utils.UI;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    [RequireComponent(typeof(IItemTooltipProvider))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            if (Input.GetMouseButton(0) == true) { return false; }
            if (GetComponent<IItemTooltipProvider>().GetItem() == null) { return false; }
            else { return true; }
        }

        public override void UpdateTooltip(Tooltip tooltip)
        {
            if (tooltip.GetType() != typeof(ItemTooltip)) { return; }

            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            var item = GetComponent<IItemTooltipProvider>().GetItem();

            itemTooltip.SetupContent(item);
        }
    }
}