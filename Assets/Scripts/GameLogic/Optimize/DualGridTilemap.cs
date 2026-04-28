using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameLogic.Optimize
{
    [ExecuteAlways]
    public class DualGridTilemap : MonoBehaviour
    {
        public static Action<Vector3Int> OnTileChanged;
        protected static readonly Vector3Int[] Neighbours = {
            new(0, 0, 0),
            new(1, 0, 0),
            new(0, 1, 0),
            new(1, 1, 0)
        };

        protected static Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> NeighbourTupleToTile;
        
        // Provide references to each tilemap in the inspector
        public Tilemap placeholderTilemap;
        public Tilemap displayTilemap;

        // Provide the dirt and grass placeholder tiles in the inspector
        public RuleTile placeholderTile;
        
        // Provide the 16 tiles in the inspector
        public Tile[] tiles;

        private void Awake()
        {
            // This dictionary stores the "rules", each 4-neighbour configuration corresponds to a tile
            // |_1_|_2_|
            // |_3_|_4_|
            NeighbourTupleToTile = new() {
                {new (TileType.Tile, TileType.Tile, TileType.Tile, TileType.Tile), tiles[6]},
                {new (TileType.None, TileType.None, TileType.None, TileType.Tile), tiles[13]}, // OUTER_BOTTOM_RIGHT
                {new (TileType.None, TileType.None, TileType.Tile, TileType.None), tiles[0]}, // OUTER_BOTTOM_LEFT
                {new (TileType.None, TileType.Tile, TileType.None, TileType.None), tiles[8]}, // OUTER_TOP_RIGHT
                {new (TileType.Tile, TileType.None, TileType.None, TileType.None), tiles[15]}, // OUTER_TOP_LEFT
                {new (TileType.None, TileType.Tile, TileType.None, TileType.Tile), tiles[1]}, // EDGE_RIGHT
                {new (TileType.Tile, TileType.None, TileType.Tile, TileType.None), tiles[11]}, // EDGE_LEFT
                {new (TileType.None, TileType.None, TileType.Tile, TileType.Tile), tiles[3]}, // EDGE_BOTTOM
                {new (TileType.Tile, TileType.Tile, TileType.None, TileType.None), tiles[9]}, // EDGE_TOP
                {new (TileType.None, TileType.Tile, TileType.Tile, TileType.Tile), tiles[5]}, // INNER_BOTTOM_RIGHT
                {new (TileType.Tile, TileType.None, TileType.Tile, TileType.Tile), tiles[2]}, // INNER_BOTTOM_LEFT
                {new (TileType.Tile, TileType.Tile, TileType.None, TileType.Tile), tiles[10]}, // INNER_TOP_RIGHT
                {new (TileType.Tile, TileType.Tile, TileType.Tile, TileType.None), tiles[7]}, // INNER_TOP_LEFT
                {new (TileType.None, TileType.Tile, TileType.Tile, TileType.None), tiles[14]}, // DUAL_UP_RIGHT
                {new (TileType.Tile, TileType.None, TileType.None, TileType.Tile), tiles[4]}, // DUAL_DOWN_RIGHT
                {new (TileType.None, TileType.None, TileType.None, TileType.None), tiles[12]},
            };
            OnTileChanged = SetDisplayTile;
            Tilemap.tilemapTileChanged += (tilemap, syncTiles) =>
            {
                if (tilemap != placeholderTilemap) return;
                foreach (var syncTile in syncTiles) SetDisplayTile(syncTile.position);
            };
        }

        public void SetCell(Vector3Int coords, Tile tile) {
            placeholderTilemap.SetTile(coords, tile);
            SetDisplayTile(coords);
        }

        private TileType GetPlaceholderTileTypeAt(Vector3Int coords)
        {
            return placeholderTilemap.GetTile(coords) == placeholderTile ? TileType.Tile : TileType.None;
        }

        protected Tile CalculateDisplayTile(Vector3Int coords) {
            // 4 neighbours
            var topRight = GetPlaceholderTileTypeAt(coords - Neighbours[0]);
            var topLeft = GetPlaceholderTileTypeAt(coords - Neighbours[1]);
            var botRight = GetPlaceholderTileTypeAt(coords - Neighbours[2]);
            var botLeft = GetPlaceholderTileTypeAt(coords - Neighbours[3]);

            Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, botLeft, botRight);

            return NeighbourTupleToTile[neighbourTuple];
        }

        protected void SetDisplayTile(Vector3Int pos)
        {
            foreach (var n in Neighbours)
            {
                var newPos = pos + n;
                displayTilemap.SetTile(newPos, CalculateDisplayTile(newPos));
            }
        }
    }

    public enum TileType {
        None,
        Tile
    }
}