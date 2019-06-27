using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class TabControlButton : Button
    {
        public string TabControlledWindow;
        public TabControl TabControl;
        private Rectangle m_typicalRectangle;
        private VisualKey m_typicalVisualKey;
        private Color m_typicalTintColor;


        public TabControlButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor,
            Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection,
            int shadowDistance, string tabControlledWindow) : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey,
                tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, "TabControl", textAlignment,
                xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, "", "")
        {
            TabControlledWindow = tabControlledWindow;
            m_onMouseDown = "TabControl";

            m_typicalRectangle = rectangle;
            m_typicalVisualKey = visualKey;
            m_typicalTintColor = tintColor;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Window w = GuiManager.GetControl(TabControlledWindow) as Window;

            if (w != null)
            {
                if (w.IsVisible)
                {
                    m_rectangle.Height = m_typicalRectangle.Height + 1;
                    m_visualKey = new VisualKey(w.VisualKey);
                    m_tintColor = w.TintColor;
                }
                else
                {
                    m_rectangle.Height = m_typicalRectangle.Height;
                    m_visualKey = m_typicalVisualKey;
                    m_tintColor = m_typicalTintColor;
                }
            }
        }

        //public void TabSelected()
        //{
        //    Window w = GuiManager.GetControl(TabControlledWindow) as Window;

        //    if (w != null)
        //    {
        //        m_rectangle = new Rectangle(m_typicalRectangle.X, m_typicalRectangle.Y, m_typicalRectangle.Width, m_typicalRectangle.Height + 1);
        //        m_visualKey = new VisualKey(w.VisualKey);
        //        m_tintColor = w.TintColor;
        //    }
        //}

        //public void TabDeselected()
        //{
        //    m_rectangle = m_typicalRectangle;
        //    m_visualKey = m_typicalVisualKey;
        //    m_tintColor = m_typicalTintColor;
        //}
    }
}
