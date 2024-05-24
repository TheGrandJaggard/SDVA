// using SDVA.InventorySystem;
// using SDVA.Utils.UI.Dragging;
// using UnityEngine;

// namespace SDVA.UI.InventorySystem
// {
//     /// <summary>
//     /// The UI slot for the player action bar.
//     /// </summary>
//     public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<BaseItem>
//     {
//         // CONFIG DATA
//         [SerializeField] InventoryItemIcon icon = null;
//         [SerializeField] int index = 0;

//         // CACHE
//         ActionStore store;

//         // LIFECYCLE METHODS
//         private void Awake()
//         {
//             store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
//             store.StoreUpdated += UpdateIcon;
//         }

//         // PUBLIC

//         public void AddItems(BaseItem item, int number)
//         {
//             store.AddAction(item, index, number);
//         }

//         public BaseItem GetItem() => store.GetAction(index);

//         public int GetNumber() => store.GetNumber(index);

//         public int MaxAcceptable(BaseItem item) => store.MaxAcceptable(item, index);

//         public void RemoveItems(int number) => store.RemoveItems(index, number);

//         // PRIVATE

//         private void UpdateIcon()
//         {
//             icon.SetItem(GetItem(), GetNumber());
//         }
//     }
// }
