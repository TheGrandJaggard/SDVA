using UnityEngine;
using UnityEngine.EventSystems;

namespace SDVA.Utils.UI.Dragging
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    /// 
    /// During dragging, the item is reparented to the parent canvas.
    /// 
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IDragContainer`
    /// or `IDragSource` and `IDragDestination` to update the interface after a drag
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        // PRIVATE STATE
        private Vector3 startPosition;
        private Transform originalParent;
        private IDragSource<T> source;

        // CACHED REFERENCES
        private Canvas parentCanvas;

        // LIFECYCLE METHODS
        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();
        }

        // PRIVATE
        #region Interface Triggers
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            startPosition = transform.position;
            originalParent = transform.parent;
            // Else won't get the drop event.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(parentCanvas.transform, true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(originalParent, true);

            IDragDestination<T> container;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                container = GetContainer(eventData);
            }
            else
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }

            if (container != null)
            {
                DropItemIntoDestination(container);
            }
        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                return container;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Attempt to drop items from source into destination.
        /// </summary>
        /// <param name="destination">The destination for items.</param>
        private void DropItemIntoDestination(IDragDestination<T> destination)
        {
            if (ReferenceEquals(destination, source)) { return; }

            // Check for swappability
            if (destination is IDragContainer<T> destinationContainer &&
                source is IDragContainer<T> sourceContainer &&
                destinationContainer.GetItem() != null)
            {
                var successful = AttemptSwap(sourceContainer, destinationContainer);
                if (successful) { return; }
            }

            AttemptSimpleTransfer(destination);
            return;
        }

        /// <summary>
        /// Attempt to swap items between source and destination.
        /// </summary>
        /// <param name="source">First container.</param>
        /// <param name="destination">Second container.</param>
        /// <returns>Whether the attempt was successful.</returns>
        private bool AttemptSwap(IDragContainer<T> source, IDragContainer<T> destination)
        {
            // Provisionally remove item from both sides. 
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

                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to move items from source to destination.
        /// </summary>
        /// <param name="destination">The destination for items.</param>
        /// <returns>The number of items added to destination.</returns>
        private int AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            var transferred = destination.AddItems(source.GetItem(), source.GetNumber());
            source.RemoveItems(transferred);

            return transferred;
        }
    }
}