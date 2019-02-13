using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class MouseCursor : Control
    {
        private List<TextCue> m_textCues;
        private int m_xDrawOffset;
        private int m_yDrawOffset;
        //private Point m_PreviousPosition;
        //private GameTime m_lastActivity;

        public List<TextCue> TextCues
        {
            get { return m_textCues; }
        }

        public MouseCursor(System.Xml.XmlTextReader reader)
        {
            m_xDrawOffset = 0;
            m_yDrawOffset = 0;

            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "VisualKey")
                    m_visualKey = new VisualKey(reader.Value);
                else if (reader.Name == "Font")
                    m_font = reader.Value;
                else if (reader.Name == "XDrawOffset")
                    m_xDrawOffset = reader.ReadContentAsInt();
                else if (reader.Name == "YDrawOffset")
                    m_yDrawOffset = reader.ReadContentAsInt();
            }

            m_visible = true;
            VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
            m_rectangle = new Rectangle(0, 0, vi.Width, vi.Height);
            m_textCues = new List<TextCue>();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = GuiManager.MouseState;
            
            if (GuiManager.Dragging)
            {
                // this is used to compensate for fast dragging
                this.Position = new Point(GuiManager.DraggedControl.Position.X + GuiManager.DraggingXOffset,
                    GuiManager.DraggedControl.Position.Y + GuiManager.DraggingYOffset);
            }
            else
            {
                this.Position = new Point(ms.X, ms.Y);
            }

            base.Update(gameTime);

            // update text
            for (int a = m_textCues.Count - 1; a >= 0; a--)
            {
                TextCue tc = m_textCues[a];
                tc.Update(gameTime, m_textCues);
                if (m_textCues.Contains(tc))
                {
                    if (BitmapFont.ActiveFonts.ContainsKey(tc.Font))
                    {
                        tc.X = ms.X;
                        tc.Y = ms.Y - BitmapFont.ActiveFonts[tc.Font].LineHeight;

                        while (tc.X < 0) tc.X++;

                        while (tc.Y < 0) tc.Y++;

                        while (tc.X + BitmapFont.ActiveFonts[tc.Font].MeasureString(tc.Text) > Client.Width)
                            tc.X--;

                        while (tc.Y > Client.Height) tc.Y--;
                    }
                    else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + tc.Font + " ] for TextCue [ " + tc.Text + " ] ");
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            if (m_visualKey != null && m_visualKey.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visualKey + " ] for Cursor [ " + m_name + " ]");
                    m_visualKey.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
                Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);
                Rectangle destRect = new Rectangle(m_rectangle.X + m_xDrawOffset, m_rectangle.Y + m_yDrawOffset, vi.Width, vi.Height);

                try
                {
                    if (!m_disabled)
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], destRect, sourceRect, m_tintColor);
                    else
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], destRect, sourceRect, new Color(s_disabledColor.R, s_disabledColor.G, s_disabledColor.B, m_visualAlpha));
                }
                catch
                {
                    Utils.LogOnce("Failed to SpriteBatch.Draw texture [ " + vi.ParentTexture + "]");
                }
            }

            // Draw text last.
            foreach (TextCue tc in m_textCues)
                tc.Draw(gameTime);
        }
    }
}
