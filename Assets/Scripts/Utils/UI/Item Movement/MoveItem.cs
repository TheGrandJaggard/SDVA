using UnityEngine;

namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Handles moving items between `IItemContainer`, `IItemSource`, and `IItemDestination`.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public abstract class MoveItem<T> : MonoBehaviour
        where T : class
    {
        // PUBLIC
        
        /// <summary>
        /// Attempt to drop items from source into destination.
        /// </summary>
        /// <param name="destination">The destination for items.</param>
        public static int MoveBetween(IItemSource<T> source, IItemDestination<T> destination)
        {
            if (ReferenceEquals(destination, source)) { return 0; }

            // Check for swappability
            if (destination is IItemContainer<T> destinationContainer &&
                source is IItemContainer<T> sourceContainer &&
                sourceContainer.GetItem() != null &&
                destinationContainer.GetItem() != null)
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

        public static int MoveTo(IItemSource<T> source, IItemDestination<T> destination)
        {
            return AttemptSimpleTransfer(source, destination);
        }

        // PRIVATE

        /// <summary>
        /// Attempt to swap items between source and destination.
        /// </summary>
        /// <param name="source">First container.</param>
        /// <param name="destination">Second container.</param>
        /// <returns>Whether the attempt was successful.</returns>
        private static int AttemptSwap(IItemContainer<T> source, IItemContainer<T> destination)
        {
            // Provisionally get info from both sides.
            var sourceNumber = source.GetNumber();
            var sourceItem = source.GetItem();
            var destinationNumber = destination.GetNumber();
            var destinationItem = destination.GetItem();
            
            // if both the source and destination can recive each other's items
            if (sourceNumber <= destination.MaxAcceptable(sourceItem)
                && destinationNumber <= source.MaxAcceptable(destinationItem))
            {
                source.RemoveItems(sourceNumber);
                destination.RemoveItems(destinationNumber);
                
                source.AddItems(destinationItem, destinationNumber);
                destination.AddItems(sourceItem, sourceNumber);

                return sourceNumber;
            }
            return 0;
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        private static int AttemptSimpleTransfer(IItemSource<T> source, IItemDestination<T> destination)
        {
            var transferred = destination.AddItems(source.GetItem(), source.GetNumber());
            source.RemoveItems(transferred);

            return transferred;
        }
    }
}