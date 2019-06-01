using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DragAndDropButton : Button
    {
        protected Point m_originalPosition;
        protected bool m_draggingToDrop;
        protected int m_mouseDownX;
        protected int m_mouseDownY;
        public Item RepresentedItem
        { get; set; }

        private SquareBorder m_border;
        public SquareBorder Border { get { return m_border; } set { m_border = value; } }

        public bool HasEnteredGridWindow { get; set; }

        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, text)
        {
            HasEnteredGridWindow = false;
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (!m_draggingToDrop)
                {
                    MouseCursor cursor = GuiManager.Cursors[gui.GuiManager.GenericSheet.Cursor];
                    if (cursor.DraggedButton != null)
                        return;
                    else cursor.DraggedButton = this;

                    m_originalPosition = new Point(Position.X, Position.Y);

                    // Attach to current cursor for draw and update.
                    GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedButton = this;

                    Position = new Point(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2));

                    // Attach to cursor.
                    m_draggingToDrop = true;

                    TextCue.AddClientInfoTextCue(this.Name + " dragging...");
                }
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                if (!m_onMouseDownSent && RepresentedItem != null)
                {
                    // determine where this drag and drop button is
                    if (Owner.StartsWith(GridBox.GridBoxFunction.Sack.ToString()))
                    {
                        int itemCount = 0;
                        foreach (Item item in Character.CurrentCharacter.Sack)
                        {
                            if (item.name == RepresentedItem.name)
                                itemCount++;
                            if (item == RepresentedItem)
                            {
                                IO.Send("look at " + itemCount + " " + RepresentedItem.name + " in sack");
                                m_onMouseDownSent = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            base.OnMouseRelease(ms);

            if(!HasEnteredGridWindow && m_draggingToDrop)
            {
                Position = new Point(m_originalPosition.X, m_originalPosition.Y);
                GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedButton = null;
                m_draggingToDrop = false;
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            if (GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedButton != null) return;

            base.OnMouseOver(ms);

            if (Border == null)
            {
                Border border = new SquareBorder(this.Name + "SquareBorder", this.Name, 1, new VisualKey("WhiteSpace"), false, Color.OldLace, 255);
                GuiManager.GenericSheet.AddControl(border);
            }

            this.m_borderAlpha = 255;

            if (Border != null)
                Border.IsVisible = true;
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
                Border.IsVisible = false;

            this.m_borderAlpha = 0;

            TextCue.ClearMouseCursorTextCue();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(m_draggingToDrop)
                this.Position = new Point(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2));

            if (Border != null)
                Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null && Border.IsVisible) Border.Draw(gameTime);
        }
    }
}
