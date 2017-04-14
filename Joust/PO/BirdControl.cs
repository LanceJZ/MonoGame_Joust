using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Joust.PO
{
    using Engine;

    public class BirdControl : Sprite
    {
        protected enum Mode
        {
            Beamin,
            Start,
            Stopped,
            Walking,
            Flight,
            BirdEscape
        };

        protected enum Direction
        {
            None,
            Right,
            Left
        };

        public BirdControl(Game game) : base(game)
        {

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
