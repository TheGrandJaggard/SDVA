using UnityEngine;

namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Handles moving items between `IItemContainer`, `IItemSource`, and `IItemDestination`.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public static class MoveItem<T>
        where T : class
    {
        // PUBLIC
        
        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        public static int MoveBetween(IItemSource<T> source, IItemDestination<T> destination)
        {
            if (ReferenceEquals(destination, source)) { return 0; }

            // Check for swappability
            if (destination is IItemContainer<T> destinationContainer &&
                source is IItemContainer<T> sourceContainer &&
                sourceContainer.GetItem() != null &&
                destinationContainer.GetItem() != null &&
                !ReferenceEquals(sourceContainer.GetItem(), destinationContainer.GetItem()))
            {
                var moved = AttemptSwap(sourceContainer, destinationContainer);
                if (moved > 0) { return moved; }
            }

            if (source.GetItem() != null)
            {
                var moved = AttemptSimpleTransfer(source, destination);
                if (moved > 0) { return moved; }
            }

            return 0;
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        public static int MoveTo(IItemSource<T> source, IItemDestination<T> destination)
        {
            return AttemptSimpleTransfer(source, destination);
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <param name="number">The number of items to move.</param>
        /// <returns>The number of items added to destination.</returns>
        public static int MoveTo(IItemSource<T> source, IItemDestination<T> destination, int number)
        {
            return AttemptSimpleTransfer(source, destination, number);
        }

        /// <summary>
        /// Attempt to move all items matching source item from source inventory to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        public static int MoveAllFromInventoryTo(IItemSource<T> source, IItemDestination<T> destination)
        {
            return MoveAllFromInventoryTo(source, destination, source.GetItem());
        }

        /// <summary>
        /// Attempt to move all items of type item from source inventory to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <param name="item">The item type to move from source inventory to destination.</param>
        /// <returns>The number of items added to destination.</returns>
        public static int MoveAllFromInventoryTo(IItemSource<T> source, IItemDestination<T> destination, T item)
        {
            var itemsMoved = AttemptSimpleTransfer(source, destination);

            foreach (var rSource in source.GetRelatedSources())
            {
                if (ReferenceEquals(rSource.GetItem(), item))
                {
                    itemsMoved += AttemptSimpleTransfer(rSource, destination);
                }
            }
            return itemsMoved;
        }

        // PRIVATE

        /// <summary>
        /// Attempt to swap items between source and destination.
        /// </summary>
        /// <param name="source">First container.</param>
        /// <param name="destination">Second container.</param>
        /// <returns>The number of items added to destination.</returns>
        private static int AttemptSwap(IItemContainer<T> source, IItemContainer<T> destination)
        {
            // Provisionally get info from both sides.
            var sourceNumber = source.GetNumber();
            var sourceItem = source.GetItem();
            var destinationNumber = destination.GetNumber();
            var destinationItem = destination.GetItem();

            source.RemoveItems(sourceNumber);
            destination.RemoveItems(destinationNumber);

            // If both the source and destination can recive each other's items
            if (sourceNumber <= destination.MaxAcceptable(sourceItem)
                && destinationNumber <= source.MaxAcceptable(destinationItem))
            {
                source.AddItems(destinationItem, destinationNumber);
                destination.AddItems(sourceItem, sourceNumber);
                return sourceNumber;
            }
            else // Put the items back
            {
                source.AddItems(sourceItem, sourceNumber);
                destination.AddItems(destinationItem, destinationNumber);
                return 0;
            }
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        private static int AttemptSimpleTransfer(IItemSource<T> source, IItemDestination<T> destination)
        {
            var transferred = destination.AddItems(source.GetItem(), source.GetNumber());
            source.RemoveItems(transferred);

            return transferred;
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="source">The item source.</param>
        /// <param name="destination">The destination for items.</param>
        /// <param name="number">The number of items to move.</param>
        /// <returns>The number of items added to destination.</returns>
        private static int AttemptSimpleTransfer(IItemSource<T> source, IItemDestination<T> destination, int number)
        {
            var transferred = destination.AddItems(source.GetItem(), Mathf.Clamp(number, 0, source.GetNumber()));
            source.RemoveItems(transferred);

            return transferred;
        }
    }
}