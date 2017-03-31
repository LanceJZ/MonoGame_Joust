using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Joust
{
    using Serv = Engine.Services;
    using Timer = Engine.Timer;
    using Object = Engine.PositionedObject;
    using Sprite = Engine.Sprite;

    public class EnemyControl : GameComponent, Engine.IBeginable
    {
        List<PO.Enemy> m_Enemys;
        PO.Player m_Player;
        Background m_Background;

        public EnemyControl(Game game) : base(game)
        {
            m_Enemys = new List<PO.Enemy>();

            for (int i = 0; i < 3; i++)
            {
                m_Enemys.Add(new PO.Enemy(game));
            }
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void BeginRun()
        {
            //Position = new Vector2(m_Background.Pads[1].Position.X + AABB.Width / 2, m_Background.Pads[1].Position.Y - AABB.Height);
            for (int i = 0; i < 3; i++)
            {
                m_Enemys[i].PlayerReference(m_Player);
                m_Enemys[i].BackgroundReference(m_Background);

                m_Enemys[i].Position = new Vector2(m_Background.Pads[i].Position.X + m_Enemys[i].AABB.Width / 2,
                    m_Background.Pads[i].Position.Y - m_Enemys[i].AABB.Height);
            }
        }

        public void BackgroundReference(Background background)
        {
            m_Background = background;
        }

        public void PlayerReference(PO.Player player)
        {
            m_Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            EnemyCollides();
        }

        void EnemyCollides()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int ii = 0; ii < 2; ii++)
                {
                    int test = 0;

                    if (i == 0)
                        test = ii + 1;

                    if (i == 1)
                    {
                        test = ii * 2;
                    }

                    if (i == 2)
                    {
                        test = ii;
                    }

                    if (m_Enemys[i].AABB.Intersects(m_Enemys[test].AABB))
                    {
                        if (m_Enemys[i].PerPixelCollision(m_Enemys[test].Position, m_Enemys[test].AABBScaledToFrame, m_Enemys[test].ColorData))
                        {
                            m_Enemys[i].Bumped(m_Enemys[test].Position);
                            m_Enemys[test].Bumped(m_Enemys[i].Position);
                        }
                    }
                }
            }
        }
    }
}
