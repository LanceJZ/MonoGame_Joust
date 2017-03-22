using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Joust.PO
{
    using Serv = Engine.Services;
    using Timer = Engine.Timer;
    using PO = Engine.PositionedObject;

    public class Player : Engine.Sprite
    {
        Engine.Sprite m_PlayerSprite;
        Engine.Sprite m_WingRight;
        Engine.Sprite m_WingLeft;
        Engine.Sprite m_RunRight;
        Engine.Sprite m_RunLeft;
        KeyboardState m_KeyState, m_KeyStateOld;
        int m_SpriteSize;
        bool m_GoLeftKeyDown;
        bool m_GoRightKeyDown;
        bool m_Stoped;
        bool m_OnGround;

        public Player(Game game) : base(game)
        {
            m_PlayerSprite = new Engine.Sprite(game);
            AddChild(m_PlayerSprite, false, true);
            m_WingRight = new Engine.Sprite(game);
            AddChild(m_WingRight, false, true);
            m_WingLeft = new Engine.Sprite(game);
            AddChild(m_WingLeft, false, true);
            m_RunRight = new Engine.Sprite(game);
            AddChild(m_RunRight, false, true);
            m_RunLeft = new Engine.Sprite(game);
            AddChild(m_RunLeft, false, true);
        }

        public override void Initialize()
        {
            base.Initialize();
            Scale = 3;
        }

        public void LoadTexture(Texture2D texture)
        {
            m_SpriteSize = 20;

            m_PlayerSprite.Initialize(texture, new Rectangle(0, 0, m_SpriteSize, m_SpriteSize), Vector2.Zero, true); //Standing Right 0
            m_PlayerSprite.AddFrame(new Rectangle(0, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));//Standing Left 1
            m_PlayerSprite.AddFrame(new Rectangle(0, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize));//Breaking Right 2
            m_PlayerSprite.AddFrame(new Rectangle(0, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize));//Breaking Left 3
            m_PlayerSprite.AddFrame(new Rectangle(0, m_SpriteSize * 4 + 4, m_SpriteSize, m_SpriteSize));//Flying Right 4
            m_PlayerSprite.AddFrame(new Rectangle(0, m_SpriteSize * 5 + 5, m_SpriteSize, m_SpriteSize));//Flying Left 5
            m_RunRight.Initialize(texture, new Rectangle(m_SpriteSize + 1, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize),
                Vector2.Zero, true);//Running Right 0
            m_RunRight.AddFrame(new Rectangle(m_SpriteSize * 2 + 2, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize));//Running Right 1
            m_RunLeft.Initialize(texture, new Rectangle(m_SpriteSize + 1, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize),
                Vector2.Zero, true);//Running Left 0
            m_RunLeft.AddFrame(new Rectangle(m_SpriteSize * 2 + 2, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize));//Running Left 1
            m_WingRight.Initialize(texture, new Rectangle(m_SpriteSize + 1, 0, m_SpriteSize, m_SpriteSize), Vector2.Zero, true);
            m_WingRight.AddFrame(new Rectangle(m_SpriteSize * 2 + 1, 0, m_SpriteSize, m_SpriteSize));
            m_WingRight.AddFrame(new Rectangle(m_SpriteSize * 3 + 1, 0, m_SpriteSize, m_SpriteSize));
            m_WingRight.Moveable = false;
            m_WingLeft.Initialize(texture, new Rectangle(m_SpriteSize + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize), Vector2.Zero, true);
            m_WingLeft.AddFrame(new Rectangle(m_SpriteSize * 2 + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));
            m_WingLeft.AddFrame(new Rectangle(m_SpriteSize * 3 + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));
            m_WingLeft.Moveable = false;
        }

        public override void BeginRun()
        {
            base.BeginRun();
            m_RunLeft.Active = false;
            m_RunLeft.Animate  = true;
            m_RunRight.Active = false;
            m_RunRight.Animate = true;
            m_PlayerSprite.Animate = false;
            Position = new Vector2(Serv.WindowWidth * 0.25f, Serv.WindowHeight - m_SpriteSize);
        }

        public override void Update(GameTime gameTime)
        {
            Position = Serv.CheckSideBorders(Position);
            Position = Serv.ClampTopBottom(Position, Scale * m_SpriteSize);

            Gravity();

            if (m_OnGround)
                OnGround();

            m_KeyState = Keyboard.GetState();
            KeyInput();
            m_KeyStateOld = m_KeyState;

            base.Update(gameTime);
        }

        void KeyInput()
        {
            if (!m_KeyStateOld.IsKeyDown(Keys.LeftControl) && !m_KeyStateOld.IsKeyDown(Keys.Space))
            {
                if (m_KeyState.IsKeyDown(Keys.LeftControl) || m_KeyState.IsKeyDown(Keys.Space))
                {
                    Flap();
                }
                else if (!m_OnGround)
                {
                    Glide();
                }
            }

            if (m_KeyState.IsKeyDown(Keys.Left))
                GoLeft();
            else
                m_GoLeftKeyDown = false;

            if (m_KeyState.IsKeyDown(Keys.Right))
                GoRight();
            else
                m_GoRightKeyDown = false;
        }

        void Glide()
        {
            Acceleration.X = 0;

            if (m_PlayerSprite.Frame == 4)
            {
                m_WingRight.Animate = false;
                m_WingRight.Frame = 1;
            }
            else if (m_PlayerSprite.Frame == 5)
            {
                m_WingLeft.Animate = false;
                m_WingLeft.Frame = 1;
            }
        }

        void Flap()
        {
            Acceleration.Y = -40;
            Position.Y -= 1;
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            m_PlayerSprite.Active = true;

            if (m_PlayerSprite.Frame == 0)
                m_PlayerSprite.Frame = 4;

            if (m_PlayerSprite.Frame == 1)
                m_PlayerSprite.Frame = 5;

            if (m_PlayerSprite.Frame == 4)
            {
                m_WingRight.Active = true;
                m_WingRight.Animate = true;
                m_WingLeft.Active = false;

                if (Velocity.X < 400 && m_GoRightKeyDown)
                    Acceleration.X = 100;
            }
            else if (m_PlayerSprite.Frame == 5)
            {
                m_WingLeft.Active = true;
                m_WingLeft.Animate = true;
                m_WingRight.Active = false;


                if (Velocity.X > -400 && m_GoLeftKeyDown)
                    Acceleration.X = -100;
            }
        }

        void GoRight()
        {
            m_WingLeft.Active = false;
            m_GoRightKeyDown = true;

            if (m_OnGround)
            {
                Acceleration.X = 175;
                m_PlayerSprite.Frame = 0;

                if (Velocity.X < 0)
                {
                    m_RunLeft.Active = false;
                    m_PlayerSprite.Active = true;
                    m_PlayerSprite.Frame = 3;
                    Acceleration.X = 250;
                }
            }
        }

        void GoLeft()
        {
            m_WingRight.Active = false;
            m_GoLeftKeyDown = true;

            if (m_OnGround)
            {
                Acceleration.X = -175;
                m_PlayerSprite.Frame = 1;

                if (Velocity.X > 0)
                {
                    m_RunRight.Active = false;
                    m_PlayerSprite.Active = true;
                    m_PlayerSprite.Frame = 2;
                    Acceleration.X = -250;
                }
            }
        }

        void OnGround()
        {
            m_WingRight.Active = false;
            m_WingLeft.Active = false;
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            m_PlayerSprite.Active = true;
            Acceleration.X = 0;

            if (Velocity.X > 0)
            {
                m_RunRight.Active = true;
                m_PlayerSprite.Active = false;
            }

            if (Velocity.X < 0)
            {
                m_RunLeft.Active = true;
                m_PlayerSprite.Active = false;
            }
        }

        void Gravity()
        {
            if (Position.Y < Serv.WindowHeight - Scale * m_SpriteSize)
            {
                m_OnGround = false;

                if (m_GoRightKeyDown)
                {
                    m_PlayerSprite.Frame = 4;
                }
                else if (m_GoLeftKeyDown)
                {
                    m_PlayerSprite.Frame = 5;
                }

                if (Velocity.Y < 200)
                {
                    if (Acceleration.Y < 0)
                        Acceleration.Y += 2;
                    else
                        Acceleration.Y = 80;
                }

                if (m_PlayerSprite.Frame == 2)
                    m_PlayerSprite.Frame = 0;

                if (m_PlayerSprite.Frame == 3)
                    m_PlayerSprite.Frame = 1;
            }
            else
            {
                Position.Y = Serv.WindowHeight - Scale * m_SpriteSize;
                Acceleration.Y = 0;
                Velocity.Y = 0;
                m_OnGround = true;
            }
        }
    }
}
