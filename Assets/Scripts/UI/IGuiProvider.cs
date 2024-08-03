using UnityEngine;

namespace SDVA.UI.InventorySystem
{
    public interface IGuiProvider
    {
        /// <summary>
        /// Instansiates a peice of GUI, sets it up, saves it, and then returns it.
        /// </summary>
        /// <param name="parent">The parent of the GUI to be instantiated.</param>
        public GameObject SetupGui(Transform parent);
    }
}