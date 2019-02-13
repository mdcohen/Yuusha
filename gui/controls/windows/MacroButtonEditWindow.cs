using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class MacroButtonEditWindow : Window
    {
        public MacroButtonEditWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }
    }
}
