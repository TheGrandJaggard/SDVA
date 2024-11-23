namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Components that implement this interfaces can act as the source for
    /// moving items by `MoveItem`.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public interface IItemSource<T>: IItemHolder<T> where T : class
    {
        /// <returns>The item type currently held by this source.</returns>
        T GetItem();

        /// <returns>The number of items this source currently holds.</returns>
        int GetNumber();

        /// <summary>
        /// Remove a given number of items from the source.
        /// </summary>
        /// <param name="number">This should never exceed the number returned by `GetNumber`.</param>
        void RemoveItems(int number);

        /// <summary>
        /// Gets related sources. (e.g. other slots in the same inventory)
        /// </summary>
        /// <returns>A list of related IItemSources.</returns>
        IItemSource<T>[] GetRelatedSources();
    }
}
