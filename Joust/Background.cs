using Microsoft.Xna.Framework;
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

    public class Background : GameComponent, Engine.IUpdateableComponent
    {
        PO.Player m_Player;
        Sprite m_Ground;
        //Sprite m_ShelfML;
        //Sprite m_ShelfLL;
        //Sprite m_ShelfMR;
        //Sprite m_ShelfLR;
        //Sprite m_ShelfUL;
        //Sprite m_ShelfUM;
        //Sprite m_ShelfUR;
        List<Sprite> m_Pads;
        List<Sprite> m_Shelfs;
        List<Sprite> m_RetractingShelfs;
        Texture2D m_BackgroundSheet;
        float m_Scale = 3.7f;

        public Background(Game game) : base(game)
        {
            //m_ShelfML = new Sprite(game);
            //m_ShelfLL = new Sprite(game);
            //m_ShelfMR = new Sprite(game);
            //m_ShelfUL = new Sprite(game);
            //m_ShelfUM = new Sprite(game);
            //m_ShelfLR = new Sprite(game);
            //m_ShelfUR = new Sprite(game);
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

            for (int i = 0; i < 4; i++)
            {
                m_Pads.Add(new Sprite(game));
            }

            m_Ground = new Sprite(game);
        }

        public override void Initialize()
        {

        }

        public void LoadContent()
        {
            m_BackgroundSheet = Game.Content.Load<Texture2D>(@"JoustBGSpriteSheet");
            m_Ground.Initialize(m_BackgroundSheet, new Rectangle(0, 0, 204, 32),
                new Vector2(Serv.WindowWidth * 0.5f - ((204 * m_Scale) * 0.5f), 204 * m_Scale), m_Scale);
            //m_ShelfML.Initialize(m_BackgroundSheet, new Rectangle(133, 46, 69, 10), new Vector2(110 * m_Scale, 156 * m_Scale), m_Scale);
            m_Shelfs[0].Initialize(m_BackgroundSheet, new Rectangle(133, 46, 69, 10), new Vector2(110 * m_Scale, 156 * m_Scale), m_Scale);
            //m_ShelfLL.Initialize(m_BackgroundSheet, new Rectangle(136, 34, 56, 8), new Vector2(0, 131 * m_Scale), m_Scale);
            m_Shelfs[1].Initialize(m_BackgroundSheet, new Rectangle(136, 34, 56, 8), new Vector2(0, 131 * m_Scale), m_Scale);
            //m_ShelfMR.Initialize(m_BackgroundSheet, new Rectangle(194, 34, 62, 11), new Vector2(220 * m_Scale, 122 * m_Scale), m_Scale);
            m_Shelfs[2].Initialize(m_BackgroundSheet, new Rectangle(194, 34, 62, 11), new Vector2(220 * m_Scale, 122 * m_Scale), m_Scale);
            //m_ShelfLR.Initialize(m_BackgroundSheet, new Rectangle(211, 4, 45, 8),
              //  new Vector2(Serv.WindowWidth - (45 * m_Scale), 131 * m_Scale), m_Scale);
            m_Shelfs[3].Initialize(m_BackgroundSheet, new Rectangle(211, 4, 45, 8),
                new Vector2(Serv.WindowWidth - (45 * m_Scale), 131 * m_Scale), m_Scale);
            //m_ShelfUL.Initialize(m_BackgroundSheet, new Rectangle(225, 13, 31, 7), new Vector2(0, 62 * m_Scale), m_Scale);
            m_Shelfs[4].Initialize(m_BackgroundSheet, new Rectangle(225, 13, 31, 7), new Vector2(0, 62 * m_Scale), m_Scale);
            //m_ShelfUM.Initialize(m_BackgroundSheet, new Rectangle(66, 34, 68, 8), new Vector2(88 * m_Scale, 74 * m_Scale), m_Scale);
            m_Shelfs[5].Initialize(m_BackgroundSheet, new Rectangle(66, 34, 68, 8), new Vector2(88 * m_Scale, 74 * m_Scale), m_Scale);
            //m_ShelfUR.Initialize(m_BackgroundSheet, new Rectangle(204, 46, 52, 7),
            //    new Vector2(Serv.WindowWidth - (52 * m_Scale), 62 * m_Scale), m_Scale);
            m_Shelfs[6].Initialize(m_BackgroundSheet, new Rectangle(204, 46, 52, 7),
                new Vector2(Serv.WindowWidth - (52 * m_Scale), 62 * m_Scale), m_Scale);
            m_RetractingShelfs[0].Initialize(m_BackgroundSheet, new Rectangle(0, 46, 80, 4), new Vector2(0, 204 * m_Scale), m_Scale);
            m_RetractingShelfs[1].Initialize(m_BackgroundSheet, new Rectangle(0, 46, 80, 4),
                new Vector2(Serv.WindowWidth - (80 * m_Scale), 204 * m_Scale), m_Scale);
            m_Pads[0].Initialize(m_BackgroundSheet, new Rectangle(226, 0, 30, 3), new Vector2(110 * m_Scale, 74 * m_Scale), m_Scale);
        }

        public void BeginRun()
        {

        }

        public void PlayerReference(PO.Player player)
        {
            m_Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CheckPlayerLanded();
            CheckPlayerBumpedShelf();
        }

        void CheckPlayerBumpedShelf()
        {
            for (int i = 0; i < 7; i++)
            {
                if (m_Player.AABB.Intersects(m_Shelfs[i].AABB))
                {
                    m_Player.Bumped(m_Shelfs[i].Position);
                    return;
                }
            }
        }

        void CheckPlayerLanded()
        {
            if (m_Player.GroundSensor.AABB.Intersects(m_Ground.AABB))
            {
                m_Player.Landed();
                return;
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    if (m_Player.GroundSensor.AABB.Intersects(m_Shelfs[i].AABB))
                    {
                        m_Player.Landed();
                        return;
                    }
                    else
                    {
                        for (int ii = 0; ii < 2; ii++)
                        {
                            if (m_Player.GroundSensor.AABB.Intersects(m_RetractingShelfs[ii].AABB))
                            {
                                m_Player.Landed();
                                return;
                            }
                        }
                    }
                }
            }

            m_Player.InAir();
        }
    }
}
