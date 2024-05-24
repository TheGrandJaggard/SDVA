using UnityEngine;
using TMPro;
using SDVA.InventorySystem;
using SDVA.Utils.UI;

namespace SDVA.UI.InventorySystem
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : Tooltip
    {
        // CONFIG DATA
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI typeText = null;
        [SerializeField] TextMeshProUGUI descriptionText = null;

        // PUBLIC
        public void SetupContent(BaseItem item)
        {
            titleText.text = item.GetDisplayName();
            descriptionText.text = item.GetDescription();
            typeText.text = item.GetItemType();
        }
    }
}
