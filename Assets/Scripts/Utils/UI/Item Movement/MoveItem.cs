using UnityEngine;

namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped or click and dropped
    /// to and from a container.
    /// 
    /// Create a subclass for the type you want to be moveable. Then place on
    /// the UI element you want to make moveable.
    ///
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IItemContainer`
    /// or `IItemSource` and `IItemDestination` to update the interface after a move
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being moved.</typeparam>
    public abstract class MoveItem<T> : MonoBehaviour
        where T : class
    {
        // PRIVATE

        /// <summary>
        /// Attempt to drop items from source into destination.
        /// </summary>
        /// <param name="source">The items' source.</param>
        /// <param name="destination">The destination for items.</param>
        internal void DropItemIntoDestination(IItemSource<T> source, IItemDestination<T> destination)
        {
            if (ReferenceEquals(destination, source)) { return; }

            // Check for swappability
            if (destination is IItemContainer<T> destinationContainer &&
                source is IItemContainer<T> sourceContainer &&
                destinationContainer.GetItem() != null)
            {
                var successful = AttemptSwap(sourceContainer, destinationContainer);
                if (successful) { return; }
            }

            AttemptSimpleTransfer(source, destination);
            return;
        }

        /// <summary>
        /// Attempt to swap items between source and destination.
        /// </summary>
        /// <param name="source">First container.</param>
        /// <param name="destination">Second container.</param>
        /// <returns>Whether the attempt was successful.</returns>
        private bool AttemptSwap(IItemContainer<T> source, IItemContainer<T> destination)
        {
            // Provisionally remove item from both sides. 
            var sourceNumber = source.GetNumber();
            var sourceItem = source.GetItem();
            var destinationNumber = destination.GetNumber();
            var destinationItem = destination.GetItem();
            
            // If both the source and destination can recive each other's items
            if (sourceNumber <= destination.MaxAcceptable(sourceItem)
                && destinationNumber <= source.MaxAcceptable(destinationItem))
            {
                source.RemoveItems(sourceNumber);
                destination.RemoveItems(destinationNumber);
                
                source.AddItems(destinationItem, destinationNumber);
                destination.AddItems(sourceItem, sourceNumber);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        private int AttemptSimpleTransfer(IItemSource<T> source, IItemDestination<T> destination)
        {
            var transferred = destination.AddItems(source.GetItem(), source.GetNumber());
            source.RemoveItems(transferred);

            return transferred;
        }
    }
}