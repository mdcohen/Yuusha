using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class SplashScreen : GameComponent
    {
        private int m_step;
        private string m_cue1 = "KSNDint1";
        private string m_cue2 = "KSNDint2";
        private string m_cue3 = "KSNDint3";
        private Cue m_nowPlaying;
        private VisualKey m_nowVisualKey;

        public SplashScreen(Game game)
            : base(game)
        {
            m_cue1 = "KSNDint1";
            m_cue2 = "KSNDint2";
            m_cue3 = "KSNDint3";
            m_nowVisualKey = new VisualKey("");
            m_step = 0;
        }

        public void SkipSplash()
        {
            if (m_nowPlaying != null)
            {
                m_nowPlaying.Stop(AudioStopOptions.Immediate);
            }

            m_step++;
        }

        public override void Update(GameTime gameTime)
        {
            if (Client.PreferredWindowWidth != 600 || Client.PreferredWindowHeight != 400)
            {
                Client.PreferredWindowWidth = 600;
                Client.PreferredWindowHeight = 400;
            }

            Yuusha.KeyboardHandler.HandleKeyboard();

            Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);

            switch (m_step)
            {
                case 0:
                    m_step = 1;
                    break;
                case 1:
                    m_nowVisualKey.Key = "Splash1";
                    if (m_nowPlaying == null)
                    {
                        //m_nowPlaying = Sound.GetCue(m_cue1);
                        m_nowPlaying.Play();
                    }
                    else if (!m_nowPlaying.IsPlaying)
                        m_step = 2;
                    break;
                case 2:
                    m_nowVisualKey.Key = "Splash2";
                    if (m_nowPlaying.Name != m_cue2)
                    {
                        //m_nowPlaying = Sound.GetCue(m_cue2);
                        m_nowPlaying.Play();
                    }
                    else if (!m_nowPlaying.IsPlaying)
                        m_step = 3;
                    break;
                case 3:
                    m_nowVisualKey.Key = "Splash3";
                    if (m_nowPlaying.Name != m_cue3)
                    {
                        //m_nowPlaying = Sound.GetCue(m_cue3);
                        m_nowPlaying.Play();
                    }
                    else if (!m_nowPlaying.IsPlaying)
                        m_step = 4;
                    break;
                default:
                    Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
                    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
                    this.Enabled = false;
                    break;
            }

            base.Update(gameTime);
        }

        public void Draw()
        {
            Client.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (m_nowVisualKey.Key.Length > 0)
            {
                VisualInfo vi = GuiManager.Visuals[m_nowVisualKey.Key];
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(0, 0, vi.Width, vi.Height), Color.White);
            }

            Client.SpriteBatch.End();
        }
    }
}
