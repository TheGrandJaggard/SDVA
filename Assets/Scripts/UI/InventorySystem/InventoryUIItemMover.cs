using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using SDVA.Utils;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;
using System.Collections.Generic;

namespace SDVA.UI.InventorySystem
{
    public class InventoryUIItemMover : MonoBehaviour
    {
        [SerializeField] CursorInventory cursorInventory;
        
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
        [Tooltip("The input that picks up all items of type in inventory.\n(Default = DoubleClick)")]
        [SerializeField] InputActionReference pickupAllItemsButton;
        [Tooltip("The input that changes click functionality to transfer between containers.\n(Default = Shift) (WIP)")]
        [SerializeField] InputActionReference transferItemButton;
        [Tooltip("The input that opens a custom menu for the item clicked item.\n(Default = LeftRightClick) (WIP)")]
        [SerializeField] InputActionReference openActionMenuButton;
        [Tooltip("The input that signals a release of items dragged.\n(Default = LeftRightClickHeld)")]
        [SerializeField] InputActionReference moveItemButtonRelease;

        private IItemSource<BaseItem> mostRecentSource; // Used for dragging returns
        private CardinalDirections placementFacing;

        private void OnEnable()
        {
            moveItemButton.ToInputAction().Enable();
            singleItemButton.ToInputAction().Enable();
            pickupAllItemsButton.ToInputAction().Enable();
            moveItemButtonRelease.ToInputAction().Enable();

            moveItemButton.ToInputAction().performed += StartMovement; // shift variant
            singleItemButton.ToInputAction().performed += StartPartialMovement;
            pickupAllItemsButton.ToInputAction().performed += StartCollectAllMovement; // shift variant

            moveItemButtonRelease.ToInputAction().performed += EndDrag;
            moveItemButtonRelease.ToInputAction().canceled += CancelDrag;
        }

        private void OnDisable()
        {
            moveItemButton.ToInputAction().Disable();
            singleItemButton.ToInputAction().Disable();
            pickupAllItemsButton.ToInputAction().Disable();
            moveItemButtonRelease.ToInputAction().Disable();
        }

        private void StartMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(cursorInventory, destination);
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

        private void StartPartialMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(cursorInventory, destination, 1);
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
        
        private void StartCollectAllMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveAllFromInventoryTo(source, cursorInventory);
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
                    MoveItem<BaseItem>.MoveBetween(cursorInventory, destination);
                    break;
                }
            }

            // If we are dragging from somewhere and we are still holding items
            if (mostRecentSource != null && cursorInventory.GetNumber() > 0)
            {
                if (mostRecentSource is IItemContainer<BaseItem> mostRecentSourceContainer)
                {
                    MoveItem<BaseItem>.MoveTo(cursorInventory, mostRecentSourceContainer);
                }
                else
                {
                    Debug.LogWarning("Items destroyed because they could not be placed back where they came from after drag.");
                }
                mostRecentSource = null;
            }
        }

        private void CancelDrag(InputAction.CallbackContext context)
        {
            if (useClick || !useDragging) { return; }
            if (mostRecentSource is IItemContainer<BaseItem> mostRecentSourceContainer)
            {
                MoveItem<BaseItem>.MoveTo(cursorInventory, mostRecentSourceContainer);
            }
            else
            {
                Debug.LogWarning("Items destroyed because they could not be placed back where they came from after drag.");
            }
            mostRecentSource = null;
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