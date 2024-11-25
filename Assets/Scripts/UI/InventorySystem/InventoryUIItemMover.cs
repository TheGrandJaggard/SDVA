using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using SDVA.Utils;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;
using System.Collections.Generic;
using System.Linq;

namespace SDVA.UI.InventorySystem
{
    public class InventoryUIItemMover : MonoBehaviour
    {
        [SerializeField] CursorInventory cursorInventory;
        [SerializeField] InventoryUIController inventoryUIController;
        
        [Header("Item Movement Settings:")]
        [Tooltip("Whether items can be picked up by clicking.")]
        [SerializeField] bool useClick;
        [Tooltip("Whether items are dropped after a drag ends.\n(Not recommended, but makes for a simpler system when other settings are turned off.)")]
        [SerializeField] bool useDragging;
        [Tooltip("Whether items should be spread out between slots and aggregated from slots that they are dragged over. (useDragging must be false for cursorInventory to function properly) (WIP)")]
        [SerializeField] bool useDragSpread;

        [Header("Item Movement Inputs:")]
        [Tooltip("The input that picks up and drops items.\n(Default = LeftClick)")]
        [SerializeField] InputActionReference moveItemButton;
        [Tooltip("The input that picks up and drops single items.\n(Default = RightClick)")]
        [SerializeField] InputActionReference singleItemButton;
        [Tooltip("The maximum number of seconds between clicks to count as a double click.\n(Default  = 0.4)")]
        [SerializeField] float doubleClickTime = 0.4f;
        [Tooltip("The input that changes click functionality to transfer between containers.\n(Default = Shift) (WIP)")]
        [SerializeField] InputActionReference transferItemButton;
        [Tooltip("The input that opens a custom menu for the item clicked item.\n(Default = LeftRightClick) (WIP)")]
        [SerializeField] InputActionReference openActionMenuButton;
        [Tooltip("The input that signals a release of items dragged.\n(Default = LeftRightClickHeld)")]
        [SerializeField] InputActionReference moveItemButtonRelease;

        private IItemSource<BaseItem> mostRecentSource; // Used for dragging returns and collect all movement
        private IItemDestination<BaseItem> mostRecentDestination; // Used for collect all movement
        private Timer timer;

        private void OnEnable()
        {
            moveItemButton.ToInputAction().Enable();
            singleItemButton.ToInputAction().Enable();
            moveItemButtonRelease.ToInputAction().Enable();

            moveItemButton.ToInputAction().performed += StartMovement; // includes variants
            singleItemButton.ToInputAction().performed += StartPartialMovement;

            moveItemButtonRelease.ToInputAction().performed += EndDrag;
            moveItemButtonRelease.ToInputAction().canceled += CancelDrag;
        }

        private void OnDisable()
        {
            moveItemButton.ToInputAction().Disable();
            singleItemButton.ToInputAction().Disable();
            moveItemButtonRelease.ToInputAction().Disable();
        }

        private void StartMovement(InputAction.CallbackContext context)
        {
            if (transferItemButton.ToInputAction().IsPressed())
            {
                StartSendToOtherInventoryMovement(context);
            }
            else
            {
                if (timer != null && timer.IsRunning)
                {
                    var moved = StartCollectAllMovement(context);

                    if (!moved) { StartNormalMovement(context); }
                }
                else
                {
                    timer = new Timer(doubleClickTime);

                    StartNormalMovement(context);
                }
            }
        }

        private void StartSendToOtherInventoryMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    source.GetItem() != null &&
                    (useClick || useDragging))
                {
                    var targetInventory = inventoryUIController.GetInventoriesInGui().FirstOrDefault(
                            otherInv => otherInv.GetInventory().GetItemsContained(source.GetItem()) > 0
                            && otherInv.MaxAcceptable(source.GetItem()) > 0);
                    targetInventory ??= inventoryUIController.GetInventoriesInGui().FirstOrDefault(
                            otherInv => otherInv.MaxAcceptable(source.GetItem()) > 0);

                    if (targetInventory == null) { return; }

                    var itemsMoved = MoveItem<BaseItem>.MoveTo(source, targetInventory);
                    mostRecentSource = source;
                    if (itemsMoved > 0) { break; }
                }
            }
        }

        private void StartNormalMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(cursorInventory, destination);
                    mostRecentDestination = destination;
                    if (itemsMoved > 0) { break; }
                }

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(source, cursorInventory);
                    mostRecentSource = source;
                    if (itemsMoved > 0) { break; }
                }
            }
        }

        private bool StartCollectAllMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    ReferenceEquals(source, mostRecentSource) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = cursorInventory.GetItem() != null
                        ? MoveItem<BaseItem>.MoveAllFromInventoryTo(source, cursorInventory, cursorInventory.GetItem())
                        : MoveItem<BaseItem>.MoveAllFromInventoryTo(source, cursorInventory);

                    mostRecentSource = source;
                    if (itemsMoved > 0) { return true; }
                }
                else if (hitObject.TryGetComponent<IItemContainer<BaseItem>>(out var container) &&
                    ReferenceEquals(container, mostRecentDestination) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = cursorInventory.GetItem() != null
                        ? MoveItem<BaseItem>.MoveAllFromInventoryTo(source, cursorInventory, cursorInventory.GetItem())
                        : MoveItem<BaseItem>.MoveAllFromInventoryTo(source, cursorInventory);

                    mostRecentSource = source;
                    if (itemsMoved > 0) { return true; }
                }
            }
            return false;
        }

        private void StartPartialMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(cursorInventory, destination, 1);
                    mostRecentDestination = destination;
                    if (itemsMoved > 0) { break; }
                }

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(source, cursorInventory, Mathf.RoundToInt(source.GetNumber() / 2f + 0.1f));
                    mostRecentSource = source;
                    if (itemsMoved > 0) { break; }
                }
            }
        }

        private void EndDrag(InputAction.CallbackContext context)
        {
            if (!useDragging) { return; }

            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination))
                {
                    // Idea 1: If mostrecentsource contains items, and destination contains items, put our items back instead

                    // Idea 2: If the destination can't take my items,
                    // and my most recent source can't take the destination's items,
                    // I won't be able to swap the items, so I'll abort
                    if (destination.MaxAcceptable(cursorInventory.GetItem()) < cursorInventory.GetNumber() &&
                        (mostRecentSource is not IItemContainer<BaseItem> mostRecentSourceContainer ||
                        (mostRecentSourceContainer.MaxAcceptable((destination as IItemContainer<BaseItem>).GetItem()) <= 0)))
                    {
                        continue;
                    }
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(cursorInventory, destination);

                    mostRecentDestination = destination;
                    if (itemsMoved > 0) { break; }
                }
            }

            // If we are dragging from somewhere and we are still holding items
            if (mostRecentSource != null && cursorInventory.GetNumber() > 0)
            {
                if (mostRecentSource is IItemContainer<BaseItem> mostRecentSourceContainer)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(cursorInventory, mostRecentSourceContainer);
                    if (itemsMoved <= 0)
                    {
                        Debug.Log($"Drag ended but {cursorInventory.GetNumber()} items could not be returned to source container.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Drag ended but {cursorInventory.GetNumber()} items could not be returned to source, as it is not a container.");
                }
                mostRecentSource = null;
            }
        }

        private void CancelDrag(InputAction.CallbackContext context)
        {
            if (useClick || !useDragging) { return; }
            if (cursorInventory.GetNumber() <= 0) { return; }

            if (mostRecentSource is IItemContainer<BaseItem> mostRecentSourceContainer)
            {
                MoveItem<BaseItem>.MoveTo(cursorInventory, mostRecentSourceContainer);
                mostRecentDestination = mostRecentSourceContainer;
            }
            else
            {
                Debug.LogWarning("Drag canceled but items could not be returned to source.");
            }
        }

        private List<RaycastResult> RaycastMouse()
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);
            
            return results;
        }
    }
}
