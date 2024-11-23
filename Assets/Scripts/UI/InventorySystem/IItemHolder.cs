using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// Allows the `ItemTooltipSpawner` to display the right information.
    /// </summary>
    public interface IItemHolder
    {
        BaseItem GetItem();
    }
}
