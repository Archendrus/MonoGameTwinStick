﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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
        Vector2 Scale;     
        Rectangle screenRectangle;

        // Game states
        enum GameState
        { 
            TitleScreen,
            Game,
            NextLevelScreen,
            ControlsScreen,
            Pause,
            GameOver
        };

        GameState currentState;
        int currentLevel;

        // Game objects
        TileMap tileMap;
        Player player;
        Victim victim;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        ParticleEngine particleEngine;

        // Input states
        KeyboardState key;
        KeyboardState lastKey;
        GamePadState gamePad;
        GamePadState lastGamePad;

        // Texture for bullet
        Texture2D bulletTexture;
        Texture2D zombieTexture;
        Texture2D victimTexture;

        // Text and messages
        SpriteFont textFont;
        String message;
        List<String> messagesList;
        Color messageColor;
        float messageTimerElapsed;
        int currentMessage;

        // Player/bullet update args
        Vector2 playerDirection;
        Vector2 shootDirection;
        int playerStartX;
        int playerStartY;

        // Sounds
        SoundEffect victimSaveSound;
        SoundEffect pauseSound;

        // Random number generator for victim spawn
        Random random;

        // Score and surviors
        SpriteFont scoreFont;
        int score;      
        const int TOTAL_VICTIMS = 8;
        int currentVictim;
        int saved;
        Sprite[] victims;
        Color[] victimColor;
        Color victimSave, victimKilled;
        RenderTarget2D victimBoardRenderTarget;
        Rectangle victimBoardRenderTargetRect;

        // Lives
        int lives;
        Sprite livesSprite;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;

            // Use windowed fullscreen
            graphics.HardwareModeSwitch = false;
            graphics.ApplyChanges();

            IsFixedTimeStep = false;
           
            Content.RootDirectory = "Content";
            this.Window.Title = "Zombies 2600";
        }
   
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // Create screen rectangle at size of backbuffer
            screenRectangle = new Rectangle(
                0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // Create a 2x scale
            Scale = new Vector2(2, 2);

            // Create render target for drawing survior score board
            victimBoardRenderTarget = new RenderTarget2D(GraphicsDevice, 256, 32);

            // Create rectangle for draw position
            victimBoardRenderTargetRect = new Rectangle(512, 4, 256, 24);

            // Initialize values for messages
            messageColor = new Color(64, 124, 64);

            // Create message list for multiple messages
            messagesList = new List<String>();

            // Reset state delay time
            //stateChangeDelay = 0;

            // Start on titlescreen
            ChangeState(GameState.TitleScreen);

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

            // Create particle engine
            temp = Content.Load<Texture2D>("particle");
            particleEngine = new ParticleEngine(temp, Vector2.Zero, Scale);

            // Create player
            temp = Content.Load<Texture2D>("hero");
            SoundEffect sound = Content.Load<SoundEffect>("die");
            player = new Player(temp, Scale, sound, particleEngine);
            playerStartX = 260;
            playerStartY = 220;

            // Create sprite for lives display
            livesSprite = new Sprite(temp, Scale);
            livesSprite.IsAlive = false;

            // Create zombies and zombie list
            zombieTexture = Content.Load<Texture2D>("zombie");
            sound = Content.Load<SoundEffect>("explode");
            enemyManager = new EnemyManager(zombieTexture, screenRectangle, Scale, sound, particleEngine);
           
            // bullets
            bulletTexture = Content.Load<Texture2D>("bullet");
            sound = Content.Load<SoundEffect>("shoot");
            bulletManager = new BulletManager(bulletTexture, Scale, sound);

            // Create one victim sprite to be used for all victims
            victimTexture = Content.Load<Texture2D>("victim");
            sound = Content.Load<SoundEffect>("die");
            victim = new Victim(victimTexture, sound, Scale, particleEngine);

            victimSaveSound = Content.Load<SoundEffect>("pickup");
            pauseSound = Content.Load<SoundEffect>("pause");
         
            // RNG for victim spawn
            random = new Random();

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
            // Get keyboard and gamepad states
            key = Keyboard.GetState();
            gamePad = GamePad.GetState(PlayerIndex.One);

            // Exit game if back or esc pressed
            if (gamePad.Buttons.Back == ButtonState.Pressed || key.IsKeyDown(Keys.Escape))
                Exit();

            // Toggle fullscreen on F1 or Y Button
            if((key.IsKeyDown(Keys.F1) && lastKey.IsKeyUp(Keys.F1)) || 
                (gamePad.Buttons.Y == ButtonState.Pressed && lastGamePad.Buttons.Y == ButtonState.Released))
            {
                graphics.ToggleFullScreen();
            }

            // Update the current state
            switch (currentState)
            {
                case GameState.TitleScreen:
                {
                    TitleScreenUpdate(gameTime);
                    break;
                }
                case GameState.ControlsScreen:
                {
                    ControlsScreenUpdate(gameTime);
                    break;
                }
                case GameState.NextLevelScreen:
                {
                    NextLevelScreenUpdate(gameTime);
                    break;
                }
                case GameState.Game:
                {
                    GameStateUpdate(gameTime);
                    break;
                }
                case GameState.Pause:
                {
                    PauseScreenUpdate();
                    break;
                }
                case GameState.GameOver:
                {
                    GameOverScreenUpdate(gameTime);
                    break;
                }
            }

            // save last key and gamepad states to check for single key presses
            lastKey = key;
            lastGamePad = gamePad;
              
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
            
            // Clear screen
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // Draw to screen
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            // draw map 
            tileMap.Draw(spriteBatch);

            // Draw blood particles just over tiles
            particleEngine.Draw(spriteBatch);

            // Draw score
            spriteBatch.DrawString(scoreFont, score.ToString("D8"), new Vector2(32, 0), Color.PaleGreen);

            // draw the victim scoreboard
            spriteBatch.Draw(victimBoardRenderTarget, victimBoardRenderTargetRect, Color.White);
            
            // draw bullets
            bulletManager.Draw(spriteBatch);

            // draw player, enemies, and victims over bullets
            player.Draw(spriteBatch);
            enemyManager.Draw(spriteBatch);

            // Draw victim if one is spawned
            victim.Draw(spriteBatch);
           
            // Draw any messages centered on the screen
            if (message != String.Empty)
            {
                int width = screenRectangle.Width;
                int height = screenRectangle.Height;
                Vector2 textSize = textFont.MeasureString(message);
                Vector2 drawPos = new Vector2((width / 2) - (textSize.X / 2), (height / 2) - (textSize.Y + 48));
                spriteBatch.DrawString(textFont, message, drawPos, messageColor);

                // draw lives sprite next to text if IsAlive
                livesSprite.Draw(spriteBatch);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Run initialization logic for newState
        // Clear messages then change to newState
        private void ChangeState(GameState newState)
        {
            message = String.Empty;

            switch (newState)
            {
                // Enter TitleScreen
                // set titlescreen message
                case GameState.TitleScreen:
                {
                    message = "ZOMBIES 2600\n\n PRESS START";
                    break;
                }
                // Enter ControlsScreen
                // create a list of message to be shown
                case GameState.ControlsScreen:
                {
                    messagesList = new List<String>();
                    messagesList.Add("WASD / LEFT STICK TO MOVE");
                    messagesList.Add("ARROWS / RIGHT STICK TO SHOOT");
                    messagesList.Add("F1 / Y BUTTON");
                    messagesList.Add("TO TOGGLE FULLSCREEN/WINDOW");
                    messagesList.Add("SHOOT ZOMBIES!\nSAVE SURVIORS!");
                    break;
                }
                // Enter NextLevelScreen
                case GameState.NextLevelScreen:
                {
                    // Set values for lives sprite
                    livesSprite.Position = new Vector2(348, (screenRectangle.Height / 2) - (livesSprite.Height + 50));

                    // Add messages
                    messagesList.Add("LEVEL " + currentLevel);
                    messagesList.Add("   X " + lives);                  
                    break;
                }
                // Enter Game state
                case GameState.Game:
                {
                    // No special init for game state
                    break;
                }
                // Enter pause state
                // Show pause message
                case GameState.Pause:
                {
                    message = "PAUSE";
                    break;
                }
                case GameState.GameOver:
                {
                    message = "GAME OVER";
                    break;
                }
            }

            // change current state to newState
            currentState = newState;
        }

        // Update function for the title screen state.
        // Displays Title until start or enter is pressed
        private void TitleScreenUpdate(GameTime gameTime)
        {
            // Change state to controls screen if enter or start pressed
            if (key.IsKeyDown(Keys.Enter) || gamePad.Buttons.Start == ButtonState.Pressed)
            {
                // Change state, remove text
                ChangeState(GameState.ControlsScreen);
            }
        }

        // Update function for the controls screen
        // Shows all messages in list controlMessages with a 2sec delay between
        // begins the game when all messages have been shows
        private void ControlsScreenUpdate(GameTime gameTime)
        {
            // get current message
            message = messagesList[currentMessage];

            // total elapsed time since last message
            messageTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (messageTimerElapsed > 2.0f)
            {
                // increment the message, reset the timer
                currentMessage++;
                messageTimerElapsed = 0;

                // all messages shown, set message to String.Empty to stop drawing
                // change state to NextLevel state
                if (currentMessage > messagesList.Count - 1)
                {
                    message = String.Empty;
                    messagesList.Clear();
                    ResetGameForNextLevel();
                    ChangeState(GameState.NextLevelScreen);
                }
            }
        }
        
        // Update logic for NextLevelScreen
        // shows level message then changes to game state
        private void NextLevelScreenUpdate(GameTime gameTime)
        {
            // Get current message
            message = messagesList[currentMessage];

            // total elapsed time since last message
            messageTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (messageTimerElapsed > 3.0f)
            {
                // increment the message, reset the timer
                currentMessage++;
                messageTimerElapsed = 0;

                // Remove lives sprite if not displaying lives
                if (currentMessage > 0)
                {
                    livesSprite.IsAlive = true;
                }

                // all messages shown, set message to String.Empty to stop drawing
                // change state to game state
                if (currentMessage > messagesList.Count - 1)
                {
                    message = String.Empty;
                    messagesList.Clear();
                    player.IsAlive = true;
                    livesSprite.IsAlive = false;
                    ChangeState(GameState.Game);
                }
            }
        }

        // Update logic for the main game state
        private void GameStateUpdate(GameTime gameTime)
        {
            // Pause game if enter or start pressed
            if ((key.IsKeyDown(Keys.Enter) && 
                lastKey.IsKeyUp(Keys.Enter)) || 
                (gamePad.Buttons.Start == ButtonState.Pressed &&
                lastGamePad.Buttons.Start == ButtonState.Released))
            {
                pauseSound.Play();
                ChangeState(GameState.Pause);
            }

            // Get input, update playerDirection and shootDirection vectors
            GameStateHandleInput();

            // Spawn victims
            SpawnVictims();

            // Update the player
            player.Update(gameTime, tileMap, playerDirection, screenRectangle);

            // Check victim save
            if (victim.IsAlive && player.CollisionRect.Intersects(victim.CollisionRect))
            {
                // Remove victim, update score, saved and victim board
                // play sound
                victim.IsAlive = false;
                score += 500;
                saved++;
                UpdateVictimBoard(victimSave);
                victimSaveSound.Play();
            }

            // Update enemy and bullet managers
            enemyManager.Update(gameTime, player, tileMap, victim);
            bulletManager.Update(gameTime, player, tileMap, shootDirection, screenRectangle);

            // Check bullet collision with enemy
            CheckBulletCollisions(gameTime);

            // Update particle engine
            particleEngine.Update(gameTime, tileMap);

            // Check enemy collision with victims
            if (enemyManager.HadVictimCollision)
            {
                victim.Kill(gameTime);
                UpdateVictimBoard(victimKilled);
            }

            // check enemy collision with player
            if (enemyManager.HadPlayerCollision)
            {
                // Hide player, decrement lives by one
                player.Kill(gameTime);
                lives--;              
            }

            // If player dead, check lives and change state
            // player.Dead returns true when death sound effect 
            // has stopped playing
            if (player.Dead())
            {
                // If still have lives
                if (lives > 0)
                {
                    // Reset level preserving score and victims
                    ResetLevel();
                    ChangeState(GameState.NextLevelScreen);
                }
                else  // No lives left
                {
                    // Go to game over
                    ChangeState(GameState.GameOver);
                }
            }         

            // Check win/lose conditions
            // All victims spawned
            if (currentVictim == TOTAL_VICTIMS && victim.Dead())
            {
                // If saved at least half the victims, go to next level
                if (saved >= TOTAL_VICTIMS / 2)
                {
                    // level passed, change level, reset values for next level
                    // and change state
                    ChangeLevel();
                    ResetGameForNextLevel();
                    ChangeState(GameState.NextLevelScreen);
                }
                // if didn't save at least half the victims and lives left
                // decrement lives and reset the level
                else if (lives > 0)
                {
                    lives--; 
                    ResetGameForNextLevel();
                    ChangeState(GameState.NextLevelScreen);
                }
                else // no lives left, game over
                {
                    ChangeState(GameState.GameOver);
                }
            }
        }

        // update logic for the pause screen
        private void PauseScreenUpdate()
        {
            //  Go back to game if enter or start pressed
            if ((key.IsKeyDown(Keys.Enter) &&
                lastKey.IsKeyUp(Keys.Enter)) ||
                (gamePad.Buttons.Start == ButtonState.Pressed &&
                lastGamePad.Buttons.Start == ButtonState.Released))
            {
                pauseSound.Play();
                ChangeState(GameState.Game);   
            }
        }

        // Update logic for the game over screen
        private void GameOverScreenUpdate(GameTime gameTime)
        {
            messageTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (messageTimerElapsed > 3.0f)
            {
                // remove message, reset game, set player back to alive
                // change to next level (level 1 after reset)
                message = String.Empty;
                messageTimerElapsed = 0;

                // Reset the entire game
                ResetGame();
                ChangeState(GameState.NextLevelScreen);
            }
        }

        // Handle input from keyboard and gamepad
        // update playerDirection and shootDirection vectors
        private void GameStateHandleInput()
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
            }
            if (key.IsKeyDown(Keys.Right) || gamePad.ThumbSticks.Right.X > .5)
            {
                shootDirection = new Vector2(1, 0);
            }
            if (key.IsKeyDown(Keys.Up) || gamePad.ThumbSticks.Right.Y > .5)
            {
                shootDirection = new Vector2(0, -1);
            }
            if (key.IsKeyDown(Keys.Down) || gamePad.ThumbSticks.Right.Y < -.5)
            {
                shootDirection = new Vector2(0, 1);
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
                bool tileClear = IsTileClear(tile);
                // keep getting tiles until we find one that is not solid and clear of zombies
                while (tile.IsSolid || tileClear == false)
                {
                    // Get a new random tile from map
                    tile = tileMap.GetTile(random.Next(TileMap.MAP_WIDTH), random.Next(TileMap.MAP_HEIGHT));
                    if (!tile.IsSolid)
                    {
                        tileClear = IsTileClear(tile);
                    }    
                }

                // found tile is not solid and clear of zombies,
                // set victim position to tile position and toggle IsAlive
                victim.Position = new Vector2(tile.Position.X, tile.Position.Y);
                victim.IsAlive = true;
            }
        }
        // Check a tile to see if it is solid or occupied by enemy
        // returns true if clear, false otherwise
        private bool IsTileClear(Tile tile)
        {
            // if not solid, check if any zombies are colliding with this tile
            bool tileClear = true;
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

            return tileClear;
        }

        // Update all bullets and check collsion with enemies
        private void CheckBulletCollisions(GameTime gameTime)
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
                        enemyManager.Kill(j, gameTime, bulletManager.Bullets[i].Direction);
                        
                        // Add to score
                        score += 100;
                    }
                }
            }
        }

        // Update the victim board tinting victim sprites with color
        private void UpdateVictimBoard(Color color)
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

            // Set render target back to screen
            GraphicsDevice.SetRenderTarget(null);
        }

        // Increment the level and change 
        // speed and spawn rate in enemy manager
        private void ChangeLevel()
        {
            currentLevel++;
            enemyManager.EnemySpawnRate -= 0.2f;

            // increase zombie speed every 2 levels
            if (currentLevel % 2 == 0)
            {
                enemyManager.EnemySpeed += 1f;
            }
        }

        // Reset the current level preserving score, lives, and victims saved
        private void ResetLevel()
        {
            // Move player to center
            player.Position = new Vector2(playerStartX, playerStartY);

            victim.IsAlive = false;

            // Clear out all enemies and bullets
            enemyManager.Reset();
            bulletManager.Reset();
            particleEngine.Reset();

            // Reset message values
            messageTimerElapsed = 0;
            currentMessage = 0;
        }

        // Reset game values for the next level
        // Score, enemySpawnRate, and enemySpeed are preserved from previous level
        private void ResetGameForNextLevel()
        {
            // Move player to center
            player.Position = new Vector2(playerStartX, playerStartY);

            // Reset victim
            victim.IsAlive = false;
            currentVictim = 0;
            saved = 0;

            // Reset message values
            messageTimerElapsed = 0;
            currentMessage = 0;

            // build parallel arrays for victim board sprites and color tints
            victims = new Sprite[TOTAL_VICTIMS];
            victimColor = new Color[TOTAL_VICTIMS];
            for (int i = 0; i < victims.Length; i++)
            {
                Sprite sprite = new Sprite(victimTexture, Scale);
                sprite.Position = new Vector2(i * sprite.Width, 0);
                victims[i] = sprite;
                victimColor[i] = Color.White;
            }

            // Clear out all enemies and bullets
            enemyManager.Reset();
            bulletManager.Reset();
            particleEngine.Reset();
        }

        // Reset all values for game
        private void ResetGame()
        {
            // Reset everything for a new level
            ResetGameForNextLevel();

            // Also reset current level, score, and enemy values
            currentLevel = 0;
            score = 0;
            lives = 3;
            enemyManager.EnemySpawnRate = 4.20f;
            enemyManager.EnemySpeed = 15f;
            ChangeLevel();           
        }
    }
}
