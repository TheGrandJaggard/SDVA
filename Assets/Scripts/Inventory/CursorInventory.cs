using System.Collections.Generic;
using SDVA.Utils.UI.ItemMovement;
using SDVA.InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SDVA.UI.InventorySystem
{
    public class CursorInventory : MonoBehaviour, IItemContainer<BaseItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon;

        [Header("Item Movement Settings:")]
        [Tooltip("Whether items can be picked up by clicking.")]
        [SerializeField] bool useClick;
        [Tooltip("Whether items are dropped after a drag ends.\n(Not recommended, but makes for a simpler system when other settings are turned off.)")]
        [SerializeField] bool useDragging;
        [Tooltip("Whether items should be spread out between slots and aggregated from slots that they are dragged over. (useDragging must be false for this to function properly) (WIP)")]
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
        [Tooltip("The input that signals a release of items dragged.\n(Default = LeftRightClickHeld)")]
        [SerializeField] InputActionReference moveItemButtonRelease;
        
        // STATE

        private int slot;
        private Inventory inventory;
        private IItemSource<BaseItem> mostRecentSource; // Used for dragging returns

        // PUBLIC

        public int MaxAcceptable(BaseItem item) => item != null ? item.GetMaxStackSize() : 0;

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);
        
        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);

        public List<IItemSource<BaseItem>> GetRelatedSources() => new();
        
        // PRIVATE

        private void Awake()
        {
            inventory = new Inventory(1);
            slot = 0;

            inventory.InventoryUpdated += Redraw;
            Redraw();
        }

        private void OnEnable()
        {
            moveItemButton.ToInputAction().Enable();
            singleItemButton.ToInputAction().Enable();
            pickupAllItemsButton.ToInputAction().Enable();
            moveItemButtonRelease.ToInputAction().Enable();

            moveItemButton.ToInputAction().performed += StartMovement; // shift variant
            singleItemButton.ToInputAction().performed += StartSingleMovement;
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
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(this, destination);
                    if (itemsMoved > 0) { break; }
                }

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(source, this);
                    mostRecentSource = source;
                    if (itemsMoved > 0) { break; }
                }
            }
        }

        private void StartSingleMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(this, destination, 1);
                    if (itemsMoved > 0) { break; }
                }

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveTo(source, this, source.GetNumber() / 2);
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
                    var itemsMoved = MoveItem<BaseItem>.MoveAllFromInventoryTo(source, this);
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
                    MoveItem<BaseItem>.MoveBetween(this, destination);
                    break;
                }
            }

            // If we are dragging from somewhere and we are still holding items
            if (mostRecentSource != null && GetNumber() > 0)
            {
                if (mostRecentSource is IItemContainer<BaseItem> mostRecentSourceContainer)
                {
                    MoveItem<BaseItem>.MoveTo(this, mostRecentSourceContainer);
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
                MoveItem<BaseItem>.MoveTo(this, mostRecentSourceContainer);
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

        private void Redraw()
        {
            if (inventory.GetSlotItem(slot) == null)
            {
                icon.GetComponent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                icon.GetComponent<CanvasGroup>().alpha = 1f;
                icon.SetItem(inventory.GetSlotItem(slot), inventory.GetSlotNumber(slot));
            }
        }
    }
}