using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DragAndDropButton : Button
    {
        protected Point m_originalPosition;
        //protected int m_originalZDepth;
        protected bool m_draggingToDrop;
        protected int m_mouseDownX;
        protected int m_mouseDownY;
        protected int m_touchDownPointX;
        protected int m_touchDownPointY;

        public Item RepresentedItem
        { get; set; }
        public Border Border
        { get; set; }

        public bool HasEnteredGridBoxWindow { get; set; }
        public bool AcceptingDroppedButtons { get; set; }

        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, text)
        {
            HasEnteredGridBoxWindow = false;
            AcceptingDroppedButtons = true; // anything created dynamically in code should be wary of this property
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (!m_draggingToDrop)
                {
                    MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];
                    if (cursor.DraggedButton != null)
                        return;
                    else cursor.DraggedButton = this;

                    m_originalPosition = new Point(Position.X, Position.Y);
                    //m_originalZDepth = ZDepth;

                    // puts the control dead center under mouse cursor
                    //Position = new Point(ms.X - (this.m_rectangle.Width / 2), ms.Y - (this.m_rectangle.Width / 2));
                    Position = new Point(cursor.Position.X - 10, cursor.Position.Y - 10);
                    //Position = new Point(cursor.Position.X - (this.m_rectangle.Width / 2), cursor.Position.Y - (this.m_rectangle.Width / 2));

                    //TextCue.AddClientInfoTextCue("Mouse State:" + ms.)
                    //TextCue.AddClientInfoTextCue(Name + ": " + Position.ToString());

                    // Attach to cursor.
                    m_draggingToDrop = true;
                }
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                if (!m_onMouseDownSent && RepresentedItem != null)
                {
                    // some of these will have a drop down menu selection

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
                    else if (Owner.StartsWith(GridBox.GridBoxFunction.Belt.ToString()))
                    {
                        int itemCount = 0;
                        foreach (Item item in Character.CurrentCharacter.Sack)
                        {
                            if (item.name == RepresentedItem.name)
                                itemCount++;
                            if (item == RepresentedItem)
                            {
                                IO.Send("look at " + itemCount + " " + RepresentedItem.name + " on belt");
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

            if(!HasEnteredGridBoxWindow && m_draggingToDrop)
                StopDragging();
            else if (HasEnteredGridBoxWindow && m_draggingToDrop)
            {
                // Right hand or left hand items
                if (this.Name.StartsWith("RH") || this.Name.StartsWith("LH"))
                {
                    string rightOrLeft = this.Name.StartsWith("RH") ? "right" : "left";
                    if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                    {
                        IO.Send("put " + rightOrLeft + " in sack");
                        IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                    }
                    else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                    {
                        IO.Send("belt " + rightOrLeft);
                        IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                    }
                    else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                    {
                        IO.Send("put " + rightOrLeft + " in pouch");
                        IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                    }
                    else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                    {
                        IO.Send("swap");
                    }
                }
                else if(this.Name.StartsWith("Belt"))
                {
                    if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                    {
                        IO.Send("wield " + this.RepresentedItem.name);
                        IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                    }
                }
                else if(this.Name.StartsWith("Sack"))
                {
                    if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                    {
                        GridBox gb = GuiManager.GenericSheet["SackGridBox"] as GridBox;
                        //foreach(Item item in gb.)
                        //IO.Send("take " + this.RepresentedItem.name );
                    }
                }

                StopDragging();
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];

            if (cursor.DraggedButton == null || cursor.DraggedButton == this)
            {
                if (Border == null)
                {
                    Border border = new SquareBorder(this.Name + "SquareBorder", this.Name, 1, new VisualKey("WhiteSpace"), false, Color.OldLace, 255);
                    if (this.Sheet == "Generic")
                        GuiManager.GenericSheet.AddControl(border);
                    else GuiManager.CurrentSheet.AddControl(border);
                }

                if (Border != null)
                    Border.IsVisible = true;
            }

            if (AcceptingDroppedButtons && cursor != null && cursor.DraggedButton != null)
            {
                //TextCue.AddClientInfoTextCue(this.Name + " is accepting a dropped button...");
                GuiManager.MouseOverDropAcceptingControl = this;
                cursor.DraggedButton.HasEnteredGridBoxWindow = true;
            }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
                Border.IsVisible = false;

            TextCue.ClearMouseCursorTextCue();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GuiManager.MouseState.LeftButton != ButtonState.Pressed)
                OnMouseRelease(GuiManager.MouseState);

            if (m_draggingToDrop)
            {
                Position = new Point(GuiManager.MouseState.X - 10, GuiManager.MouseState.Y - 10);
                //Position = new Point(GuiManager.MouseState.X - (this.m_rectangle.Width / 2), GuiManager.MouseState.Y - (this.m_rectangle.Width / 2));
            }

            if (Border != null)
                Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null && Border.IsVisible) Border.Draw(gameTime);
        }

        public void StopDragging()
        {
            Position = new Point(m_originalPosition.X, m_originalPosition.Y);
            //ZDepth = m_originalZDepth;
            GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedButton = null;
            m_draggingToDrop = false;
        }
    }
}
