using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Joust.PO
{
    using Engine;

    public class Enemy : BirdControl
    {
        Background m_Background;
        Player m_Player;
        Sprite m_FlyingRight;
        Sprite m_FlyingLeft;
        Sprite m_NoRiderRight;
        Sprite m_NoRiderLeft;
        Sprite m_WalkingRight;
        Sprite m_WalkingLeft;
        Sprite m_RescueRight;
        Sprite m_RescueLeft;
        PositionedObject m_GroundSensor;
        PositionedObject m_LanceSensor;
        Timer m_WakeFromSpawn;
        Timer m_FlapTimer;
        Timer m_TouchedBottomTimer;
        Timer m_SwitchDirection;
        bool m_Awake;
        bool m_WalkMode;
        bool m_FlightMode;
        bool m_TouchedBottom;

        public PositionedObject GroundSensor
        {
            get { return m_GroundSensor; }
        }

        public Enemy(Game game) : base(game)
        {
            m_WalkingRight = new Sprite(game);
            AddChild(m_WalkingRight, false, true);
            m_FlyingRight = new Sprite(game);
            AddChild(m_FlyingRight, false, true);
            m_NoRiderRight = new Sprite(game);
            AddChild(m_NoRiderRight, false, true);
            m_WalkingLeft = new Sprite(game);
            AddChild(m_WalkingLeft, false, true);
            m_FlyingLeft = new Sprite(game);
            AddChild(m_FlyingLeft, false, true);
            m_NoRiderLeft = new Sprite(game);
            AddChild(m_NoRiderLeft, false, true);
            m_GroundSensor = new PositionedObject(game);
            AddChild(m_GroundSensor, true, false);
            m_LanceSensor = new PositionedObject(game);
            AddChild(m_LanceSensor, true, false);

            m_WakeFromSpawn = new Timer(game);
            m_FlapTimer = new Timer(game);
            m_TouchedBottomTimer = new Timer(game);
            m_SwitchDirection = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();

            Scale = 3.7f;
            m_WakeFromSpawn.Amount = 1;
            m_TouchedBottomTimer.Amount = 2;
            m_SwitchDirection.Amount = 5;
            Disable();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Texture2D spriteSheet = Game.Content.Load<Texture2D>(@"EnemyOneSpriteSheet");
            int spriteSize = 20;
            Initialize(spriteSheet, new Rectangle(0, spriteSize * 3 + 3, spriteSize, spriteSize), Vector2.Zero, Scale, false); //Flying Left One
            m_FlyingRight.Initialize(spriteSheet, new Rectangle(0, 0, spriteSize, spriteSize), Vector2.Zero, Scale, false);
            m_FlyingRight.AddFrame(new Rectangle(spriteSize + 1, 0, spriteSize, spriteSize));
            m_FlyingLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 2 + 2, 0, spriteSize, spriteSize), Vector2.Zero, Scale, false);
            m_FlyingLeft.AddFrame(new Rectangle(spriteSize * 3 + 3, 0, spriteSize, spriteSize));
            m_WalkingRight.Initialize(spriteSheet, new Rectangle(0, spriteSize + 1, spriteSize, spriteSize), Vector2.Zero, Scale, false);
            m_WalkingRight.AddFrame(new Rectangle(spriteSize + 1, spriteSize + 1, spriteSize, spriteSize));
            m_WalkingLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 2 + 2, spriteSize + 1, spriteSize, spriteSize),
                Vector2.Zero, Scale, false);
            m_WalkingLeft.AddFrame(new Rectangle(spriteSize * 3 + 3, spriteSize + 1, spriteSize, spriteSize));
            m_NoRiderRight.Initialize(spriteSheet, new Rectangle(0, spriteSize * 2 + 2, spriteSize, spriteSize), Vector2.Zero, Scale, false);
            m_NoRiderRight.AddFrame(new Rectangle(spriteSize + 1, spriteSize * 2 + 2, spriteSize, spriteSize));
            m_NoRiderLeft.Initialize(spriteSheet, new Rectangle(spriteSize * 2 + 2, spriteSize * 2 + 2, spriteSize, spriteSize),
                Vector2.Zero, Scale, false);
            m_NoRiderLeft.AddFrame(new Rectangle(spriteSize * 3 + 3, spriteSize * 2 + 2, spriteSize, spriteSize));
        }

        public void BackgroundReference(Background background)
        {
            m_Background = background;
        }

        public void PlayerReference(Player player)
        {
            m_Player = player;
        }

        public override void BeginRun()
        {
            m_GroundSensor.AABB.Width = 22;
            m_GroundSensor.AABB.Height = 5;
            m_GroundSensor.ReletivePosition.Y = AABB.Height;
            m_GroundSensor.ReletivePosition.X = AABB.Width / 2;

            m_LanceSensor.AABB.Width = AABB.Width;
            m_LanceSensor.AABB.Height = 11;
            m_LanceSensor.ReletivePosition.Y = 19;
            m_LanceSensor.ReletivePosition.X = AABB.Width / 2;

            m_WalkingLeft.Animate = true;
            m_WalkingRight.Animate = true;
            m_FlyingLeft.Animate = true;
            m_FlyingRight.Animate = true;

            Visable = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!m_Awake)
            {
                if (m_WakeFromSpawn.Expired)
                {
                    m_Awake = true;
                    m_WalkingRight.Active = true;
                    m_WalkMode = true;
                    Visable = false;
                    m_FlapTimer.Amount = 0.5f;
                    Velocity.X = 100;
                }
            }
            else
            {
                if (m_FlightMode)
                    InAir();
                else if (m_WalkMode)
                    OnGround();

                if (m_FlapTimer.Expired && m_FlightMode)
                {
                    Flap();

                    if (m_TouchedBottom)
                    {
                        m_FlapTimer.Amount = Services.RandomMinMax(0.05f, 0.25f);
                    }
                    else
                    {
                        m_FlapTimer.Amount = Services.RandomMinMax(0.05f, 0.36f);
                    }
                }

                Position = Services.CheckSideBorders(Position, AABB.Width);

                if (Services.HitTop(Position))
                {
                    Acceleration.Y = 0;
                    Velocity.Y = (Velocity.Y * 0.25f) * -1;
                    Position.Y = 1;
                    m_TouchedBottom = false;
                }

                if (m_SwitchDirection.Expired)
                {
                    m_SwitchDirection.Amount = Services.RandomMinMax(4, 15);
                    Switch();
                }
            }

            NearPlayer();
        }

        void Spawn(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Visable = true;
            Active = true;
            m_Awake = false;
            m_WakeFromSpawn.Reset();
        }

        void Switch()
        {
            if (m_FlyingLeft.Active)
            {
                m_FlyingLeft.Active = false;
                m_FlyingRight.Active = true;
                Velocity.X *= 0.1f;
                return;
            }

            if (m_FlyingRight.Active)
            {
                m_FlyingLeft.Active = true;
                m_FlyingRight.Active = false;
                Velocity.X *= 0.1f;
                return;
            }

            if (m_WalkingLeft.Active)
            {
                m_WalkingLeft.Active = false;
                m_WalkingRight.Active = true;
                Velocity.X = 100;
                return;
            }

            if (m_WalkingRight.Active)
            {
                m_WalkingLeft.Active = true;
                m_WalkingRight.Active = false;
                Velocity.X = -100;
            }
        }

        void FlightMode() //Flight mode puts enemy into that mode.
        {
            Visable = false;
            m_FlightMode = true;
            m_WalkMode = false;

            if (m_WalkingLeft.Active)
            {
                m_FlyingLeft.Active = true;
                m_WalkingLeft.Active = false;
            }
            else if (m_WalkingRight.Active)
            {
                m_FlyingRight.Active = true;
                m_WalkingRight.Active = false;
            }

        }

        void WalkMode(int topOfGround)
        {
            Visable = false;
            m_WalkMode = true;
            m_FlightMode = false;
            Acceleration.Y = 0;
            Velocity.Y = 0;
            Position.Y = topOfGround - AABB.Height;

            if (m_FlyingLeft.Active)
            {
                m_WalkingLeft.Active = true;
                m_FlyingLeft.Active = false;
            }
            else if (m_FlyingRight.Active)
            {
                m_WalkingRight.Active = true;
                m_FlyingRight.Active = false;
            }
        }

        void OnGround()
        {
            if (!TouchGround())
                FlightMode();
            else
            {
                if (m_TouchedBottom)
                {
                    if (m_TouchedBottomTimer.Expired)
                    {
                        Flap();
                        m_TouchedBottomTimer.Amount = Services.RandomMinMax(1, 3);
                    }
                }
            }
        }

        void InAir()
        {
            if(!TouchGround())
            {
                if (Velocity.Y < 200)
                {
                    if (Acceleration.Y < 0)
                        Acceleration.Y += 2;
                    else
                        Acceleration.Y = 80;
                }

                BumpShelf();
            }
        }

        void Flap()
        {
            if (m_WalkMode)
            {
                FlightMode();
            }

            Position.Y -= 2;

            if (Velocity.Y < 100)
                Acceleration.Y = -20;

            if (m_FlyingRight.Active)
            {
                if (Velocity.X < 300)
                    Acceleration.X = 100;
                else
                    Acceleration.X = 0;

                return;
            }

            if (m_FlyingLeft.Active)
            {
                if (Velocity.X > -300)
                    Acceleration.X = -100;
                else
                    Acceleration.X = 0;
            }
        }

        public void Bumped(Vector2 position)
        {
            Acceleration = Vector2.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += Services.SetVelocityFromAngle(Services.AngleFromVectors(position, Position), 75);
        }

        public void JoustLost()
        {
            Active = false;
            Disable();
        }

        void BumpPlayer()
        {

            if (PerPixelCollision(m_Player.Position, m_Player.AABBScaledToFrame, m_Player.ColorData))
            {
                if (m_LanceSensor.AABB.Intersects(m_Player.LanceSensor.AABB))
                {
                    Bumped(m_Player.Position);
                    m_Player.Bumped(Position);
                    return;
                }

                if (m_LanceSensor.AABB.Bottom < m_Player.LanceSensor.AABB.Top)
                {
                    m_Player.JoustLost();
                }

                if (m_LanceSensor.AABB.Top > m_Player.LanceSensor.AABB.Bottom)
                {
                    //JoustLost();
                    Disable();
                    Spawn(m_Background.Pads[(int)Services.RandomMinMax(0, 2.9f)].Position + new Vector2(AABB.Width / 2, -AABB.Height));
                }

            }
        }

        void NearPlayer()
        {
            if (AABB.Intersects(m_Player.AABB))
                BumpPlayer();
        }

        void BumpShelf()
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

        bool TouchGround()
        {
            if (GroundSensor.AABB.Intersects(m_Background.Ground.AABB))
            {
                if (AABB.Intersects(m_Background.Ground.AABB))
                {
                    m_TouchedBottom = true;
                    m_TouchedBottomTimer.Reset();
                    WalkMode(m_Background.Ground.AABB.Top);
                }

                return true;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    if (GroundSensor.AABB.Intersects(m_Background.Shelfs[i].AABB))
                    {
                        if (AABB.Intersects(m_Background.Shelfs[i].AABB))
                            WalkMode(m_Background.Shelfs[i].AABB.Top);

                        return true;
                    }
                    else
                    {
                        for (int ii = 0; ii < 2; ii++)
                        {
                            if (GroundSensor.AABB.Intersects(m_Background.RetractingShelfs[ii].AABB))
                            {
                                if (AABB.Intersects(m_Background.RetractingShelfs[ii].AABB))
                                {
                                    m_TouchedBottom = true;
                                    m_TouchedBottomTimer.Reset();
                                    WalkMode(m_Background.RetractingShelfs[ii].AABB.Top);
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        void Disable()
        {
            Visable = false;
            m_WalkingLeft.Active = false;
            m_WalkingRight.Active = false;
            m_FlyingLeft.Active = false;
            m_FlyingRight.Active = false;
            m_NoRiderLeft.Active = false;
            m_NoRiderRight.Active = false;

        }
    }
}
