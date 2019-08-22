using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class MacroButton : Button
    {
        public MacroButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, "", Client.ClientSettings.DefaultOnClickSound)
        {
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (m_disabled)
                return;

            if (!m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
            {
                try
                {
                    Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), m_onMouseDown, true), this);
                    m_onMouseDownSent = true;
                }
                catch (Exception e)
                {
                    Utils.LogException(e);
                    Utils.LogOnce("Failed OnMouseDown event for Button [ " + m_name + " ] of Sheet [ " + this.Sheet + " ]");
                }
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                Utils.SetClipboardText(this.Text);
                TextCue.AddClientInfoTextCue("Text Copied to Keyboard", Color.LimeGreen, Color.Black, 1000);
            }
        }
    }
}
