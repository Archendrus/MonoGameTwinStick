using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwinStick
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        // Graphics/screen 
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;
        int VirtualWidth = 800;
        int VirtualHeight = 480;
        Vector2 Scale;     
        Rectangle screenRectangle;
        Rectangle virtualScreenRect;

        // Game states
        enum GameState { TitleScreen, Game, NextLevel, Controls };
        GameState currentState;
        int currentLevel;
        bool levelChanged;

        // Game objects
        TileMap tileMap;
        Player player;
        Victim victim;

        EnemyManager enemyManager;
        BulletManager bulletManager;

        // Input states
        KeyboardState key;
        GamePadState gamePad;

        // Texture for bullet
        Texture2D bulletTexture;
        Texture2D zombieTexture;
        Texture2D victimTexture;

        // Text and messages
        SpriteFont textFont;
        String message;
        List<String> controlMessages;
        Color messageColor;
        float messageTimerElapsed = 0;
        int currentControlMessage = 0;

        // Player/bullet update args
        Vector2 playerDirection;
        Vector2 shootDirection;


        // Random number generator for victim spawn
        Random random;

        // Score and surviors
        SpriteFont scoreFont;
        int score = 0;
        int totalVictims;
        int currentVictim;
        int saved;
        Sprite[] victims;
        Color[] victimColor;
        Color victimSave, victimKilled;
        RenderTarget2D victimBoardRenderTarget;
        Rectangle victimBoardRenderTargetRect;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 864;
            graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // Make fullscreen
            Window.IsBorderless = false;
            IsFixedTimeStep = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create screen rectangle at size of user's desktop resolution
            screenRectangle = new Rectangle(
                0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            virtualScreenRect = new Rectangle(0, 0, VirtualWidth, VirtualHeight);

            // Create a 2x scale
            Scale = new Vector2(2, 2);

            // Create render target at Virtual resolution
            renderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);

            // Create render target for drawing survior score board
            victimBoardRenderTarget = new RenderTarget2D(GraphicsDevice, 256, 32);
            // Create rectangle for draw position
            victimBoardRenderTargetRect = new Rectangle(512, 4, 256, 24);

            // Initialize values for messages
            messageColor = new Color(32, 92, 32);
            controlMessages = new List<String>();
            controlMessages.Add("WASD / LEFT STICK TO MOVE");
            controlMessages.Add("ARROWS / RIGHT STICK TO SHOOT");
            controlMessages.Add("SHOOT ZOMBIES!\nSAVE SURVIORS!");

            // Start on titlescreen
            currentState = GameState.TitleScreen;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Font and message rendering
            scoreFont = Content.Load<SpriteFont>("font");
            textFont = Content.Load<SpriteFont>("textFont");

            // initialize colors for tinting victim board sprites
            victimSave = new Color(92, 156, 92);
            victimKilled = new Color(208, 112, 112);
               
            // Temp texture to load sprites
            Texture2D temp;

            // Create tile map
            temp = Content.Load<Texture2D>("tiles");
            tileMap = new TileMap(temp, Scale);

            // Create player
            temp = Content.Load<Texture2D>("hero");
            player = new Player(temp, Scale);
            
            // Create zombies and zombie list
            zombieTexture = Content.Load<Texture2D>("zombie");
            enemyManager = new EnemyManager(zombieTexture, virtualScreenRect, Scale);
            enemyManager.EnemySpawnRate = 4.20f;
            enemyManager.EnemySpeed = 15f;
           
            // bullets
            bulletTexture = Content.Load<Texture2D>("bullet");
            bulletManager = new BulletManager(bulletTexture, Scale);

            // Victim
            victimTexture = Content.Load<Texture2D>("victim");
            
            // Create one victim sprite to be used for all victims
            victim = new Victim(victimTexture, Scale);
            
            // RNG for victim spawn
            random = new Random();

            // Start at level 0, game will increment to 1 before starting
            currentLevel = 0;

            // Reset values for start of game
            ResetGame();
         
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Get keyboard and gamepad states
            key = Keyboard.GetState();
            gamePad = GamePad.GetState(PlayerIndex.One);

            // Update the current state
            switch (currentState)
            {
                case GameState.TitleScreen:
                {
                    TitleScreenUpdate(gameTime);
                    break;
                }
                case GameState.Controls:
                {
                    ControlsScreenUpdate(gameTime);
                    break;
                }
                case GameState.NextLevel:
                {
                    ChangeLevel(gameTime);
                    break;
                }
                case GameState.Game:
                {
                    GameStateUpdate(gameTime);
                    break;
                }

            }
                
            base.Update(gameTime);
        }
      
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Draw the victim board to victimBoardRenderTarget
            DrawVictimBoard();

            // Set Render to 800X480 render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            // ******************************************************
            // Drawing to render target at virtual resolution
            // *******************************************************
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            // draw map 
            tileMap.Draw(spriteBatch);

            // Draw score
            spriteBatch.DrawString(scoreFont, score.ToString("D8"), new Vector2(32, 0), Color.PaleGreen);

            // draw the victim scoreboard
            spriteBatch.Draw(victimBoardRenderTarget, victimBoardRenderTargetRect, Color.White);

            // draw player over bullets
            player.Draw(spriteBatch);

            bulletManager.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);
            // Draw victim if one is spawned
            victim.Draw(spriteBatch);
           
            // Draw any messages centered on the screen
            if (message != String.Empty)
            {
                Vector2 textSize = textFont.MeasureString(message);
                Vector2 drawPos = new Vector2((VirtualWidth / 2) - (textSize.X / 2), (VirtualHeight / 2) - (textSize.Y));
                spriteBatch.DrawString(textFont, message, drawPos, messageColor);
            }
            
            spriteBatch.End();

            // ***********************************************************************
            // Drawing render target to screen
            // **********************************************************************

            // Set back to screen, scale render target up and draw to screen
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            spriteBatch.Draw(renderTarget, screenRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        // Update function for the title screen state.
        // Displays Title until start or enter is pressed
        private void TitleScreenUpdate(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Show title screen message
            message = "ZOMBIES 2600\n\n PRESS START";

            // Change state to controls screen if enter or start pressed
            if (key.IsKeyDown(Keys.Enter) || gamePad.Buttons.Start == ButtonState.Pressed)
            {
                // Change state, remove text
                currentState = GameState.Controls;
                message = String.Empty;
            }
        }

        // Update function for the controls screen
        // Shows all messages in list controlMessages with a 2sec delay between
        // begins the game when all messages have been shows
        private void ControlsScreenUpdate(GameTime gameTime)
        {
            // get current message
            message = controlMessages[currentControlMessage];

            // total elapsed time since last message
            messageTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (messageTimerElapsed > 2.0f)
            {
                // increment the message, reset the timer
                currentControlMessage++;
                messageTimerElapsed = 0;

                // all messages shown, set message to String.Empty to stop drawing
                // change state to game state
                if (currentControlMessage > controlMessages.Count - 1)
                {
                    message = String.Empty;
                    currentState = GameState.NextLevel;
                }
            }
        }

        public void ChangeLevel(GameTime gameTime)
        {
            // Change level and level values once
            if (!levelChanged)
            {
                currentLevel++;
                enemyManager.EnemySpawnRate -= 0.20f;

                // increase zombie speed every 2 levels
                if (currentLevel % 2 == 0)
                {
                    enemyManager.EnemySpeed += 1f;
                }

                levelChanged = true;
            }
            
            // Show level message for two seconds
            // then change to game state
            message = "LEVEL " + currentLevel;
            messageTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (messageTimerElapsed > 2.0f)
            {
                // Change state, remove text
                message = String.Empty;
                messageTimerElapsed = 0;
                currentState = GameState.Game;
            }
        }

        // Update function for the main game state
        public void GameStateUpdate(GameTime gameTime)
        {
            // Get input, update playerDirection and shootDirection vectors
            GameStateHandleInput();

            // Spawn victims
            SpawnVictims();

            // Update the player
            player.Update(gameTime, tileMap, playerDirection, virtualScreenRect);

            // Check victim save
            if (victim.IsAlive && player.CollisionRect.Intersects(victim.CollisionRect))
            {
                victim.IsAlive = false;
                score += 500;
                saved++;
                UpdateVictimBoard(victimSave);
            }

            // Update enemy and bullet managers
            enemyManager.Update(gameTime, player, tileMap, victim);
            bulletManager.Update(gameTime, player, tileMap, shootDirection, virtualScreenRect);

            Console.WriteLine(enemyManager.EnemySpawnRate);
            Console.WriteLine(enemyManager.EnemySpeed);

            // Check bullet collision with enemy
            UpdateBulletsAndCheckCollisions(gameTime);

            // Check enemy collision with victim
            for (int i = 0; i < enemyManager.Enemies.Count; i++)
            {
                if (victim.IsAlive && enemyManager.Enemies[i].CollisionRect.Intersects(victim.CollisionRect))
                {
                    victim.IsAlive = false;
                    UpdateVictimBoard(victimKilled);
                }
            }

            // Check win/lose conditions
            // All victims spawned
            if (currentVictim == totalVictims)
            {
                // If saved at least half the victims, go to next level
                if (saved >= totalVictims / 2)
                {
                    ResetGame();
                    currentState = GameState.NextLevel;
                }
            }
        }

        // Handle input from keyboard and gamepad
        // update playerDirection and shootDirection vectors
        public void GameStateHandleInput()
        {
            // Reset direction vector to stop moving
            playerDirection = Vector2.Zero;

            // Handle movement input
            // Set x,y values independently to allow for diagonal movement
            // move left
            if (key.IsKeyDown(Keys.A) || gamePad.ThumbSticks.Left.X < 0)
            {
                playerDirection.X = -1f;
            }
            // move right
            if (key.IsKeyDown(Keys.D) || gamePad.ThumbSticks.Left.X > 0)
            {
                playerDirection.X = 1f;
            }
            // move up
            if (key.IsKeyDown(Keys.W) || gamePad.ThumbSticks.Left.Y > 0)
            {
                playerDirection.Y = -1f;
            }
            // move down
            if (key.IsKeyDown(Keys.S) || gamePad.ThumbSticks.Left.Y < 0)
            {
                playerDirection.Y = 1f;
            }


            // Handle shooting input
            // Set opposite axis to zero to disallow diagonal shooting
            // .5 and -.5 as the threshold for stick axis so won't change shoot direction
            // until pushing dominantly in that direction
            shootDirection = Vector2.Zero;
            if (key.IsKeyDown(Keys.Left) || gamePad.ThumbSticks.Right.X < -.5)
            {
                shootDirection = new Vector2(-1, 0);
                //shootDirection.X = -1;
            }
            if (key.IsKeyDown(Keys.Right) || gamePad.ThumbSticks.Right.X > .5)
            {
                shootDirection = new Vector2(1, 0);
                //shootDirection.X = 1;
            }
            if (key.IsKeyDown(Keys.Up) || gamePad.ThumbSticks.Right.Y > .5)
            {
                shootDirection = new Vector2(0, -1);
                //shootDirection.Y = -1;
            }
            if (key.IsKeyDown(Keys.Down) || gamePad.ThumbSticks.Right.Y < -.5)
            {
                shootDirection = new Vector2(0, 1);
                //shootDirection.Y = 1;
            }
        }

        // Spawn on a random, non-solid, non-occupied tile with a chance of
        // spawnChance
        private void SpawnVictims()
        {
            float spawnChance = .003f;
            
            // Try to spawn a new victim if there isn't one
            if (!victim.IsAlive && random.NextDouble() < spawnChance)
            {
                // Get a tile to spawn victim on
                Tile tile = tileMap.GetTile(random.Next(TileMap.MAP_WIDTH), random.Next(TileMap.MAP_HEIGHT));
                bool tileClear = false;
                // keep getting tiles until we find one that is not solid and clear of zombies
                while (tile.IsSolid || tileClear == false)
                {
                    // Get a new random tile from map
                    tile = tileMap.GetTile(random.Next(TileMap.MAP_WIDTH), random.Next(TileMap.MAP_HEIGHT));
                    if (!tile.IsSolid)
                    {
                        // if not solid, check if any zombies are colliding with this tile
                        tileClear = true;
                        foreach (Zombie zombie in enemyManager.Enemies)
                        {
                            // get all tiles zombie is currently intersecting
                            List<Tile> collisionTiles = tileMap.CheckTileCollsions(zombie.CollisionRect);
                        
                            foreach (Tile collisionTile in collisionTiles)
                            {
                                // check if any zombie collision tiles are the same as the tile we've chosen
                                if (collisionTile.Position == tile.Position)
                                {
                                    tileClear = false;
                                }
                            }
                        }
                    }    
                }

                // found tile is not solid and clear of zombies,
                // set victim position to tile position and toggle IsAlive
                victim.Position = new Vector2(tile.Position.X, tile.Position.Y);
                victim.IsAlive = true;
            }
        }

        // Update all bullets and check collsion with enemies
        public void UpdateBulletsAndCheckCollisions(GameTime gameTime)
        {
            // Update the bullets
            for (int i = 0; i < bulletManager.Bullets.Count; i++)
            {
                // Check each bullet for collision with each enemy
                for (int j = 0; j < enemyManager.Enemies.Count; j++)
                {
                    if (bulletManager.Bullets[i].CollisionRect.Intersects(enemyManager.Enemies[j].CollisionRect))
                    {
                        bulletManager.Kill(i);
                        enemyManager.Kill(j);
                        score += 100;
                    }
                }
            }
        }

        // Update the victim board tinting victim sprites with color
        public void UpdateVictimBoard(Color color)
        {
            if (currentVictim < victims.Length)
            {
                // tint sprite, increment current victim
                victimColor[currentVictim] = color;
                currentVictim++;
            }
        }

        // Draw victim board to a render target to 
        // to draw the whole thing to the screen at once
        private void DrawVictimBoard()
        {
            // Set render target to the victim board render target
            GraphicsDevice.SetRenderTarget(victimBoardRenderTarget);

            // Clear with transparent
            GraphicsDevice.Clear(Color.Transparent);

            // Draw with point clamp filtering
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            // Create a sprite and draw a survivor for each survivor left
            for (int i = 0; i < victims.Length; i++)
            {
                // draw victim scoreboard sprites with color tint
                victims[i].Draw(spriteBatch, victimColor[i]);
            }
            spriteBatch.End();
        }

        public void ResetGame()
        {
            // Move player to center
            //player.Position = new Vector2((VirtualWidth / 2) - (player.Width / 2), (VirtualHeight / 2) - (player.Height / 2));
            player.Position = new Vector2(200f, 200f);
            // Reset victim
            victim.IsAlive = false;
            totalVictims = 8;
            currentVictim = 0;
            saved = 0;

            // build parallel arrays for victim board sprites and color tints
            victims = new Sprite[totalVictims];
            victimColor = new Color[totalVictims];
            for (int i = 0; i < victims.Length; i++)
            {
                Sprite sprite = new Sprite(victimTexture, Scale);
                sprite.Position = new Vector2(i * sprite.Width, 0);
                victims[i] = sprite;
                victimColor[i] = Color.White;
            }

            enemyManager.Reset();
            bulletManager.Reset();

            // reset level changed flag
            levelChanged = false;
        }
    }
}
