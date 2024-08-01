using UnityEngine;
using SDVA.Saving;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SDVA.InventorySystem
{
    /// <summary>
    /// This component should be placed on chests
    /// </summary>
    public class ChestInventoryComponent : InventoryComponent
    {
        // CONFIG DATA
        [SerializeField] GameObject UITemplate;

        public void OpenChest()
        {
            
        }
    }
}