using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class ScrollbarControlBox : Control
    {
        private readonly Enums.EScrollbarControlType m_scrollbarControlType;
        private readonly string m_scrollbarOwner;

        ScrollbarControlBox(string name, string scrollbarOwner, Enums.EScrollbarControlType scrollbarControlType,
            Color tintColor)
        {
            m_name = name;
            m_scrollbarOwner = scrollbarOwner;
            m_scrollbarControlType = scrollbarControlType;
            m_tintColor = tintColor;
        }
    }
}
