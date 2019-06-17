using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class Background : Control
    {
        #region Private Data
        private bool m_centered = false;
        private List<TextCue> m_textCues;
        #endregion

        #region Public Properties
        public bool IsCentered
        {
            get { return m_centered; }
        }
        public List<TextCue> TextCues
        {
            get { return m_textCues; }
        }
        #endregion

        #region Constructor
        public Background(System.Xml.XmlReader reader)
        {
            m_visible = true; // by default

            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "VisualKey")
                    m_visualKey = new VisualKey(reader.Value);
                else if (reader.Name == "IsCentered")
                    m_centered = reader.ReadContentAsBoolean();
                else if (reader.Name == "IsTiled")
                    m_visualTiled = reader.ReadContentAsBoolean();
                else if (reader.Name == "TintColor")
                    m_tintColor = Utils.GetColor(reader.Value);
                else if (reader.Name == "VisualAlpha")
                    m_visualAlpha = reader.ReadContentAsInt();
                else if (reader.Name == "IsVisible")
                    m_visible = reader.ReadContentAsBoolean();
            }

            if (m_visualTiled)
                m_centered = false;

            m_textCues = new List<TextCue>();

            ZDepth = 10000;
        }

        public Background(string font, string visualKey, bool centered, bool tiled, Color tintColor, byte visualAlpha, bool visible)
        {
            m_font = font;
            m_visualKey = new VisualKey(visualKey);
            m_centered = centered;
            m_visualTiled = tiled;
            if (m_visualTiled)
                m_centered = false;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_visible = visible;
            m_textCues = new List<TextCue>();
        } 
        #endregion

        public override void Update(GameTime gameTime)
        {
            for (int a = m_textCues.Count - 1; a >= 0; a--)
                m_textCues[a].Update(gameTime, m_textCues);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            VisualInfo vi = null;

            try
            {
                vi = GuiManager.Visuals[m_visualKey.Key];
            }
            catch
            {
            }

            if (vi != null)
            {
                if (!m_centered)
                {
                    if (!m_visualTiled)
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(0, 0, Client.Width, Client.Height), m_tintColor);
                    }
                    else
                    {
                        int xAmount = (int)(Client.Width / vi.Width) + 1;
                        int yAmount = (int)(Client.Height / vi.Height) + 1;
                        for (int x = 0; x <= xAmount; x++)
                        {
                            for (int y = 0; y <= yAmount; y++)
                            {
                                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(x * vi.Width, y * vi.Height, vi.Width, vi.Height), vi.Rectangle, m_tintColor);
                            }
                        }
                    }
                }
                else
                {
                    int x = (int)Client.Width / 2 - (int)vi.Width / 2;
                    int y = (int)Client.Height / 2 - (int)vi.Height / 2;
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(x, y, vi.Width, vi.Height), m_tintColor);
                }
            }

            // draw strings
            foreach (TextCue tc in m_textCues)
            {
                tc.Draw(gameTime);
            }
        }
    }
}
