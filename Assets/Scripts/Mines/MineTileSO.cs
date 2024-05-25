using UnityEngine;
using UnityEngine.Tilemaps;
using SDVA.InventorySystem;

namespace SDVA.Mines
{
    [CreateAssetMenu(fileName = "MineTileSO", menuName = "SDVA/MineTileSO")]
    public class MineTileSO : ScriptableObject
    {
        public BaseItem droppedItem;
        public TileBase tile;
        public string[] correctToolTypes;
        public int minimumToolLevel;
        public int tileHealth;
    }
}