using System.Collections.Generic;

namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Components that implement this interfaces can act as the source for
    /// moving items by `DragItem` or `TODO put name here`.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public interface IItemSource<T> where T : class
    {
        /// <summary>
        /// What item type currently resides in this source?
        /// </summary>
        T GetItem();

        /// <summary>
        /// What is the quantity of items in this source?
        /// </summary>
        int GetNumber();

        /// <summary>
        /// Remove a given number of items from the source.
        /// </summary>
        /// <param name="number">This should never exceed the number returned by `GetNumber`.</param>
        void RemoveItems(int number);

        /// <summary>
        /// Gets related sources. (eg. other slots in the same inventory)
        /// </summary>
        /// <returns>A list of related IItemSources.</returns>
        List<IItemSource<T>> GetRelatedSources();
    }
}