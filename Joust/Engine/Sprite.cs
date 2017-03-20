using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Joust.Engine
{
    public class Sprite : PositionedObject, IDrawComponent
    {
        #region Declarations
        private Texture2D m_Texture;
        private Color m_TintColor = Color.White;
        private List<Rectangle> m_Frames = new List<Rectangle>();
        private Timer m_FrameTime;
        private int m_CurrentFrame;
        public bool Animate = false;
        public bool OnTopOfParent = false;
        public bool AnimateWhenStopped = true;
        #endregion

        #region Drawing and Animation Properties
        public int FrameWidth
        {
            get { return m_Frames[0].Width; }
        }

        public int FrameHeight
        {
            get { return m_Frames[0].Height; }
        }

        public Color TintColor
        {
            get { return m_TintColor; }
            set { m_TintColor = value; }
        }

        public int Frame
        {
            get { return m_CurrentFrame; }
            set
            {
                m_CurrentFrame = (int)MathHelper.Clamp(value, 0, m_Frames.Count - 1);
            }
        }

        public float FrameTime
        {
            get { return m_FrameTime.Amount; }
            set { m_FrameTime.Amount = MathHelper.Max(0, value); }
        }

        public Rectangle Source
        {
            get { return m_Frames[m_CurrentFrame]; }
        }
        #endregion
        public Sprite(Game game) : base(game)
        {
            m_FrameTime = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();
            Services.AddDrawableComponent(this);
            m_FrameTime.Amount = 0.1f;
        }

        public void Initialize(Texture2D texture, Rectangle initialFrame, Vector2 position, bool onTop)
        {
            m_Texture = texture;
            Position = position;
            OnTopOfParent = onTop;
            m_Frames.Add(initialFrame);
        }

        public void AddFrame(Rectangle frameRectangle)
        {
            m_Frames.Add(frameRectangle);
        }

        public override void BeginRun()
        {

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (Animate)
            {
                if (m_FrameTime.Seconds >= m_FrameTime.Amount)
                {
                    if ((AnimateWhenStopped) || (Velocity != Vector2.Zero))
                    {
                        m_CurrentFrame = (m_CurrentFrame + 1) % (m_Frames.Count);
                        m_FrameTime.Reset();
                    }
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (Active)
            {
                Services.SpriteBatch.Draw(m_Texture, Position, Source, m_TintColor, RotationInRadians,
                    Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
            }
        }
    }
}
