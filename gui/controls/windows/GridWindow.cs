using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class GridWindow : Window
    {
        private int m_numRows;
        private int m_numColumns;
        private int m_rowWidth;
        private int m_columnWidth;
        private int m_paddingBetweenControls;

        public GridWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }
    }
}
