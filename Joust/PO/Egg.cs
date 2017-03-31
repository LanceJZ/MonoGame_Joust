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

    public class Egg : Sprite
    {
        Sprite m_HatchedEnemy;

        public Egg(Game game) : base(game)
        {
            m_HatchedEnemy = new Sprite(game);
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public override void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}
