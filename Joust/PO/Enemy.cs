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

    public class Enemy : Sprite
    {
        Background m_Background;
        Sprite m_FlyingRight;
        Sprite m_FlyingLeft;
        Sprite m_NoRiderRight;
        Sprite m_NoRiderLeft;
        Sprite m_WalkingRight;
        Sprite m_WalkingLeft;
        Object m_GroundSensor;

        public Object GroundSensor
        {
            get { return m_GroundSensor; }
        }

        public Enemy(Game game) : base(game)
        {
            m_WalkingRight = new Sprite(game);
            //AddChild(m_WalkingRight, false, true);
            m_FlyingRight = new Sprite(game);
            //AddChild(m_FlyingRight, false, true);
            m_NoRiderRight = new Sprite(game);
            //AddChild(m_NoRiderRight, false, true);
            m_WalkingLeft = new Sprite(game);
            //AddChild(m_WalkingLeft, false, true);
            m_FlyingLeft = new Sprite(game);
            //AddChild(m_FlyingLeft, false, true);
            m_NoRiderLeft = new Sprite(game);
            //AddChild(m_NoRiderLeft, false, true);
            m_GroundSensor = new Object(game);
            //AddChild(m_GroundSensor, true, true);
        }

        public override void Initialize()
        {
            Scale = 3.7f;
            Visable = false;
            m_WalkingLeft.Active = false;
            m_WalkingRight.Active = false;
            m_FlyingLeft.Active = false;
            m_FlyingRight.Active = false;
            m_NoRiderLeft.Active = false;
            m_NoRiderRight.Active = false;

            base.Initialize();
        }

        public void LoadContent()
        {
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

        public override void BeginRun()
        {
            m_GroundSensor.AABB.Width = 22;
            m_GroundSensor.AABB.Height = 5;
            m_GroundSensor.ReletivePosition.Y = AABB.Height;
            m_GroundSensor.ReletivePosition.X = AABB.Width / 2;

            m_WalkingLeft.Active = true;
            m_WalkingRight.Active = true;
            m_FlyingLeft.Active = true;
            m_FlyingRight.Active = true;

            m_WalkingLeft.Animate = true;
            m_WalkingRight.Animate = true;
            m_FlyingLeft.Animate = true;
            m_FlyingRight.Animate = true;

            m_WalkingLeft.Position = new Vector2(m_Background.Pads[0].Position.X, m_Background.Pads[0].Position.Y - AABB.Height);
            m_WalkingRight.Position = new Vector2(m_Background.Pads[2].Position.X, m_Background.Pads[2].Position.Y - AABB.Height);
            m_FlyingLeft.Position = new Vector2(Serv.WindowWidth / 2, AABB.Height + 20);
            m_FlyingRight.Position = new Vector2(Serv.WindowWidth / 2, AABB.Height + Serv.WindowHeight / 3);


            Position = new Vector2(m_Background.Pads[1].Position.X + AABB.Width / 2, m_Background.Pads[1].Position.Y - AABB.Height);
            Visable = true;

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        void Spawn()
        {
            Visable = true;
        }

        void FlightMode()
        {
            Visable = false;
        }

        void GroundMode()
        {
            Visable = false;
        }

        void OnGround()
        {

        }

        void Gravity()
        {

        }
    }
}
