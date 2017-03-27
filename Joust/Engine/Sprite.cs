using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Joust.Engine
{
    public struct ColorData
    {
        public Color[] Data;

        public ColorData (Color[] data)
        {
            Data = data;
        }
    }

    public class Sprite : PositionedObject, IDrawComponent
    {

        #region Declarations
        Texture2D m_Texture;
        Color m_TintColor = Color.White;
        List<Rectangle> m_Frames = new List<Rectangle>();
        List<ColorData> m_ColorData = new List<ColorData>(); //This sprite's frame color data.
        Timer m_FrameTime;
        int m_SpriteWidth;
        int m_SpriteHeight;
        int m_CurrentFrame;
        public List<PositionedObject> SpriteChildren;
        public Rectangle AABBScaledToFrame;
        public bool Visable = true;
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

        public Color[] ColorData
        {
            get { return m_ColorData[Frame].Data; }
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
            AABBScaledToFrame = new Rectangle(0, 0, initialFrame.Width, initialFrame.Height);
            m_SpriteWidth = (int)(initialFrame.Width * Scale);
            m_SpriteHeight = (int)(initialFrame.Height * Scale);
            m_Frames.Add(initialFrame);
            m_ColorData.Add(new ColorData(new Color[initialFrame.Width * initialFrame.Height]));
            m_Texture.GetData<Color>(0, initialFrame, m_ColorData[0].Data, 0, m_ColorData[0].Data.Length);
        }

        public void AddFrame(Rectangle frameRectangle)
        {
            m_Frames.Add(frameRectangle);
            m_ColorData.Add(new ColorData(new Color[frameRectangle.Width * frameRectangle.Height]));
            m_Texture.GetData<Color>(0, frameRectangle, m_ColorData[m_ColorData.Count - 1].Data, 0, m_ColorData[m_ColorData.Count - 1].Data.Length);
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
        /// <summary>
        /// Per Pixel Collusion. Pass in the position of the target sprite, target rectangle scaled to the actual sprite
        /// size along with the color data.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetScaledAABB"></param> This is the rectangle scaled to the actual sprite texture size.
        /// <param name="targetColorData"></param>
        /// <returns></returns>
        public bool PerPixelCollusion(Vector2 targetPosition, Rectangle targetScaledAABB, Color[] targetColorData)
        {
            //Move rectangles into scaled position.
            AABBScaledToFrame.X = (int)(Position.X / Scale);
            AABBScaledToFrame.Y = (int)(Position.Y / Scale);
            targetScaledAABB.X = (int)(targetPosition.X / Scale);
            targetScaledAABB.Y = (int)(targetPosition.Y / Scale);
            // Find the bounds of the rectangle intersection
            int top = Math.Max(AABBScaledToFrame.Top, targetScaledAABB.Top);
            int bottom = Math.Min(AABBScaledToFrame.Bottom, targetScaledAABB.Bottom);
            int left = Math.Max(AABBScaledToFrame.Left, targetScaledAABB.Left);
            int right = Math.Min(AABBScaledToFrame.Right, targetScaledAABB.Right);

            // Check every point within the intersection bounds
            for (int pointY = top; pointY < bottom; pointY++)
            {
                for (int pointX = left; pointX < right; pointX++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = ColorData[(pointX - AABBScaledToFrame.Left) + (pointY - AABBScaledToFrame.Top)
                        * AABBScaledToFrame.Width];
                    Color colorB = targetColorData[(pointX - targetScaledAABB.Left) + (pointY - targetScaledAABB.Top)
                        * targetScaledAABB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            return false;
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
