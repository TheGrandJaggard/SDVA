namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Components that implement this interfaces can act as the destination for
    /// moving items by `MoveItem`.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public interface IItemDestination<T>: IItemHolder<T> where T : class
    {
        /// <summary>
        /// How many of the given item the destination can currently accept.
        /// If there is no limit Int.MaxValue should be returned.
        /// </summary>
        /// <param name="item">The item type to query on.</param>
        /// <returns>The number of items of type item that can be added to destination.</returns>
        int MaxAcceptable(T item);

        /// <summary>
        /// Update the UI and any data to reflect adding the item to this destination.
        /// </summary>
        /// <param name="item">The item type.</param>
        /// <param name="number">The quantity of items.</param>
        /// <returns>Number of items added to destination.</returns>
        int AddItems(T item, int number);
    }
}