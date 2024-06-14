using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SDVA.InventorySystem;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] GameObject textContainer = null;
        [SerializeField] TextMeshProUGUI itemNumber = null;

        // PUBLIC

        public void SetItem(BaseItem item, int number = 1)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }

            if (itemNumber != null)
            {
                textContainer?.SetActive(number > 0);
                itemNumber.text = number.ToString();
            }
        }
    }
}