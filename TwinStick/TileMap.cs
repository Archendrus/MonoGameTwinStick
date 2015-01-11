using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class TileMap
    {
        const int MAP_HEIGHT = 15;
        const int MAP_WIDTH = 25;
        private int[,] map;
        private Tile newTile;
        private Tile[,] tiles;
        private Texture2D tileSheet;
        private bool isSolid;

        public TileMap(Texture2D tileSheet)
        {
            this.tileSheet = tileSheet;

            map = new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1}
            };

            LoadTiles();
        }

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
                    Vector2 position = new Vector2(j * Tile.WIDTH, i * Tile.HEIGHT);

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
                    newTile = new Tile(new Rectangle(tileId * 16, 0, 16, 16),
                                       position, isSolid);

                    // Add new tile to tiles array
                    tiles[i, j] = newTile;
                }
            }
        }

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
                        Game1.Scale,
                        SpriteEffects.None,
                        0.0f);
                }

            }
        }

        public Tile GetTile(int x, int y)
        {
            Tile tile;
            // prevent from accessing tiles out of bounds
            x = MathHelper.Clamp(x, 0, MAP_WIDTH - 1);
            y = MathHelper.Clamp(y, 0, MAP_HEIGHT - 1);
            
            tile = tiles[y, x];

            return tile;
        }
    }
}
