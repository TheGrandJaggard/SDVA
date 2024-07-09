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

        [SerializeField] bool useClick;
        [SerializeField] bool useDragging;

        private IItemSource<BaseItem> mostRecentSource; // Used for dragging returns

        // STATE
        private int slot;
        private Inventory inventory;
        private PlayerWorldInputActions playerControls;

        // PUBLIC

        public int MaxAcceptable(BaseItem item) => item != null ? item.GetMaxStackSize() : 0;

        public int AddItems(BaseItem item, int number) => inventory.AddToSlot(slot, item, number);

        public int GetNumber() => inventory.GetSlotNumber(slot);

        public BaseItem GetItem() => inventory.GetSlotItem(slot);
        
        public void RemoveItems(int number) => inventory.RemoveFromSlot(slot, number);
        
        // PRIVATE

        private void Awake()
        {
            inventory = new Inventory(1);
            slot = 0;

            playerControls = new PlayerWorldInputActions();

            inventory.InventoryUpdated += Redraw;
            Redraw();
        }

        private void OnEnable()
        {
            playerControls = new PlayerWorldInputActions();
            playerControls.Enable();
            playerControls.UI.Click.performed += StartMovement;
            playerControls.UI.ClickHold.performed += EndDrag;
            playerControls.UI.ClickHold.canceled += CancelDrag;
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void StartMovement(InputAction.CallbackContext context)
        {
            foreach (var hitResult in RaycastMouse())
            {
                var hitObject = hitResult.gameObject;

                if (hitObject.TryGetComponent<IItemSource<BaseItem>>(out var source) &&
                    (useClick || useDragging))
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(source, this);
                    mostRecentSource = source;
                    if (itemsMoved > 0) { break; }
                }

                if (hitObject.TryGetComponent<IItemDestination<BaseItem>>(out var destination) &&
                    useClick)
                {
                    var itemsMoved = MoveItem<BaseItem>.MoveBetween(this, destination);
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