using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SDVA.Mines
{
    public class MineController : MonoBehaviour
    {
        [Serializable] public struct TileSpawn
        {
            public int changeY;
            public float sharpness;
            public MineTileSO mineTileSO;
        }

        [Serializable] public struct OreSpawn
        {
            public int startY;
            public int endY;
            public float sharpness;
            public float abundance;
            public MineTileSO mineTileSO;
        }
        
        private struct DamagedTile
        {
            public Vector3Int pos;
            public GameObject crackDisplay;
            public float timeSinceMined;
            public float tileHealthRemaining;
            public float tileMaxHealth;
        }
        
        [SerializeField] MineTileSO blankMineTileSO;
        [SerializeField] TileSpawn[] tileSpawns;
        [Tooltip("Items at the top of the list will override items at the bottom")]
        [SerializeField] OreSpawn[] oreSpawns;
        
        [SerializeField] Sprite[] crackSprites;
        private List<DamagedTile> damagedTiles = new();

        
        public Tilemap foreground;
        public Tilemap foregroundOverlay;
        public Tilemap background;
        public Tilemap backgroundOverlay;

        private void Start() {
            GetComponent<MineGenerator>().GenerateMines(20, -20, 20, -20, oreSpawns, tileSpawns);
        }

        public void MineTile(Vector3Int tilePos, string toolType, int toolLevel, float mineSpeed)
        {
            TileSpawn[] tiles = tileSpawns.Where(t => t.mineTileSO.tile == foreground.GetTile(tilePos)).ToArray();
            OreSpawn[] ores = oreSpawns.Where(o => o.mineTileSO.tile == foregroundOverlay.GetTile(tilePos)).ToArray();

            TileSpawn tile = tiles.Length != 0 ? tiles.First():
                new TileSpawn() { mineTileSO = blankMineTileSO };
            OreSpawn ore = ores.Length != 0 ? ores.First():
                new OreSpawn() { mineTileSO = blankMineTileSO };
            
            string[] correctToolTypes = tile.mineTileSO.correctToolTypes.Intersect(ore.mineTileSO.correctToolTypes).ToArray();
            if (!correctToolTypes.Contains(toolType)) { return; }

            int minimumToolLevel = Mathf.Max(tile.mineTileSO.minimumToolLevel, ore.mineTileSO.minimumToolLevel);
            if (toolLevel < minimumToolLevel) { return; }

            if (foreground.GetTile(tilePos) == null) { return; }

            DamagedTile[] matchingDamagedTiles = damagedTiles.Where(d => d.pos == tilePos).ToArray();
            DamagedTile damagedTile;
            if (matchingDamagedTiles.Length == 0)
            {
                GameObject crackObject = new("Crack", typeof(SpriteRenderer));
                crackObject.transform.position = new(tilePos.x + 0.5f, tilePos.y + 0.5f, 0.8f);

                damagedTile = new() { pos = tilePos, tileMaxHealth = tile.mineTileSO.tileHealth + ore.mineTileSO.tileHealth, tileHealthRemaining = tile.mineTileSO.tileHealth + ore.mineTileSO.tileHealth, crackDisplay = crackObject };
                damagedTiles.Add(damagedTile);
            }
            else
            {
                damagedTile = matchingDamagedTiles.First();
            }

            int damagedTileIndex = damagedTiles.IndexOf(damagedTile);
            damagedTile.timeSinceMined = 0f;
            damagedTile.tileHealthRemaining -= mineSpeed;
            damagedTiles[damagedTileIndex] = damagedTile;

            if (damagedTile.tileHealthRemaining < 0)
            {
                foreground.SetTile(damagedTile.pos, null);
                foregroundOverlay.SetTile(damagedTile.pos, null);
                Destroy(damagedTile.crackDisplay);
                damagedTiles.Remove(damagedTile);
            }
            else
            {
                UpdateCracks(damagedTile);
            }
        }

        private void UpdateCracks(DamagedTile damagedTile)
        {
            int spriteNum = Mathf.FloorToInt(crackSprites.Length - crackSprites.Length * damagedTile.tileHealthRemaining / damagedTile.tileMaxHealth);
            Sprite crackSprite = crackSprites[Mathf.Clamp(spriteNum, 0, crackSprites.Length-1)];
            damagedTile.crackDisplay.GetComponent<SpriteRenderer>().sprite = crackSprite;
        }

        private void Update()
        {
            ManageTileBreakage();
        }

        private void ManageTileBreakage()
        {
            for (int i = 0; i < damagedTiles.Count; i++)
            {
                DamagedTile damagedTile = damagedTiles[i];
                if (damagedTile.timeSinceMined > 1f)
                {
                    damagedTile.tileHealthRemaining += Time.deltaTime;
                    damagedTiles[i] = damagedTile;
                    UpdateCracks(damagedTile);

                    if (damagedTile.tileHealthRemaining > damagedTile.tileMaxHealth)
                    {
                        Destroy(damagedTile.crackDisplay);
                        damagedTiles.Remove(damagedTile);
                    }
                }
                else
                {
                    damagedTile.timeSinceMined += Time.deltaTime;
                    damagedTiles[i] = damagedTile;
                }
            }
        }
    }
}