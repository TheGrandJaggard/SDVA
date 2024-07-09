using UnityEngine;
using UnityEngine.EventSystems;

namespace SDVA.Utils.UI.ItemMovement
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped to and from a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    ///
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IItemContainer`
    /// or `IItemSource` and `IItemDestination` to update the interface after a drag
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public class DragItem<T> : MoveItem<T>, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        private Vector3 startPosition;
        private Transform originalParent;

        // CACHED REFERENCES
        private Canvas parentCanvas;

        private new void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            base.Awake();
        }

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

            IItemDestination<T> container;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                container = GetContainer(eventData);
            }
            else
            {
                container = parentCanvas.GetComponent<IItemDestination<T>>();
            }

            if (container != null)
            {
                DropItemIntoDestination(container);
            }
        }

        private IItemDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IItemDestination<T>>();
                return container;
            }
            return null;
        }
    }
}