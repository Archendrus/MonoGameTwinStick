﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    // TileMap class
    // contains array for map
    // 0 = ground tile
    // 1 = solid wall tile
    // Creates array of tiles based on map
    // Draws all tiles to the screen
    // CheckTileCollisions(collisionRect) can get a list tiles
    // potentially colliding with collisionRect
    class TileMap
    {
        public static int MAP_HEIGHT = 15;  // Height of map in tiles
        public static int MAP_WIDTH = 25;  // Width of map in tiles
        private int[,] map;  // map array
        
        private Tile[,] tiles;  // array of tiles
        private Texture2D tileSheet;  // Texture containing all tiles images
        private bool isSolid;  // set if solid tile (=1)
        Vector2 scale;


        // Create tilemap, using tiles from tilesheet, at scale
        public TileMap(Texture2D tileSheet, Vector2 scale)
        {
            this.tileSheet = tileSheet;
            this.scale = scale;

            map = new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1}
            };

            LoadTiles();
        }

        // Loop through map array, create array of tiles
        private void LoadTiles()
        {
            // Create array of tiles at map size
            tiles = new Tile[MAP_HEIGHT, MAP_WIDTH];
            // Loop through map
            for (int i = 0; i < MAP_HEIGHT; i++)
            {
                for (int j = 0; j < MAP_WIDTH; j++)
                {
                    //  get tile type at current map pos
                    int tileId = map[i, j];

                    // calculate position from map to pixel coordinates
                    Vector2 position = new Vector2(j * Tile.Width, i * Tile.Height);

                    // Set solid tile
                    if (tileId > 0)
                    {
                        isSolid = true;
                    }
                    else
                    {
                        isSolid = false;
                    }

                    // Create new tile
                    Tile newTile = new Tile(new Rectangle(tileId * 16, 0, 16, 16),
                                       position, scale, isSolid);

                    // Add new tile to tiles array
                    tiles[i, j] = newTile;
                }
            }
        }


        // Draw all tiles in tile array
        public void Draw(SpriteBatch spriteBatch)
        {
            // Loop through tile array and draw
            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    // draw SourceRectangle from tileSheet at Position
                    spriteBatch.Draw(
                        tileSheet,
                        new Vector2(tile.Position.X, tile.Position.Y),
                        tile.SourceRectangle,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        0.0f);
                }
            }
        }


        // Get the tile with upper left coord at x,y
        public Tile GetTile(int x, int y)
        {
            Tile tile;
            // prevent from accessing tiles out of bounds
            x = MathHelper.Clamp(x, 0, MAP_WIDTH - 1);
            y = MathHelper.Clamp(y, 0, MAP_HEIGHT - 1);
            
            tile = tiles[y, x];

            return tile;
        }

        // Get a list of tiles collisionRect may be colliding with 
        public List<Tile> CheckTileCollsions(Rectangle collisionRect)
        {
            List<Tile> tiles = new List<Tile>();

            int leftTile = (int)Math.Floor((float)collisionRect.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)collisionRect.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)collisionRect.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)collisionRect.Bottom / Tile.Height)) - 1;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // add tile to list to be returned
                    Tile tile = GetTile(x, y);
                    tiles.Add(tile);
                }
            }

            return tiles;
        }
    }
}
