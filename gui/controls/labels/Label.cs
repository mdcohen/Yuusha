using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class Label : Control
    {
        protected Rectangle m_textRectangle;

        /// <summary>
        /// TextRectangle always modifies size to fit font.
        /// </summary>
        public bool EnlargenTextRectangle
        { get; set; }
        public Border Border
        { get; set; }
        public bool TextShadow
        { get; set; } = false;
        public int TextShadowDistance
        { get; set; } = 5;
        public Map.Direction TextShadowDirection
        { get; set; } = Map.Direction.Northwest;
        public byte TextShadowAlpha
        { get; set; } = 80;

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
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
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
            m_textRectangle = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);

            if(EnlargenTextRectangle)
            {
                if(BitmapFont.ActiveFonts[Font].MeasureString(Text) > m_textRectangle.Width)
                {
                    switch(TextAlignment)
                    {
                        case BitmapFont.TextAlignment.Left:
                            m_textRectangle.Width = BitmapFont.ActiveFonts[Font].MeasureString(Text);
                            break;
                        case BitmapFont.TextAlignment.Right:
                            m_textRectangle.Width = BitmapFont.ActiveFonts[Font].MeasureString(Text);
                            m_textRectangle.X = m_textRectangle.X - (m_textRectangle.Width - m_rectangle.Width);
                            break;
                        case BitmapFont.TextAlignment.Center:
                            m_textRectangle.Width = BitmapFont.ActiveFonts[Font].MeasureString(Text);
                            m_textRectangle.X = m_textRectangle.X - m_textRectangle.Width / 2;
                            break;
                    }
                }
            }

            base.Update(gameTime);

            if (Border != null) Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime);

            Color textColor;

            if (!m_disabled)
                textColor = new Color(m_textColor, TextAlpha);
            else
                textColor = new Color(ColorDisabledStandard, TextAlpha);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                // override BitmapFont sprite batch
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                // set font alignment
                BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                // draw string
                Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);
                // change color of text if mouse over text color is not null
                if (!string.IsNullOrEmpty(m_text) && m_text.Length > 0)
                {
                    // draw shadow
                    if (TextShadow)
                    {
                        Rectangle shadowRect = new Rectangle(rect.X + GetXShadow(TextShadowDirection, TextShadowDistance), rect.Y + GetYShadow(TextShadowDirection, TextShadowDistance), rect.Width, rect.Height);
                        Color shadowColor = new Color(Color.Black, TextShadowAlpha);
                        BitmapFont.ActiveFonts[Font].TextBox(shadowRect, shadowColor, m_text);
                    }

                    if (!m_disabled && m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
                    {
                        BitmapFont.ActiveFonts[Font].TextBox(rect, new Color(m_textOverColor, TextAlpha), m_text);
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
    }
}
