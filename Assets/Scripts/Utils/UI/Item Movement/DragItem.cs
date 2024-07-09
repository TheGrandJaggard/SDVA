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
    public class DragItem<T> : MoveItem<T>, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        // CACHED REFERENCES
        private Canvas parentCanvas;
        private IItemTransitionContainer<T> cursorContainer;

        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            cursorContainer = parentCanvas.GetComponent<IItemTransitionContainer<T>>();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            // if (EventSystem.current.IsPointerOverGameObject() && eventData.pointerEnter)
            // {
            //     IItemSource<T> source;
            //     source = eventData.pointerEnter.GetComponentInParent<IItemSource<T>>();
            //     DropItemIntoDestination(source, cursorContainer);
            // }

            // startPosition = transform.position;
            // originalParent = transform.parent;
            // Else won't get the drop event.
            // GetComponent<CanvasGroup>().blocksRaycasts = false;
            // transform.SetParent(parentCanvas.transform, true);
        }
        
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnDrop");
            // if (EventSystem.current.IsPointerOverGameObject() && eventData.pointerEnter)
            // {
            //     IItemDestination<T> destination;
            //     destination = eventData.pointerEnter.GetComponentInParent<IItemDestination<T>>();
            //     DropItemIntoDestination(cursorContainer, destination);
            // }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            // Do nothing
            // transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            // IItemDestination<T> destination;
            // if (EventSystem.current.IsPointerOverGameObject() && eventData.pointerEnter)
            // {
            //     destination = eventData.pointerEnter.GetComponentInParent<IItemDestination<T>>();
            // }
            // else
            // {
            //     parentCanvas.TryGetComponent(out destination);
            // }
            
            // if (destination != null)
            // {
            //     DropItemIntoDestination(cursorContainer, destination);
            // }

            // transform.position = startPosition;
            // GetComponent<CanvasGroup>().blocksRaycasts = true;
            // transform.SetParent(originalParent, true);
        }
    }
}