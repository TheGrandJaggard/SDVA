// using UnityEngine;
// using SDVA.Utils.UI.Dragging;
// using SDVA.InventorySystem;

// namespace SDVA.UI.InventorySystem
// {
//     /// <summary>
//     /// An slot for the players equipment.
//     /// </summary>
//     public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<BaseItem>
//     {
//         // CONFIG DATA

//         [SerializeField] InventoryItemIcon icon = null;
//         [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

//         // CACHE
//         Equipment playerEquipment;

//         // LIFECYCLE METHODS
       
//         private void Awake() 
//         {
//             var player = GameObject.FindGameObjectWithTag("Player");
//             playerEquipment = player.GetComponent<Equipment>();
//             playerEquipment.EquipmentUpdated += RedrawUI;
//         }

//         private void Start() 
//         {
//             RedrawUI();
//         }

//         // PUBLIC

//         public int MaxAcceptable(BaseItem item)
//         {
//             EquipableItem equipableItem = item as EquipableItem;
//             if (equipableItem == null) return 0;
//             if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
//             if (GetItem() != null) return 0;

//             return 1;
//         }

//         public void AddItems(BaseItem item, int number)
//         {
//             playerEquipment.AddItem(equipLocation, (EquipableItem) item);
//         }

//         public BaseItem GetItem()
//         {
//             return playerEquipment.GetItemInSlot(equipLocation);
//         }

//         public int GetNumber()
//         {
//             if (GetItem() != null)
//             {
//                 return 1;
//             }
//             else
//             {
//                 return 0;
//             }
//         }

//         public void RemoveItems(int number)
//         {
//             playerEquipment.RemoveItem(equipLocation);
//         }

//         // PRIVATE

//         void RedrawUI()
//         {
//             icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
//         }
//     }
// }