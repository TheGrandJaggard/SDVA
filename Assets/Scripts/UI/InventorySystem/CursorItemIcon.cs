using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    public class CursorItemIcon : InventoryItemIcon
    {
        private void Update()
        {
            transform.position = Input.mousePosition;
        }
    }
}