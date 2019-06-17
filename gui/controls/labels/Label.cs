using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class Label : Control
    {
        protected Rectangle m_textRectangle;
        private Border m_border;

        public Border Border
        {
            get { return m_border; }
            set { m_border = value; }
        }

        public Label()
            : base()
        {
            TextAlignment = BitmapFont.TextAlignment.Left;
            m_textOverColor = new Color();
            m_tintColor = new Color();
            m_hasTextOverColor = false;
            m_textRectangle = m_rectangle; // TODO:
            PopUpText = "";
            m_onDoubleClickEvent = "";
        }

        public Label(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText)
            : base()
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
            PopUpText = popUpText;
        }

        public override void Update(GameTime gameTime)
        {
            m_textRectangle = m_rectangle;

            base.Update(gameTime);

            if (m_border != null) m_border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            Color textColor;

            if (!m_disabled)
                textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, TextAlpha);
            else
                textColor = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, TextAlpha);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                // override BitmapFont sprite batch
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                // set font alignment
                BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                // draw string
                Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);
                // change color of text if mouse over text color is not null
                if (m_text != null && m_text.Length > 0)
                {
                    if (!m_disabled && m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
                    {
                        BitmapFont.ActiveFonts[Font].TextBox(rect, m_textOverColor, m_text);
                    }
                    else
                    {
                        BitmapFont.ActiveFonts[Font].TextBox(rect, textColor, m_text);
                    }
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for Label [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");

            if (Border != null) Border.Draw(gameTime);
        }

        protected override void OnMouseOver(MouseState ms)
        {
            if (m_cursorOverride != "" && GuiManager.Cursors.ContainsKey(m_cursorOverride))
                GuiManager.CurrentSheet.CursorOverride = m_cursorOverride;
        }
    }
}
