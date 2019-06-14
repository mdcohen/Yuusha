using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class StatusBar : Label
    {
        #region Private Data
        private int m_xVisualAdj;
        private int m_yVisualAdj;
        private Enums.ELayoutType m_layoutType;
        private int m_percent;
        #endregion

        #region Public Properties
        public int Percent
        {
            get { return m_percent; }
        }
        #endregion

        public StatusBar(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors,
            Enums.ELayoutType layoutType) : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_textRectangle = rectangle; // TODO:
            m_text = text;
            m_textColor = textColor;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_borderAlpha = borderAlpha;
            m_textAlpha = textAlpha;
            TextAlignment = textAlignment;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            m_onDoubleClickEvent = onDoubleClickEvent;
            m_cursorOverride = cursorOverride;
            m_anchors = anchors;
            m_layoutType = layoutType;

            m_yVisualAdj = 0;
            m_xVisualAdj = 0;
            m_percent = 100;
        }

        public override void Draw(GameTime gameTime)
        {
            if (m_visualKey != null && m_visualKey.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visualKey + " ] for Control [ " + m_name + " ]");
                    m_visualKey.Key = ""; // clear visual key
                    return;
                }

                // get the visual info
                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
                // set source rectangle, adjust accordingly since this is a status bar
                Rectangle sourceRect = new Rectangle(vi.X + m_xVisualAdj, vi.Y + m_yVisualAdj, vi.Width - m_xVisualAdj, vi.Height - m_yVisualAdj);
                // set draw rectangle, adjust accordingly since this is a status bar
                Rectangle drawRect = new Rectangle(m_rectangle.X + m_xVisualAdj, m_rectangle.Y + m_yVisualAdj, m_rectangle.Width - m_xVisualAdj, m_rectangle.Height - m_yVisualAdj);

                if (m_dropShadow)
                {
                    Rectangle shadowRect = new Rectangle(drawRect.X + m_shadowDistance, drawRect.Y + m_shadowDistance, drawRect.Width, drawRect.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, sourceRect, shadowColor);
                }

                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], drawRect, sourceRect, m_tintColor);
            }
        }

        public void SetPercent(int percent)
        {
            m_percent = percent;

            if (m_percent >= 100)
            {
                m_percent = 100;
                m_xVisualAdj = 0;
                m_yVisualAdj = 0;
                return;
            }

            VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];

            if (m_percent <= 0)
            {
                m_percent = 0;
                m_xVisualAdj = vi.Width;
                m_yVisualAdj = vi.Height;
                return;
            }

            if (m_layoutType == Enums.ELayoutType.Horizontal)
                m_xVisualAdj = vi.Width - (int)((vi.Width * m_percent) / 100);
            else if (m_layoutType == Enums.ELayoutType.Vertical)
                m_yVisualAdj = vi.Height - (int)((vi.Height * m_percent) / 100);
        }
    }
}
