using UnityEngine;
using UnityEngine.Tilemaps;

namespace SDVA.Mines
{
    [CreateAssetMenu(fileName = "MineTileSO", menuName = "SDVA/MineTileSO")]
    public class MineTileSO : ScriptableObject
    {
        public string droppedItem;
        public TileBase tile;
        public string[] correctToolTypes;
        public int minimumToolLevel;
        public int tileHealth;
    }
}