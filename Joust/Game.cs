﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joust
{
    using Serv = Engine.Services;
    using Timer = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_GraphicsDM;
        Joust.PO.Player m_Player;
        Texture2D m_PlayerTexture;

        public Game()
        {
            m_GraphicsDM = new GraphicsDeviceManager(this);
            m_GraphicsDM.IsFullScreen = false;
            m_GraphicsDM.SynchronizeWithVerticalRetrace = true;
            m_GraphicsDM.GraphicsProfile = GraphicsProfile.HiDef;
            m_GraphicsDM.PreferredBackBufferWidth = 1200;
            m_GraphicsDM.PreferredBackBufferHeight = 900;
            m_GraphicsDM.PreferMultiSampling = true; //Error in MonoGame 3.6 for DirectX, fixed for next version.
            m_GraphicsDM.PreparingDeviceSettings += SetMultiSampling;
            m_GraphicsDM.ApplyChanges();
            IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            m_Player = new Joust.PO.Player(this);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 4;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Serv.Initialize(m_GraphicsDM, this);
            // Create a new SpriteBatch, which can be used to draw textures.
            Serv.SpriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        protected override void BeginRun()
        {
            base.BeginRun();

            m_Player.Texture(m_PlayerTexture);
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            m_PlayerTexture = Content.Load<Texture2D>(@"JoustSpriteSheet");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(new Vector3(0.01666f, 0, 0.1f)));

            Serv.SpriteBatch.Begin();
            base.Draw(gameTime);
            Serv.SpriteBatch.End();
        }
    }
}
