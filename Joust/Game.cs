using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joust
{
    using Serv = Engine.Services;
    using Timer = Engine.Timer;
    using Object = Engine.PositionedObject;

    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_GraphicsDM;
        PO.Player m_Player;
        Background m_Background;
        EnemyControl m_Enemy;
        Engine.SpriteFontDisplay m_P1Score;

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

            m_Background = new Background(this);
            m_Player = new PO.Player(this);
            m_Enemy = new EnemyControl(this);
            m_P1Score = new Engine.SpriteFontDisplay(this);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 1;
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
            base.Initialize();
            m_Player.BackgroundReference(m_Background);
            m_Enemy.BackgroundReference(m_Background);
            m_Enemy.PlayerReference(m_Player);
            Serv.AddUpdateableComponent(m_Background);
            Components.Add(m_Enemy);
            Serv.AddBeginable(m_Background);
            Serv.AddBeginable(m_Enemy);
        }

        protected override void BeginRun()
        {
            base.BeginRun();

            m_Background.BeginRun();
            Serv.BeginRun();
            m_P1Score.String = "0";
            m_P1Score.TintColor = Color.Red;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            m_Background.LoadContent();
            m_P1Score.Initialize(Content.Load<SpriteFont>(@"Joystix"), new Vector2(420, 770)); // 420 770
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
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
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
