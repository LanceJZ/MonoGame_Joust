using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Joust.Engine
{
    public class Sprite : PositionedObject, IDrawComponent
    {
        #region Declarations
        Texture2D m_Texture;
        Color m_TintColor = Color.White;
        List<Rectangle> m_Frames = new List<Rectangle>();
        Timer m_FrameTime;
        int m_SpriteWidth;
        int m_SpriteHeight;
        int m_CurrentFrame;
        public bool Visable = true;
        public List<PositionedObject> SpriteChildren;
        public bool Animate = false;
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
            set { m_CurrentFrame = (int)MathHelper.Clamp(value, 0, m_Frames.Count - 1); }
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

        public Vector2 SpriteSize
        {
            get { return new Vector2(m_SpriteWidth, m_SpriteHeight); }
        }

        public int SpriteWidth
        {
            get { return m_SpriteWidth; }
        }

        public int SpriteHeight
        {
            get { return m_SpriteHeight; }
        }

        #endregion
        public Sprite(Game game) : base(game)
        {
            m_FrameTime = new Timer(game);
            SpriteChildren = new List<PositionedObject>();
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
        public void Initialize(Texture2D texture, Rectangle initialFrame, Vector2 position, float scale, bool animate)
        {
            Animate = animate;
            Initialize(texture, initialFrame, position, scale);
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
            m_SpriteWidth = (int)(initialFrame.Width * Scale);
            m_SpriteHeight = (int)(initialFrame.Height * Scale);
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

        public override void AddChild(PositionedObject child, bool activeDependent, bool directConnection)
        {
            if (child is Sprite)
            {
                SpriteChildren.Add(child);
                SpriteChildren[SpriteChildren.Count - 1].ActiveDependent = activeDependent;
                SpriteChildren[SpriteChildren.Count - 1].DirectConnection = directConnection;
                SpriteChildren[SpriteChildren.Count - 1].Child = true;
                Parent = true;
            }

            base.AddChild(child, activeDependent, directConnection);
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
            if (Active && !Child && Visable)
            {
                if (m_Frames.Count > 0)
                {
                    Services.SpriteBatch.Draw(m_Texture, Position, Source, m_TintColor, RotationInRadians,
                        Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
                }
            }

            if (Parent)
            {
                foreach (Sprite child in SpriteChildren)
                {
                    if (child.Active && child.Visable)
                    {
                        Services.SpriteBatch.Draw(child.Texture, child.Position, child.Source, m_TintColor, child.RotationInRadians,
                            Vector2.Zero, child.Scale, SpriteEffects.None, 0.0f);
                    }
                }
            }
        }
    }
}
