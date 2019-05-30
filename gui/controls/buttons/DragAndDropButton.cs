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

        private SquareBorder m_border;
        public SquareBorder Border { get { return m_border; } set { m_border = value; } }

        public bool HasEnteredGridWindow { get; set; }

        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, "")
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

                gui.GuiManager.GenericSheet.AddControl(this);

                this.m_rectangle = new Rectangle(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2), this.Width, this.Height);

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

                gui.GuiManager.GenericSheet.RemoveControl(this);

                // Make a failed drag and drop sound?
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            if (Border == null)
                GuiManager.GenericSheet.CreateSquareBorder(this.Name + "SquareBorder", this.Name, 1, new VisualKey("WhiteSpace"), false, Color.OldLace, 255);

            this.m_borderAlpha = 255;

            if (Border != null)
                Border.IsVisible = true;

            TextCue.AddMouseCursorTextCue(this.Text, Color.LightCyan, this.Font);

        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
                Border.IsVisible = false;

            this.m_borderAlpha = 0;

            TextCue.ClearCursorTextCue();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(m_dragging)
            {
                this.Position = new Point(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2));
                this.m_rectangle = new Rectangle(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2), this.Width, this.Height);
            }

            if (Border != null) Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }
    }
}
