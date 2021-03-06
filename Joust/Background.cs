﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace Joust
{
    using Serv = Engine.Services;
    using Timer = Engine.Timer;
    using Object = Engine.PositionedObject;
    using Sprite = Engine.Sprite;

    public class Background : GameComponent, Engine.IUpdateableComponent, Engine.IBeginable
    {
        Sprite m_Ground;
        List<Sprite> m_Pads;
        List<Sprite> m_Shelfs;
        List<Sprite> m_Lava;
        List<Sprite> m_RetractingShelfs;
        float m_Scale = 3.7f;

        public List<Sprite> Pads { get => m_Pads;}
        public List<Sprite> Shelfs { get => m_Shelfs;}
        public List<Sprite> RetractingShelfs { get => m_RetractingShelfs;}
        public Sprite Ground { get => m_Ground; }

        public Background(Game game) : base(game)
        {
            m_Lava = new List<Sprite>();
            m_Pads = new List<Sprite>();
            m_Shelfs = new List<Sprite>();
            m_RetractingShelfs = new List<Sprite>();

            for (int i = 0; i < 7; i++)
            {
                m_Shelfs.Add(new Sprite(game));
            }

            for (int i = 0; i < 2; i++)
            {
                m_RetractingShelfs.Add(new Sprite(game));
            }

            for (int i = 0; i < 5; i++)
            {
                m_Lava.Add(new Sprite(game));
            }

            m_Ground = new Sprite(game);

            for (int i = 0; i < 4; i++)
            {
                m_Pads.Add(new Sprite(game));
            }
        }

        public override void Initialize()
        {

        }

        public void LoadContent()
        {
            Texture2D backgroundSheet = Game.Content.Load<Texture2D>(@"JoustBGSpriteSheet");
            m_Ground.Initialize(backgroundSheet, new Rectangle(0, 0, 204, 32),
                new Vector2(Serv.WindowWidth * 0.5f - ((204 * m_Scale) * 0.5f), 204 * m_Scale), m_Scale);
            m_Shelfs[0].Initialize(backgroundSheet, new Rectangle(133, 46, 69, 10), new Vector2(110 * m_Scale, 156 * m_Scale), m_Scale);
            m_Shelfs[1].Initialize(backgroundSheet, new Rectangle(136, 34, 56, 8), new Vector2(0, 131 * m_Scale), m_Scale);
            m_Shelfs[2].Initialize(backgroundSheet, new Rectangle(194, 34, 62, 11), new Vector2(220 * m_Scale, 122 * m_Scale), m_Scale);
            m_Shelfs[3].Initialize(backgroundSheet, new Rectangle(211, 4, 45, 8),
                new Vector2(Serv.WindowWidth - (45 * m_Scale), 131 * m_Scale), m_Scale);
            m_Shelfs[4].Initialize(backgroundSheet, new Rectangle(225, 13, 31, 7), new Vector2(0, 62 * m_Scale), m_Scale);
            m_Shelfs[5].Initialize(backgroundSheet, new Rectangle(66, 34, 68, 8), new Vector2(88 * m_Scale, 74 * m_Scale), m_Scale);
            m_Shelfs[6].Initialize(backgroundSheet, new Rectangle(204, 46, 52, 7),
                new Vector2(Serv.WindowWidth - (52 * m_Scale), 62 * m_Scale), m_Scale);
            m_RetractingShelfs[0].Initialize(backgroundSheet, new Rectangle(0, 46, 80, 4), new Vector2(0, 204 * m_Scale), m_Scale);
            m_RetractingShelfs[1].Initialize(backgroundSheet, new Rectangle(0, 46, 80, 4),
                new Vector2(Serv.WindowWidth - (80 * m_Scale), 204 * m_Scale), m_Scale);
            m_Pads[0].Initialize(backgroundSheet, new Rectangle(226, 0, 30, 3), new Vector2(110 * m_Scale, 74 * m_Scale), m_Scale);
            m_Pads[1].Initialize(backgroundSheet, new Rectangle(226, 0, 30, 3), new Vector2(11 * m_Scale, 131 * m_Scale), m_Scale);
            m_Pads[2].Initialize(backgroundSheet, new Rectangle(226, 0, 30, 3), new Vector2(239 * m_Scale, 122 * m_Scale), m_Scale);
            m_Pads[3].Initialize(backgroundSheet, new Rectangle(226, 0, 30, 3), new Vector2(125 * m_Scale, 204 * m_Scale), m_Scale);
            // Pad 0 is Top. Pad 1 is Left. Pad 2 is Right. Pad 3 is Bottom.

            for (int i = 0; i < 5; i++)
            {
                m_Lava[i].Initialize(backgroundSheet, new Rectangle(0, 34, 64, 11),
                    new Vector2(240 * i, 232 * m_Scale), 4);
            }
        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}
