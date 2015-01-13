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
        Rectangle screenRectangle;
        TileMap tileMap;
        Player player;
        Zombie zombie;
        List<Sprite> enemies;

        public static Vector2 Scale;

        public const int VirtualWidth = 800;
        public const int VirtualHeight = 480;


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
            // TODO: Add your initialization logic here

            // Create screen rectangle at size of user's desktop resolution
            screenRectangle = new Rectangle(
                0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            

            // Create a 2x scale
            Scale = new Vector2(2, 2);

            // Create render target at 2x Virtual resolution
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
            temp = Content.Load<Texture2D>("tiles");
            tileMap = new TileMap(temp);
            temp = Content.Load<Texture2D>("hero");

            player = new Player(temp);
            player.Position = new Vector2((VirtualWidth / 2) - (player.Width / 2), (VirtualHeight / 2) - (player.Height / 2));
            temp = Content.Load<Texture2D>("zombie");
            enemies = new List<Sprite>();
            for (int i = 0; i < 2; i++)
            {
                zombie = new Zombie(temp);
                zombie.Position = new Vector2((VirtualWidth / 2) - (zombie.Width / 2), -(i * 50) - zombie.Height);                
                enemies.Add(zombie);
            }   
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            player.Update(gameTime, tileMap);
            for (int i = 0; i < enemies.Count; i++)
            {
                Zombie zombie = enemies[i] as Zombie;
                zombie.Update(gameTime, tileMap, player.Position);
            }

            ResolveEnemyCollision();

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Set Render to 432X240 render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
       
            // Draw with point clamp sampling to avoid blurry textures
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
            // draw map and player to render target
            tileMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void ResolveEnemyCollision()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    Zombie zombie1 = enemies[i] as Zombie;
                    Zombie zombie2 = enemies[j] as Zombie;
                    float depth = zombie1.CollisionCircle.GetIntersectionDepth(zombie2.CollisionCircle);
                    if (depth != 0)
                    {
                        Vector2 direction = zombie1.CollisionCircle.Position - zombie2.CollisionCircle.Position;
                        direction.Normalize();
                        zombie1.Position += direction * (depth / 2.0f);
                        zombie2.Position -= direction * (depth / 2.0f);
                    }
                }
            }
        }
    }
}
