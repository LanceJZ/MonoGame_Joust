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

        public Texture2D Texture
        {
            get { return m_Texture; }
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
        /// <summary>
        /// Initialize sprite with texture, where on sprite sheet to get texture, starting position, and if on top if child.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="initialFrame"></param>
        /// <param name="position"></param>
        /// <param name="onTop"></param>
        public void Initialize(Texture2D texture, Rectangle initialFrame, Vector2 position, float scale, bool onTop, bool animate)
        {
            m_Texture = texture;
            Position = position;
            Scale = scale;
            SetAABB(new Vector2(initialFrame.Width - initialFrame.X, initialFrame.Height - initialFrame.Y));
            OnTopOfParent = onTop;
            Animate = animate;
            m_Frames.Add(initialFrame);
        }
        /// <summary>
        /// This is mostly used for background sprites. Not movement and animation will be set to false.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="initialFrame"></param>
        /// <param name="position"></param>
        public void Initialize(Texture2D texture, Rectangle initialFrame, Vector2 position, float scale)
        {
            m_Texture = texture;
            Position = position;
            Scale = scale;
            SetAABB(new Vector2(initialFrame.Width, initialFrame.Height));
            OnTopOfParent = false;
            Moveable = false;
            Animate = false;
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
            if (Animate && Active)
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
            if (Active && !OnTopOfParent)
            {
                if (m_Frames.Count > 0)
                {
                    Services.SpriteBatch.Draw(m_Texture, Position, Source, m_TintColor, RotationInRadians,
                        Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
                }
            }

            if (Parent)
            {
                foreach (Sprite child in Children)
                {
                    if (child.OnTopOfParent && child.Active)
                    {
                        Services.SpriteBatch.Draw(child.Texture, child.Position, child.Source, m_TintColor, child.RotationInRadians,
                            Vector2.Zero, child.Scale, SpriteEffects.None, 0.0f);
                    }
                }
            }
        }
    }
}
