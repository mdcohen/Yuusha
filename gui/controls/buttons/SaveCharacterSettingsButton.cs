using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui.controls.buttons
{
    public class SaveCharacterSettingsButton : Button
    {
        public SaveCharacterSettingsButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command) : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command)
        {
        }
    }
}
