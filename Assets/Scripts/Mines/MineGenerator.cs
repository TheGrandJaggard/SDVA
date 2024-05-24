using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SDVA.Mines
{
    [RequireComponent(typeof(MineController))]
    public class MineGenerator : MonoBehaviour
    {
        private MineController mineController;
        
        public void GenerateMines(int top, int bottom, int right, int left, MineController.OreSpawn[] oreSpawns, MineController.TileSpawn[] tileSpawns)
        {
            Dictionary<Vector3Int, TileBase> baseTiles = new();
            Dictionary<Vector3Int, TileBase> oreTiles = RandomOrePatches(top, bottom, right, left, oreSpawns);//new();
            mineController = GetComponent<MineController>();
            for (int i = bottom; i < top; i++)
            {
                for (int j = left; j < right; j++)
                {
                    baseTiles[new(i, j, 0)] = GetTile(tileSpawns, new(i, j));
                    
                    // oreTiles[new(i, j, 0)] = SpawnOre(oreSpawns, new(i, j));
                }
            }

            mineController.foreground.SetTiles(baseTiles.Keys.ToArray(), baseTiles.Values.ToArray());
            mineController.background.SetTiles(baseTiles.Keys.ToArray(), baseTiles.Values.ToArray());
            mineController.foregroundOverlay.SetTiles(oreTiles.Keys.ToArray(), oreTiles.Values.ToArray());
            mineController.backgroundOverlay.SetTiles(oreTiles.Keys.ToArray(), oreTiles.Values.ToArray());
        }

        TileBase SpawnOre(MineController.OreSpawn[] oreSpawns, Vector2 pos)
        {
            foreach (MineController.OreSpawn oreSpawn in oreSpawns)
            {
                float randomNum = UnityEngine.Random.value;

                float oreChance = oreSpawn.abundance
                    - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.startY - pos.y) * oreSpawn.sharpness))
                    - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.endY - pos.y) * -oreSpawn.sharpness));

                bool spawnOre = oreChance > randomNum; // this decides wether by chance we round up or down
                
                // Debug.Log("pos: " + pos + ", randomNum: " + randomNum + ", oreChance: " + oreChance + ", spawnOre: " + spawnOre + ", tile: " + oreSpawn.tile.name);
                if (spawnOre) { return oreSpawn.mineTileSO.tile; }
            }

            return null;
        }

        TileBase GetTile(MineController.TileSpawn[] stages, Vector2 pos)
        {
            float randomNum = UnityEngine.Random.value;
            float sigSum = 0;

            for (int i = 0; i < stages.Length -1; i++)
            {
                var stage = stages[i];
                sigSum += 1 / (1 + Mathf.Exp((stage.changeY + pos.y) * stage.sharpness));
            }

            int tier = Mathf.FloorToInt(sigSum); // first tier will be zero, which is dirt
            bool randomUp = sigSum > randomNum + tier; // this decides wether by chance we round up or down
            tier += Convert.ToInt32(randomUp);

            TileBase tile = stages[tier].mineTileSO.tile;

            // Debug.Log("randomNum: " + randomNum + ", sigSum: " + sigSum + ", tier: " + tier + ", tile: " + tile.name);

            return tile;
        }

        Dictionary<Vector3Int, TileBase> RandomOrePatches(int top, int bottom, int right, int left, MineController.OreSpawn[] oreSpawns)
        {
            Dictionary<Vector3Int, TileBase> oreTiles = new();
            foreach (var oreSpawn in oreSpawns)
            {
                int numPatches = UnityEngine.Random.Range(4, 6);
                // Debug.Log("Generating " + numPatches + " patches of " + oreSpawn.tile.name);
                
                for (int patchNum = 0; patchNum < numPatches; patchNum++)
                {
                    Dictionary<float, Vector3Int> weightedPositions = new();
                    for (int z = 0; z < 10; z++)
                    {
                        Vector3Int pos = new(UnityEngine.Random.Range(left, right), UnityEngine.Random.Range(bottom, top), 0);
                        
                        float oreChance = oreSpawn.abundance
                            - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.startY - pos.y) * oreSpawn.sharpness))
                            - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.endY - pos.y) * -oreSpawn.sharpness));
                        
                        weightedPositions[oreChance * UnityEngine.Random.value] = pos;
                    }
                    Vector3Int startPosition = weightedPositions[weightedPositions.Keys.Max()];
                    // Debug.Log("From weighted positions:  " + weightedPositions + " we chose " + startPosition);
                    oreTiles[startPosition] = oreSpawn.mineTileSO.tile;

                    int patchQuality = UnityEngine.Random.Range(1, 10);

                    for (int tileNum = 1; tileNum < patchQuality; tileNum++)
                    {
                        Vector3Int position = startPosition;
                        while (oreTiles.Keys.Contains(position))
                        {
                            position.x += Mathf.RoundToInt(UnityEngine.Random.onUnitSphere.x);
                            position.y += Mathf.RoundToInt(UnityEngine.Random.onUnitSphere.y);
                            // Debug.Log("Generating " + oreSpawn.tile.name + " at " + position);
                        }
                        oreTiles[position] = oreSpawn.mineTileSO.tile;
                    }
                }
            }
            return oreTiles;

            // foreach (MineController.OreSpawn oreSpawn in oreSpawns)
            // {
            //     float randomNum = UnityEngine.Random.value;

            //     float oreChance = oreSpawn.abundance
            //         - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.startY - pos.y) * oreSpawn.sharpness))
            //         - oreSpawn.abundance / (1 + Mathf.Exp((oreSpawn.endY - pos.y) * -oreSpawn.sharpness));

            //     bool spawnOre = oreChance > randomNum; // this decides wether by chance we round up or down
                
            //     // Debug.Log("pos: " + pos + ", randomNum: " + randomNum + ", oreChance: " + oreChance + ", spawnOre: " + spawnOre + ", tile: " + oreSpawn.tile.name);
            //     if (spawnOre) { return oreSpawn.tile; }
            // }
            // 
            // return null;
        }
    }
}