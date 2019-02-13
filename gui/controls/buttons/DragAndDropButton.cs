using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DragAndDropButton : Button
    {
        protected Rectangle m_originalRectangle;
        protected string m_originalOwner;
        protected bool m_dragging;
        protected int m_mouseDownX;
        protected int m_mouseDownY;

        public bool HasEnteredGridWindow { get; set; }

        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command) : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command)
        {
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (ms.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return base.MouseHandler(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                m_originalRectangle = this.m_rectangle;
                m_originalOwner = this.Owner;

                // Attach to cursor.
                m_dragging = true;
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            base.OnMouseRelease(ms);

            if(!HasEnteredGridWindow)
            {
                this.m_rectangle = m_originalRectangle;
                this.m_owner = m_originalOwner;
                m_dragging = false;

                // Make a failed drag and drop sound?
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(m_dragging)
            {
                this.m_rectangle = new Rectangle(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2), this.Width, this.Height);
            }
        }
    }
}
