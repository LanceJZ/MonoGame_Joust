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
    using Object = Engine.PositionedObject;
    using Sprite = Engine.Sprite;

    public class Player : Sprite
    {
        Sprite m_WingRight;
        Sprite m_WingLeft;
        Sprite m_RunRight;
        Sprite m_RunLeft;
        KeyboardState m_KeyState, m_KeyStateOld;
        Object m_GroundSensor;
        int m_SpriteSize;
        bool m_GoLeftKeyDown;
        bool m_GoRightKeyDown;
        bool m_Stoped;
        bool m_OnGround;

        public Object GroundSensor
        {
            get { return m_GroundSensor; }
        }

        public Player(Game game) : base(game)
        {
            m_WingRight = new Sprite(game);
            AddChild(m_WingRight, false, true);
            m_WingLeft = new Sprite(game);
            AddChild(m_WingLeft, false, true);
            m_RunRight = new Sprite(game);
            AddChild(m_RunRight, false, true);
            m_RunLeft = new Sprite(game);
            AddChild(m_RunLeft, false, true);
            m_GroundSensor = new Object(game);
            AddChild(m_GroundSensor, true, false);
        }

        public override void Initialize()
        {
            Scale = 3.7f;

            base.Initialize();
        }

        public void LoadContent()
        {
            Texture2D playerSheet = Game.Content.Load<Texture2D>(@"JoustSpriteSheet");
            m_SpriteSize = 20;
            Initialize(playerSheet, new Rectangle(0, 0, m_SpriteSize, m_SpriteSize), Vector2.Zero, Scale, false); //Standing Right 0
            AddFrame(new Rectangle(0, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));//Standing Left 1
            AddFrame(new Rectangle(0, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize));//Breaking Right 2
            AddFrame(new Rectangle(0, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize));//Breaking Left 3
            AddFrame(new Rectangle(0, m_SpriteSize * 4 + 4, m_SpriteSize, m_SpriteSize));//Flying Right 4
            AddFrame(new Rectangle(0, m_SpriteSize * 5 + 5, m_SpriteSize, m_SpriteSize));//Flying Left 5
            m_RunRight.Initialize(playerSheet, new Rectangle(m_SpriteSize + 1, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize),
                Vector2.Zero, Scale, true);//Running Right 0
            m_RunRight.AddFrame(new Rectangle(m_SpriteSize * 2 + 2, m_SpriteSize * 2 + 2, m_SpriteSize, m_SpriteSize));//Running Right 1
            m_RunLeft.Initialize(playerSheet, new Rectangle(m_SpriteSize + 1, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize),
                Vector2.Zero, Scale, true);//Running Left 0
            m_RunLeft.AddFrame(new Rectangle(m_SpriteSize * 2 + 2, m_SpriteSize * 3 + 3, m_SpriteSize, m_SpriteSize));//Running Left 1
            m_WingRight.Initialize(playerSheet, new Rectangle(m_SpriteSize + 1, 0, m_SpriteSize, m_SpriteSize), Vector2.Zero,
                Scale, true);
            m_WingRight.AddFrame(new Rectangle(m_SpriteSize * 2 + 1, 0, m_SpriteSize, m_SpriteSize));
            m_WingRight.AddFrame(new Rectangle(m_SpriteSize * 3 + 1, 0, m_SpriteSize, m_SpriteSize));
            m_WingRight.Moveable = false;
            m_WingLeft.Initialize(playerSheet, new Rectangle(m_SpriteSize + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize),
                Vector2.Zero, Scale, true);
            m_WingLeft.AddFrame(new Rectangle(m_SpriteSize * 2 + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));
            m_WingLeft.AddFrame(new Rectangle(m_SpriteSize * 3 + 1, m_SpriteSize + 1, m_SpriteSize, m_SpriteSize));
            m_WingLeft.Moveable = false;
        }

        public override void BeginRun()
        {
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            Position = new Vector2(Serv.WindowWidth * 0.25f, 204 * Scale - m_SpriteSize * Scale);
            m_GroundSensor.AABB.Width = 22;
            m_GroundSensor.AABB.Height = 5;
            m_GroundSensor.ReletivePosition.Y = SpriteHeight;
            m_GroundSensor.ReletivePosition.X = SpriteWidth / 2;
            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            Position = Serv.CheckSideBorders(Position, AABB.Width);

            if (Serv.HitTop(Position))
            {
                Acceleration.Y = 0;
                Velocity.Y = (Velocity.Y * 0.25f) * -1;
                Position.Y = 1;
            }

            if (m_OnGround)
                OnGround();
            else
                Gravity();

            m_KeyState = Keyboard.GetState();
            KeyInput();
            m_KeyStateOld = m_KeyState;

            base.Update(gameTime);
        }

        public void Bumped(Vector2 position)
        {
            Acceleration = Vector2.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += Serv.SetVelocityFromAngle(Serv.AngleFromVectors(position, Position), 75);
        }

        public void Landed(float groundY)
        {
            Acceleration.Y = 0;
            Velocity.Y = 0;
            Position.Y = groundY - AABB.Height;
            m_OnGround = true;

            if (Frame == 4)
                Frame = 0;

            if (Frame == 5)
                Frame = 1;

        }

        public void InAir()
        {
            if (m_OnGround)
            {
                m_OnGround = false;
                FlightMode();
                Glide();
            }
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

            if (Frame == 4)
            {
                m_WingRight.Animate = false;
                m_WingRight.Frame = 1;
            }
            else if (Frame == 5)
            {
                m_WingLeft.Animate = false;
                m_WingLeft.Frame = 1;
            }
        }

        void FlightMode()
        {
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            Visable = true;

            if (Frame == 0)
                Frame = 4;

            if (Frame == 1)
                Frame = 5;
        }

        void Flap()
        {
            Acceleration.Y = -50;
            Position.Y -= 1;
            m_OnGround = false;

            FlightMode();

            if (Frame == 4)
            {
                m_WingRight.Active = true;
                m_WingRight.Animate = true;
                m_WingLeft.Active = false;

                if (Velocity.X < 400 && m_GoRightKeyDown)
                    Acceleration.X = 120;
            }
            else if (Frame == 5)
            {
                m_WingLeft.Active = true;
                m_WingLeft.Animate = true;
                m_WingRight.Active = false;


                if (Velocity.X > -400 && m_GoLeftKeyDown)
                    Acceleration.X = -120;
            }
        }

        void GoRight()
        {
            m_WingLeft.Active = false;
            m_GoRightKeyDown = true;

            if (m_OnGround)
            {
                if (!m_Stoped)
                {
                    Acceleration.X = 175;
                    Frame = 0;
                }
                else
                {
                    Frame = 1;
                }


                if (Velocity.X < 0 && !m_Stoped)
                {
                    m_RunLeft.Active = false;
                    Visable = true;
                    Frame = 3;
                    Acceleration.X = 250;

                    if (Velocity.X > -10)
                    {
                        m_Stoped = true;
                        Velocity.X = 0;
                        Acceleration.X = 0;
                    }
                }
            }
        }

        void GoLeft()
        {
            m_WingRight.Active = false;
            m_GoLeftKeyDown = true;

            if (m_OnGround)
            {
                if (!m_Stoped)
                {
                    Acceleration.X = -175;
                    Frame = 1;
                }
                else
                {
                    Frame = 0;
                }


                if (Velocity.X > 0 && !m_Stoped)
                {
                    m_RunRight.Active = false;
                    Visable = true;
                    Frame = 2;
                    Acceleration.X = -250;

                    if (Velocity.X < 10)
                    {
                        m_Stoped = true;
                        Velocity.X = 0;
                        Acceleration.X = 0;
                    }
                }
            }
        }

        void OnGround()
        {
            m_WingRight.Active = false;
            m_WingLeft.Active = false;
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            Visable = true;
            Acceleration.X = 0;

            if (Velocity.X > 0 && !m_Stoped)
            {
                m_RunRight.Active = true;
                Frame = 0;
                Visable = false;
            }

            if (Velocity.X < 0 && !m_Stoped)
            {
                m_RunLeft.Active = true;
                Frame = 1;
                Visable = false;
            }

            if (!m_GoLeftKeyDown && !m_GoRightKeyDown)
                m_Stoped = false;
        }

        void Gravity()
        {
            if (Position.Y < Serv.WindowHeight - Scale * m_SpriteSize)
            {
                if (m_GoRightKeyDown)
                {
                    Frame = 4;
                }
                else if (m_GoLeftKeyDown)
                {
                    Frame = 5;
                }

                if (Velocity.Y < 200)
                {
                    if (Acceleration.Y < 0)
                        Acceleration.Y += 2;
                    else
                        Acceleration.Y = 80;
                }

                if (Frame == 2)
                    Frame = 0;

                if (Frame == 3)
                    Frame = 1;
            }
        }
    }
}
