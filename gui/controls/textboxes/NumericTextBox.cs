using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    /// <summary>
    /// Textbox that accepts only numeric values.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        protected int m_maxValue = 100;
        protected int m_minValue = 1;

        public int CurrentValue { get; set; }

        public NumericTextBox(
            string name, string owner, Rectangle rectangle, string text, Color textColor, BitmapFont.TextAlignment textAlignment, bool visible, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, bool editable, int maxLength,
            bool passwordBox, bool blinkingCursor, Color cursorColor, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled,
            int xTextOffset, int yTextOffset, string onKeyboardEnter, Color selectionColor, List<Enums.EAnchorType> anchors, int tabOrder, int maxValue, int minValue) : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_text = text;
            m_cursorPosition = m_text.Length;
            m_textColor = textColor;
            m_textAlignment = textAlignment;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_borderAlpha = borderAlpha;
            m_textAlpha = textAlpha;
            m_editable = editable;
            m_maxLength = maxLength;
            m_passwordBox = passwordBox;
            m_blinkingCursor = blinkingCursor;
            m_cursorColor = cursorColor;
            m_xTextOffset = xTextOffset;
            m_yTextOffset = yTextOffset;
            m_onKeyboardEnter = onKeyboardEnter;
            m_selectionColor = selectionColor;
            m_anchors = anchors;

            if (m_visualKey.Key != "" && !m_visuals.ContainsKey(Enums.EControlState.Normal))
                m_visuals.Add(Enums.EControlState.Normal, m_visualKey);
            if (visualKeyOver.Key != "" && !m_visuals.ContainsKey(Enums.EControlState.Over))
                m_visuals.Add(Enums.EControlState.Over, visualKeyOver);
            if (visualKeyDown.Key != "" && !m_visuals.ContainsKey(Enums.EControlState.Down))
                m_visuals.Add(Enums.EControlState.Down, visualKeyDown);
            if (visualKeyDisabled.Key != "" && !m_visuals.ContainsKey(Enums.EControlState.Disabled))
                m_visuals.Add(Enums.EControlState.Disabled, visualKeyDisabled);

            m_previousBlink = new System.TimeSpan();
            m_selectionStart = 0;
            m_selectionLength = 0;
            m_tabOrder = tabOrder;

            m_maxValue = maxValue;
            m_minValue = minValue;
        }

        public override void Update(GameTime gameTime)
        {
            if(!int.TryParse(Text, out int CurrentValue))
            {
                Clear();
                AddText(m_maxValue.ToString());
                CurrentValue = m_maxValue;
            }

            if (!this.HasFocus)
            {
                if (CurrentValue > m_maxValue)
                {
                    Clear();
                    AddText(m_maxValue.ToString());
                    CurrentValue = m_maxValue;
                }
                else if (CurrentValue < m_minValue)
                {
                    Clear();
                    AddText(m_minValue.ToString());
                    CurrentValue = m_minValue;
                }
            }

            base.Update(gameTime);
        }

        public override void AddText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsDigit(text, i))
                    return;
            }

            base.AddText(text);
        }
    }
}
