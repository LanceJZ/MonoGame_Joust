using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Joust.PO
{
    using Engine;

    public class Player : BirdControl
    {
        Background m_Background;
        Sprite m_FlyingRight;
        Sprite m_FlyingLeft;
        Sprite m_RunRight;
        Sprite m_RunLeft;
        Sprite m_FlyWOPlayerRight;
        Sprite m_FlyWOPlayerLeft;
        KeyboardState m_KeyState, m_KeyStateOld;
        PositionedObject m_GroundSensor;
        PositionedObject m_LanceSensor;
        Timer m_TurnAround;
        Mode CurrentMode;
        Direction KeyDown;

        public PositionedObject LanceSensor
        {
            get { return m_LanceSensor; }
        }

        public Player(Game game) : base(game)
        {
            m_FlyingRight = new Sprite(game);
            AddChild(m_FlyingRight, false, true);
            m_FlyingLeft = new Sprite(game);
            AddChild(m_FlyingLeft, false, true);
            m_RunRight = new Sprite(game);
            AddChild(m_RunRight, false, true);
            m_RunLeft = new Sprite(game);
            AddChild(m_RunLeft, false, true);
            m_FlyWOPlayerRight = new Sprite(game);
            AddChild(m_FlyWOPlayerRight, false, true);
            m_FlyWOPlayerLeft = new Sprite(game);
            AddChild(m_FlyWOPlayerLeft, false, true);
            m_GroundSensor = new PositionedObject(game);
            AddChild(m_GroundSensor, true, false);
            m_LanceSensor = new PositionedObject(game);
            AddChild(m_LanceSensor, true, false);
            m_TurnAround = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Scale = 3.7f;
            m_TurnAround.Amount = 0.33f;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Texture2D spriteSheet = Game.Content.Load<Texture2D>(@"PlayerOneSpriteSheet");
            int spriteSize = 20;
            Initialize(spriteSheet, new Rectangle(0, 0, spriteSize, spriteSize), Vector2.Zero, Scale, false); //Standing Right 0
            AddFrame(new Rectangle(spriteSize + 1, 0, spriteSize, spriteSize));//Standing Left 1
            AddFrame(new Rectangle(0, spriteSize * 2 + 2, spriteSize, spriteSize));//Breaking Right 2
            AddFrame(new Rectangle(spriteSize + 1, spriteSize * 2 + 2, spriteSize, spriteSize));//Breaking Left 3
            m_RunRight.Initialize(spriteSheet, new Rectangle(0, spriteSize * 3 + 3, spriteSize, spriteSize),
                Vector2.Zero, Scale, true);//Running Right 0
            m_RunRight.AddFrame(new Rectangle(spriteSize + 1, spriteSize * 3 + 3, spriteSize, spriteSize));//Running Right 1
            m_RunLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 2 + 2, spriteSize * 3 + 3, spriteSize, spriteSize),
                Vector2.Zero, Scale, true);//Running Left 0
            m_RunLeft.AddFrame(new Rectangle(spriteSize * 3 + 3, spriteSize * 3 + 3, spriteSize, spriteSize));//Running Left 1
            m_FlyingRight.Initialize(spriteSheet, new Rectangle(0, spriteSize + 1, spriteSize, spriteSize), Vector2.Zero,
                Scale, true);
            m_FlyingRight.AddFrame(new Rectangle(spriteSize + 1, spriteSize + 1, spriteSize, spriteSize));
            m_FlyingRight.AddFrame(new Rectangle(spriteSize * 2 + 2, spriteSize + 1, spriteSize, spriteSize));
            m_FlyingLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 3 + 3, spriteSize + 1, spriteSize, spriteSize),
                Vector2.Zero, Scale, true);
            m_FlyingLeft.AddFrame(new Rectangle(spriteSize * 4 + 4, spriteSize + 1, spriteSize, spriteSize));
            m_FlyingLeft.AddFrame(new Rectangle(spriteSize * 5 + 5, spriteSize + 1, spriteSize, spriteSize));
            m_FlyWOPlayerRight.Initialize(spriteSheet, new Rectangle(0, spriteSize * 4 + 4, spriteSize, spriteSize),
                Vector2.Zero, Scale, false);
            m_FlyWOPlayerRight.AddFrame(new Rectangle(spriteSize + 1, spriteSize * 4 + 4, spriteSize, spriteSize));
            m_FlyWOPlayerRight.AddFrame(new Rectangle(spriteSize * 2 + 2, spriteSize * 4 + 4, spriteSize, spriteSize));
            m_FlyWOPlayerLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 3 + 3, spriteSize * 4 + 4, spriteSize, spriteSize),
                Vector2.Zero, Scale, false);
            m_FlyWOPlayerLeft.AddFrame(new Rectangle(spriteSize * 4 + 4, spriteSize * 4 + 4, spriteSize, spriteSize));
            m_FlyWOPlayerLeft.AddFrame(new Rectangle(spriteSize * 5 + 5, spriteSize * 4 + 4, spriteSize, spriteSize));
        }

        public void BackgroundReference(Background background)
        {
            m_Background = background;
        }

        public override void BeginRun()
        {
            base.BeginRun();

            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            m_FlyingLeft.Active = false;
            m_FlyingRight.Active = false;
            m_FlyWOPlayerRight.Active = false;
            m_FlyWOPlayerLeft.Active = false;
            Position = new Vector2(Services.WindowWidth * 0.25f, 204 * Scale - AABB.Height);

            m_GroundSensor.AABB.Width = 22;
            m_GroundSensor.AABB.Height = 5;
            m_GroundSensor.ReletivePosition.Y = AABB.Height;
            m_GroundSensor.ReletivePosition.X = AABB.Width / 2;

            m_LanceSensor.AABB.Width = AABB.Width;
            m_LanceSensor.AABB.Height = 11;
            m_LanceSensor.ReletivePosition.Y = 15;
            m_LanceSensor.ReletivePosition.X = AABB.Width / 2;

            CurrentMode = Mode.Stopped;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position = Services.CheckSideBorders(Position, AABB.Width);

            if (Services.HitTop(Position))
            {
                Acceleration.Y = 0;
                Velocity.Y = (Velocity.Y * 0.25f) * -1;
                Position.Y = 1;
            }

            if (TouchedGround())
            {

            }
            else
            {
                switch (CurrentMode)
                {
                    case Mode.Flight:
                        BumpedShelf();
                        break;

                    case Mode.Walking:
                        SwitchToFlightMode();
                        break;
                }
            }

            switch (CurrentMode)
            {
                case Mode.Walking:
                    OnGround();
                    break;

                case Mode.Stopped:
                    break;

                case Mode.Start:
                    break;

                case Mode.Beamin:
                    break;

                case Mode.BirdEscape:
                    BirdEscapes();
                    break;

                case Mode.Flight:
                    Glide();
                    break;
            }

            m_KeyState = Keyboard.GetState();
            KeyInput();
            m_KeyStateOld = m_KeyState;
        }

        public void JoustLost()
        {
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Position = m_Background.Pads[(int)Services.RandomMinMax(0, 3.9f)].Position;
            Position.X += AABB.Width / 2;
            Position.Y -= AABB.Height;
        }

        public void Bumped(Vector2 position)
        {
            Acceleration = Vector2.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += Services.SetVelocityFromAngle(Services.AngleFromVectors(position, Position), 75);
        }

        void BirdEscapes()
        {

        }

        void SwitchToWalkMode(float groundY)
        {
            if (CurrentMode == Mode.Flight)
            {
                Acceleration.Y = 0;
                Velocity.Y = 0;
                Position.Y = groundY - AABB.Height;
                m_FlyingLeft.Active = false;
                m_FlyingRight.Active = false;
                CurrentMode = Mode.Walking;
                Visable = true;

                if (Frame == 2)
                    Frame = 0;

                if (Frame == 3)
                    Frame = 1;
            }
        }

        void KeyInput() // Player stops then turns around when direction change on ground is held down.
        {
            if (!m_KeyStateOld.IsKeyDown(Keys.LeftControl) && !m_KeyStateOld.IsKeyDown(Keys.Space))
            {
                if (m_KeyState.IsKeyDown(Keys.LeftControl) || m_KeyState.IsKeyDown(Keys.Space))
                {
                    Flap();
                }
                else if (CurrentMode == Mode.Flight)
                {
                    Glide();
                }
            }

            KeyDown = Direction.None;

            if (m_KeyState.IsKeyDown(Keys.Left))
                GoLeft();

            if (m_KeyState.IsKeyDown(Keys.Right))
                GoRight();
        }

        void Glide()
        {
            Acceleration.X = 0;

            if (m_FlyingRight.Active)
            {
                m_FlyingRight.Animate = false;
                m_FlyingRight.Frame = 1;
            }
            else if (m_FlyingLeft.Active)
            {
                m_FlyingLeft.Animate = false;
                m_FlyingLeft.Frame = 1;
            }

            if (Position.Y < Services.WindowHeight - AABB.Height)
            {
                if (Velocity.Y < 200)
                {
                    if (Acceleration.Y < 0)
                        Acceleration.Y += 2;
                    else
                        Acceleration.Y = 80;
                }
            }
        }

        void SwitchToFlightMode()
        {
            if (CurrentMode != Mode.Flight)
            {
                Visable = false;
                CurrentMode = Mode.Flight;
                m_RunLeft.Active = false;
                m_RunRight.Active = false;

                if (Frame == 0 || Frame == 2)
                    m_FlyingRight.Active = true;

                if (Frame == 1 || Frame == 1)
                    m_FlyingLeft.Active = true;

                Glide();
            }
        }

        void Flap()
        {
            Acceleration.Y = -50;
            Position.Y -= 1;

            if (CurrentMode != Mode.Flight)
                SwitchToFlightMode();

            if (m_FlyingRight.Active)
            {
                if (Velocity.X < 500 && KeyDown == Direction.Right)
                    Acceleration.X = 1000;

                m_FlyingRight.Animate = true;
            }
            else if (m_FlyingLeft.Active)
            {

                if (Velocity.X > -500 && KeyDown == Direction.Left)
                    Acceleration.X = -1000;

                m_FlyingLeft.Animate = true;
            }
        }

        void GoRight()
        {
            KeyDown = Direction.Right;

            switch (CurrentMode)
            {
                case Mode.Flight:
                    m_FlyingRight.Active = true;
                    m_FlyingLeft.Active = false;
                    break;

                case Mode.Walking:
                    if (Velocity.X < 600)
                        Acceleration.X = 175;

                    Frame = 0;

                    if (Velocity.X < 0)
                    {
                        m_RunLeft.Active = false;
                        Visable = true;
                        Frame = 3;
                        Acceleration.X = 250;

                        if (Velocity.X > -10)
                        {
                            CurrentMode = Mode.Stopped;
                            Velocity.X = 0;
                            Acceleration.X = 0;
                            m_TurnAround.Reset();
                        }
                    }
                    break;

                case Mode.Stopped:
                    Frame = 1;

                    if (m_TurnAround.Expired)
                    {
                        CurrentMode = Mode.Walking;
                    }
                    break;
            }
        }

        void GoLeft()
        {
            KeyDown = Direction.Left;

            switch (CurrentMode)
            {
                case Mode.Flight:
                    m_FlyingLeft.Active = true;
                    m_FlyingRight.Active = false;
                    break;

                case Mode.Walking:
                    if (Velocity.X > -600)
                        Acceleration.X = -175;

                    if (Velocity.X > 0)
                    {
                        Frame = 1;
                        m_RunRight.Active = false;
                        Visable = true;
                        Frame = 2;
                        Acceleration.X = -250;

                        if (Velocity.X < 10)
                        {
                            CurrentMode = Mode.Stopped;
                            Velocity.X = 0;
                            Acceleration.X = 0;
                            m_TurnAround.Reset();
                        }
                    }
                    break;

                case Mode.Stopped:
                    Frame = 0;
                if (m_TurnAround.Expired)
                    {
                        CurrentMode = Mode.Walking;
                    }
                    break;
            }
        }

        void OnGround()
        {
            m_FlyingRight.Active = false;
            m_FlyingLeft.Active = false;
            m_RunLeft.Active = false;
            m_RunRight.Active = false;
            Visable = true;
            Acceleration.X = 0;

            if (Velocity.X > 0)
            {
                m_RunRight.Active = true;
                Frame = 0;
                Visable = false;
            }
            else if (Velocity.X < 0)
            {
                m_RunLeft.Active = true;
                Frame = 1;
                Visable = false;
            }
        }

        void BumpedShelf()
        {
            for (int i = 0; i < 7; i++)
            {
                if (AABB.Intersects(m_Background.Shelfs[i].AABB))
                {
                    if (PerPixelCollision(m_Background.Shelfs[i].Position, m_Background.Shelfs[i].AABBScaledToFrame,
                        m_Background.Shelfs[i].ColorData))
                    {
                        Bumped(m_Background.Shelfs[i].Position);
                        return;
                    }
                }
            }
        }

        bool TouchedGround()
        {
            if (m_GroundSensor.AABB.Intersects(m_Background.Ground.AABB))
            {
                if (AABB.Intersects(m_Background.Ground.AABB))
                    SwitchToWalkMode(m_Background.Ground.AABB.Top);

                return true;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    if (m_GroundSensor.AABB.Intersects(m_Background.Shelfs[i].AABB))
                    {
                        if (AABB.Intersects(m_Background.Shelfs[i].AABB))
                            SwitchToWalkMode(m_Background.Shelfs[i].AABB.Top);

                        return true;
                    }
                    else
                    {
                        for (int ii = 0; ii < 2; ii++)
                        {
                            if (m_GroundSensor.AABB.Intersects(m_Background.RetractingShelfs[ii].AABB))
                            {
                                if (AABB.Intersects(m_Background.RetractingShelfs[ii].AABB))
                                    SwitchToWalkMode(m_Background.RetractingShelfs[ii].AABB.Top);

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
