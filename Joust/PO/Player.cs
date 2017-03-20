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
        Engine.Sprite m_Wing;

        public Player(Game game) : base(game)
        {
            m_Wing = new Engine.Sprite(game);
            AddChild(m_Wing, true, true);
        }

        public override void Initialize()
        {
            base.Initialize();
            Velocity.X = 100;
            Velocity.Y = 20;
            Scale = 2;
            m_Wing.Animate = true;
        }

        public void Texture(Texture2D texture)
        {
            Initialize(texture, new Rectangle(0, 0, 20, 20), new Vector2(100, 100), false);
            m_Wing.Initialize(texture, new Rectangle(20, 0, 20, 20), Vector2.Zero, false);
            m_Wing.AddFrame(new Rectangle(40, 0, 20, 20));
        }

        public override void BeginRun()
        {
            base.BeginRun();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position = Serv.CheckSideBorders(Position);
            Position = Serv.ClampTopBottom(Position, 40);
        }
    }
}
