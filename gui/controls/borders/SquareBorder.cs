using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class SquareBorder : Border
    {
        protected int m_borderWidth;
        protected Rectangle m_topRectangle;
        protected Rectangle m_leftRectangle;
        protected Rectangle m_rightRectangle;
        protected Rectangle m_bottomRectangle;

        public int BorderWidth
        {
            get { return m_borderWidth; }
        }

        public SquareBorder(string name, string owner, int width, VisualKey visualKey, bool visualTiled, Color tintColor, int visualAlpha)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_borderWidth = width;
            m_visualKey = visualKey;
            m_visualTiled = visualTiled;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_visible = true;
            m_topRectangle = new Rectangle();
            m_leftRectangle = new Rectangle();
            m_rightRectangle = new Rectangle();
            m_bottomRectangle = new Rectangle();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Control owner = GuiManager.GetControl(m_owner);

            if (owner != null)
            {
                Point p = owner.Position;
                m_topRectangle = new Rectangle(p.X, p.Y, owner.Width, m_borderWidth);
                m_leftRectangle = new Rectangle(p.X, p.Y, m_borderWidth, owner.Height);
                m_rightRectangle = new Rectangle(p.X + owner.Width - m_borderWidth, p.Y, m_borderWidth, owner.Height);
                m_bottomRectangle = new Rectangle(p.X, p.Y + owner.Height - m_borderWidth, owner.Width, m_borderWidth);
            }
        }

        public void ManuallySetRectangles(Control owner)
        {
            if (owner != null)
            {
                Point p = owner.Position;
                m_topRectangle = new Rectangle(p.X, p.Y, owner.Width, m_borderWidth);
                m_leftRectangle = new Rectangle(p.X, p.Y, m_borderWidth, owner.Height);
                m_rightRectangle = new Rectangle(p.X + owner.Width - m_borderWidth, p.Y, m_borderWidth, owner.Height);
                m_bottomRectangle = new Rectangle(p.X, p.Y + owner.Height - m_borderWidth, owner.Width, m_borderWidth);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            if (m_visualKey != null && !string.IsNullOrEmpty(m_visualKey.Key))
            {
                if (!GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visualKey + " ] for Border [ " + m_name + " ]");
                    m_visualKey.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
                Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);
                Color color = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, VisualAlpha);

                if (m_disabled)
                    color = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, VisualAlpha);

                // Fixes overlapping border sides on lower alpha.
                Rectangle leftRect = new Rectangle(m_leftRectangle.X, m_leftRectangle.Y + BorderWidth, m_leftRectangle.Width, m_leftRectangle.Height - (BorderWidth * 2));
                Rectangle rightRect = new Rectangle(m_rightRectangle.X, m_rightRectangle.Y + BorderWidth, m_rightRectangle.Width, m_rightRectangle.Height - (BorderWidth * 2));

                try
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_topRectangle, sourceRect, color);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], leftRect, sourceRect, color);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], rightRect, sourceRect, color);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_bottomRectangle, sourceRect, color);
                }
                catch
                {
                    Utils.LogOnce("Failed to SpriteBatch.Draw texture [ " + vi.ParentTexture + "]");
                }
            }
        }
    }
}
