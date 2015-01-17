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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;
        public static Rectangle screenRectangle;
        TileMap tileMap;
        Player player;
        Zombie zombie;
        List<Zombie> enemies;
        List<Bullet> bullets;
        KeyboardState key;
        GamePadState gamePad;
        Texture2D bulletTexture;

        float totalElapsed = 0;
        float fireRate = .30f;

        public static Vector2 Scale;

        public const int VirtualWidth = 800;
        public const int VirtualHeight = 480;


        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);


            //graphics.PreferredBackBufferWidth = 864;
            //graphics.PreferredBackBufferHeight = 480;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            // Make fullscreen
            Window.IsBorderless = true;
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

            // Create a 2x scale
            Scale = new Vector2(2, 2);

            // Create render target at Virtual resolution
            renderTarget = new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);

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

            Texture2D temp;

            // Create tile map
            temp = Content.Load<Texture2D>("tiles");
            tileMap = new TileMap(temp);

            // Create player
            temp = Content.Load<Texture2D>("hero");
            player = new Player(temp);
            player.Position = new Vector2((VirtualWidth / 2) - (player.Width / 2), (VirtualHeight / 2) - (player.Height / 2));

            // Create zombies
            temp = Content.Load<Texture2D>("zombie");
            enemies = new List<Zombie>();
            for (int i = 0; i < 50; i++)
            {
                zombie = new Zombie(temp);
                zombie.Position = new Vector2((VirtualWidth / 2) - (zombie.Width / 2), -(i * 50) - zombie.Height);                
                enemies.Add(zombie);
            }

            // load bullet texture
            bulletTexture = Content.Load<Texture2D>("bullet");

            bullets = new List<Bullet>();
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

            // Reset direction vector to stop moving
            Vector2 playerDirection = Vector2.Zero;

            // Get keyboard and gamepad states
            key = Keyboard.GetState();
            gamePad = GamePad.GetState(PlayerIndex.One);

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
            Vector2 shootDirection = Vector2.Zero;
            if (key.IsKeyDown(Keys.Left) || gamePad.ThumbSticks.Right.X < -.5)
            {
                shootDirection.X = -1;
            }
            if (key.IsKeyDown(Keys.Right) || gamePad.ThumbSticks.Right.X > .5)
            {
                shootDirection.X = 1;
            }
            if (key.IsKeyDown(Keys.Up) || gamePad.ThumbSticks.Right.Y > .5)
            {
                shootDirection.Y = -1;
            }
            if (key.IsKeyDown(Keys.Down) || gamePad.ThumbSticks.Right.Y < -.5)
            {
                shootDirection.Y = 1;
            }

            // accumulate elapsed time
            totalElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // if shooting
            if (shootDirection != Vector2.Zero)
            {
                // if fireRate time has passed since last shot
                if (totalElapsed > fireRate)
                {
                    // create a new bullet at player position, in shootDirection
                    shootDirection.Normalize();
                    Bullet bullet = new Bullet(bulletTexture, shootDirection);
                    bullet.Position = new Vector2(
                        player.Position.X + ((player.Width / 2.0f) - (bullet.Width / 2.0f)),
                        player.Position.Y + ((player.Height / 2.0f) - (bullet.Height / 2.0f)));
                    bullets.Add(bullet);
                    // reset timer
                    totalElapsed = 0;
                }      
            }

            // Update the player
            player.Update(gameTime, tileMap, playerDirection);
           
            // Update the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime, tileMap, player.Position);
            }

            ResolveEnemyCollision();

            // Update the bullets
            for (int i = 0; i < bullets.Count; i++ )
            {
                bullets[i].Update(gameTime, tileMap);
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (bullets[i].CollisionRect.Intersects(enemies[j].CollisionRect))
                    {
                        bullets[i].IsAlive = false;
                        enemies[j].IsAlive = false;
                    }
                }
            }

            // Remove inactive bullets
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].IsAlive)
                {
                    bullets.Remove(bullets[i]);
                }
            }

            // Remove inactive enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].IsAlive)
                {
                    enemies.Remove(enemies[i]);
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
            // Set Render to 800X480 render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
       
            // Draw with point clamp sampling to avoid blurry textures
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            // draw map 
            tileMap.Draw(spriteBatch);

            // draw bullets
            for (int i = 0; i < bullets.Count; i++ )
            {
                bullets[i].Draw(spriteBatch);
            }

            // draw player over bullets
            player.Draw(spriteBatch);

            // draw enemies over player
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }
            
            spriteBatch.End();


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


        // Check for and resolve collisions between enemies
        // if two enemies collide, push them back in opposite directions
        public void ResolveEnemyCollision()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    Zombie zombie1 = enemies[i] as Zombie;
                    Zombie zombie2 = enemies[j] as Zombie;

                    // Check if enemies are colliding and get the depth of collision
                    float depth = zombie1.CollisionCircle.GetIntersectionDepth(zombie2.CollisionCircle);

                    // Collision
                    if (depth != 0)
                    {
                        // Get direction to move first zombie away from second zombie
                        Vector2 direction = zombie1.CollisionCircle.Position - zombie2.CollisionCircle.Position;
                        direction.Normalize();

                        // Move first zombie away half the depth of collision
                        zombie1.Position += direction * (depth / 2.0f);
                        // Move second zombie in the opposite direction half the depth of collision
                        zombie2.Position -= direction * (depth / 2.0f);
                    }
                }
            }
        }
    }
}
