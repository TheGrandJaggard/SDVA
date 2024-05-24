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
                // Debug.Log("creating new tile with breaktime of: " + damagedTile.tileHealthRemaining);
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
            // Debug.Log("tile now has breaktime of: " + damagedTile.tileHealthRemaining);

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
            // Debug.Log("damage dealt to tile: " + damagedTile.pos + ", time till broken: " + damagedTile.tileHealthRemaining);
        }

        private void UpdateCracks(DamagedTile damagedTile)
        {
            int spriteNum = Mathf.FloorToInt(crackSprites.Length - crackSprites.Length * damagedTile.tileHealthRemaining / damagedTile.tileMaxHealth);
            Sprite crackSprite = crackSprites[Mathf.Clamp(spriteNum, 0, crackSprites.Length-1)];
            damagedTile.crackDisplay.GetComponent<SpriteRenderer>().sprite = crackSprite;
            
            // Debug.Log("crackSprites.Length=" + crackSprites.Length);
            // Debug.Log("damagedTile.tileMaxHealth=" + damagedTile.tileMaxHealth);
            // Debug.Log("damagedTile.tileHealthRemaining=" + damagedTile.tileHealthRemaining);
            // Debug.Log("crackSprites.Length * damagedTile.tileHealthRemaining / damagedTile.tileMaxHealth=" + crackSprites.Length * damagedTile.tileHealthRemaining / damagedTile.tileMaxHealth);
            // Debug.Log("Mathf.FloorToInt(crackSprites.Length * damagedTile.tileHealthRemaining / damagedTile.tileMaxHealth) (spriteNum)=" + spriteNum);
            // Debug.Log("Mathf.Clamp(spriteNum, 0, crackSprites.Length-1)=" + Mathf.Clamp(spriteNum, 0, crackSprites.Length-1));
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
                    // Debug.Log("health regenning. health: " + damagedTile.tileHealthRemaining + "/" + damagedTile.tileMaxHealth);
                    damagedTile.tileHealthRemaining += Time.deltaTime;
                    damagedTiles[i] = damagedTile;
                    UpdateCracks(damagedTile);

                    if (damagedTile.tileHealthRemaining > damagedTile.tileMaxHealth)
                    {
                        // Debug.Log("removing crack");
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

            // // Debug.Log("mining tile: " + miningTile);
            // List<DamagedTile> newDamagedTiles = new();
            // for (int i = 0; i < damagedTiles.Count; i++)
            // {
            //     DamagedTile damagedTile = damagedTiles[i];
            //     TileSpawn[] tiles = tileSpawns.Where(t => t.tile == foreground.GetTile(damagedTile.pos)).ToArray();
            //     OreSpawn[] ores = oreSpawns.Where(o => o.tile == foregroundOverlay.GetTile(damagedTile.pos)).ToArray();
            //     TileSpawn tile = tiles.Length != 0 ? tiles.First():
            //         new TileSpawn() { correctToolTypes = new[] {"Drill"}, breakTime = 0, minimumToolLevel = 0};
            //     OreSpawn ore = ores.Length != 0 ? ores.First():
            //         new OreSpawn() { correctToolTypes = new[] {"Drill"}, additionalBreakTime = 0, minimumToolLevel = 0};

            //     if (damagedTile.pos == miningTile)
            //     {
            //         damagedTile.timeTillBroken -= miningSpeed * Time.deltaTime;
            //         Debug.Log("damage dealt to tile: " + damagedTile.pos + ", time till broken: " + damagedTile.timeTillBroken);
            //     }
            //     else
            //     {
            //         damagedTile.timeTillBroken += 0.01f;
            //         Debug.Log("tile healed: " + damagedTile.pos + ", time till broken: " + damagedTile.timeTillBroken);
            //     }

            //     int spriteNum = Mathf.FloorToInt(crackSprites.Length * damagedTile.timeTillBroken / (tile.breakTime + ore.additionalBreakTime));
            //     Sprite crackSprite = crackSprites[Mathf.Clamp(spriteNum - 1, 0, crackSprites.Length)];
            //     damagedTile.crackDisplay.GetComponent<SpriteRenderer>().sprite = crackSprite;

            //     if (damagedTile.timeTillBroken < 0)
            //     {
            //         Destroy(damagedTile.crackDisplay);
            //         foreground.SetTile(damagedTile.pos, null);
            //         foregroundOverlay.SetTile(damagedTile.pos, null);
            //         Debug.Log("Tile Destroyed: " + damagedTile.pos);
            //     }
            //     else if (damagedTile.timeTillBroken > (tile.breakTime + ore.additionalBreakTime))
            //     {
            //         Destroy(damagedTile.crackDisplay);
            //         Debug.Log("Tile Restored: " + damagedTile.pos);
            //     }
            //     else
            //     {
            //         newDamagedTiles.Add(damagedTile);
            //     }
            // }
            // damagedTiles = newDamagedTiles;
        }
    }
}