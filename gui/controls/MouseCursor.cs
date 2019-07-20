using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class MouseCursor : Control
    {
        private int m_xDrawOffset;
        private int m_yDrawOffset;
        
        public Control DraggedControl
        { get; set; }

        public List<TextCue> TextCues { get; }

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
            TextCues = new List<TextCue>();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = GuiManager.MouseState;

            if(Utility.Settings.StaticSettings.DisplayMouseCursorCoordinates)
                TextCue.AddMouseCursorTextCue(ms.Position.ToString());

            //if (DraggedButton != null)
            //    DraggedButton.Update(gameTime);

            //if (GuiManager.Dragging)
            //{
            //    this.Position = new Point(GuiManager.DraggedControl.Position.X + GuiManager.DraggingXOffset,
            //        GuiManager.DraggedControl.Position.Y + GuiManager.DraggingYOffset);
            //}
            //else
            //{
                Position = new Point(ms.X, ms.Y);
            //}

            base.Update(gameTime);

            if(DraggedControl != null)
            {
                DraggedControl.Position = new Point(Position.X + 1, Position.Y + 1);
                DraggedControl.ZDepth = 0;
            }

            // update text
            for (int a = GuiManager.Cursors["Normal"].TextCues.Count - 1; a >= 0; a--)
            {
                TextCue tc = GuiManager.Cursors["Normal"].TextCues[a];
                tc.Update(gameTime, TextCues);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            // Instead of messing with zDepth, this does another call to draw the dragged button to confirm it is visible just "under" the mouse cursor.
            if (DraggedControl != null)
                DraggedControl.Draw(gameTime);

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
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], destRect, sourceRect, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                }
                catch
                {
                    Utils.LogOnce("Failed to SpriteBatch.Draw texture [ " + vi.ParentTexture + "]");
                }
            }

            // Draw text last.
            foreach (TextCue tc in GuiManager.Cursors["Normal"].TextCues)
                tc.Draw(gameTime);
        }
    }
}
