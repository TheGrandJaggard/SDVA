namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Base class for both IItemSource and IItemDestination.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public interface IItemHolder<T> where T : class
    {

    }
}