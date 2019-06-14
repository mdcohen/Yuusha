using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class Button : Control
    {        
        protected bool m_onMouseDownSent = false;
        public bool IsTextVisible
        { get; set; }

        public Button(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor,
            System.Collections.Generic.List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection,
            int shadowDistance, string command, string popUpText)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_text = text;
            IsTextVisible = textVisible;
            m_textColor = textColor;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_borderAlpha = borderAlpha;
            m_textAlpha = textAlpha;

            if (m_visualKey.Key != "")
                m_visuals.Add(Enums.EControlState.Normal, m_visualKey);
            if (visualKeyOver.Key != "")
                m_visuals.Add(Enums.EControlState.Over, visualKeyOver);
            if (visualKeyDown.Key != "")
                m_visuals.Add(Enums.EControlState.Down, visualKeyDown);
            if (visualKeyDisabled.Key != "")
                m_visuals.Add(Enums.EControlState.Disabled, visualKeyDisabled);

            m_onMouseDown = onMouseDownEvent;
            TextAlignment = textAlignment;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            m_textOverColor = textOverColor;
            m_hasTextOverColor = hasTextOverColor;
            m_tintOverColor = tintOverColor;
            m_hasTintOverColor = hasTintOverColor;
            m_anchors = anchors;
            m_dropShadow = dropShadow;
            m_shadowDirection = shadowDirection;
            m_shadowDistance = shadowDistance;
            Command = command;
            m_popUpText = popUpText;

            m_onMouseDownSent = false;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            if (m_text.Length > 0 && IsTextVisible)
            {
                if (BitmapFont.ActiveFonts.ContainsKey(Font))
                {
                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                    // draw string in textbox, using x and y text offsets to create new rectangle
                    Rectangle rect = new Rectangle(m_rectangle.X + XTextOffset, m_rectangle.Y + YTextOffset, m_rectangle.Width, m_rectangle.Height);
                    if (!m_disabled)
                    {
                        // change color of text if mouse over text color is not null
                        if (m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
                        {
                            BitmapFont.ActiveFonts[Font].TextBox(rect, m_textOverColor, m_text);
                        }
                        else
                        {
                            BitmapFont.ActiveFonts[Font].TextBox(rect, m_textColor, m_text);
                        }
                    }
                    else
                    {
                        BitmapFont.ActiveFonts[Font].TextBox(rect, ColorDisabledStandard, m_text);
                    }
                }
                else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for Button [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
            }
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (m_disabled || !m_visible)
                return;

            if (m_owner != "")
                if (!GuiManager.GetControl(m_owner).IsVisible) return;

            if (m_visuals.ContainsKey(Enums.EControlState.Down))
                m_visualKey = m_visuals[Enums.EControlState.Down];

            if (ms.LeftButton == ButtonState.Pressed && !m_mouseLeftDown)
                m_mouseLeftDown = true;

            if (ms.RightButton == ButtonState.Pressed && !m_mouseRightDown)
                m_mouseRightDown = true;

            if (m_onMouseDown != "" && !m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
            {
                try
                {
                    Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), m_onMouseDown, true), this);
                    m_onMouseDownSent = true;
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                    Utils.LogOnce("Failed OnMouseDown event for Button [ " + m_name + " ] with Event [" + m_onMouseDown + "] of Sheet [ " + this.Sheet + " ]");
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            if (m_disabled)
                return;

            m_onMouseDownSent = false;

            if (ms.LeftButton == ButtonState.Released && m_mouseLeftDown)
                m_mouseLeftDown = false;

            if (ms.RightButton == ButtonState.Pressed && m_mouseRightDown)
                m_mouseRightDown = false;

            if (m_visuals.ContainsKey(Enums.EControlState.Normal))
                m_visualKey = m_visuals[Enums.EControlState.Normal];
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            // Enter key acts as a button press.
            if (HasFocus && ks.IsKeyDown(Keys.Enter))
            {
                OnMouseDown(GuiManager.MouseState);
                return true;
            }

            return base.OnKeyDown(ks);
        }
    }
}
