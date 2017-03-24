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
        Sprite m_SelfML;
        List<Sprite> m_RetractingShelfs;
        Texture2D m_BackgroundSheet;
        float m_Scale = 3.7f;

        public Background(Game game) : base(game)
        {
            m_SelfML = new Sprite(game);
            m_RetractingShelfs = new List<Sprite>();

            for (int i = 0; i < 2; i++)
            {
                m_RetractingShelfs.Add(new Sprite(game));
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
            m_SelfML.Initialize(m_BackgroundSheet, new Rectangle(133, 46, 69, 10), new Vector2(110 * m_Scale, 156 * m_Scale), m_Scale);
            m_RetractingShelfs[0].Initialize(m_BackgroundSheet, new Rectangle(0, 46, 80, 4), new Vector2(0, 204 * m_Scale), m_Scale);
            m_RetractingShelfs[1].Initialize(m_BackgroundSheet, new Rectangle(0, 46, 80, 4),
                new Vector2(Serv.WindowWidth - (80 * m_Scale), 204 * m_Scale), m_Scale);
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

        }

        void CheckPlayerLanded()
        {
            if (m_Player.AABB.Intersects(m_Ground.AABB))
            {
                m_Player.Landed();
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                if (m_Player.AABB.Intersects(m_RetractingShelfs[i].AABB))
                {
                    m_Player.Landed();
                    return;
                }
            }
        }
    }
}
