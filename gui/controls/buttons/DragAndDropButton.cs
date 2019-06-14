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
        public List<GridBoxWindow.GridBoxPurpose> GridBoxUpdateRequests
        { get; set; }

        public Item RepresentedItem
        { get; set; }
        public Border Border
        { get; set; }

        public bool HasEnteredGridBoxWindow { get; set; }
        public bool AcceptingDroppedButtons { get; set; }
        public bool DrawBlackBackground { get; set; } // used because of item textures irregularities... (need an artist!) 6/8/2019
        public DropDownMenu DropDownMenu
        { get; set; }


        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, "", popUpText)
        {
            HasEnteredGridBoxWindow = false;
            AcceptingDroppedButtons = true; // anything created dynamically in code should be wary of this property
            DrawBlackBackground = false;
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed && (DropDownMenu == null || !DropDownMenu.IsVisible))
            {
                if (!m_draggingToDrop)
                {
                    MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];
                    if (cursor.DraggedButton != null)
                        return;
                    else cursor.DraggedButton = this;

                    m_originalPosition = new Point(Position.X, Position.Y);

                    // puts the control dead center under mouse cursor
                    Position = new Point(cursor.Position.X - 10, cursor.Position.Y - 10);
                    //Position = new Point(cursor.Position.X - (this.m_rectangle.Width / 2), cursor.Position.Y - (this.m_rectangle.Width / 2));

                    // Attach to cursor.
                    m_draggingToDrop = true;
                }
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                // create drop down menu
                if(RepresentedItem != null && DropDownMenu == null)
                {
                    Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 200, 100); // default height for 5 drop down menu items

                    // readjust Y if out of client width bounds
                    if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                    GuiManager.GenericSheet.CreateDropDownMenu(Name + "DropDownMenu", this, "", dropDownRectangle, true,
                        Font, new VisualKey("WhiteSpace"), Client.UserSettings.ColorDropDownMenu, VisualAlpha, true, Map.Direction.Northwest, 5);

                    DropDownMenu.HasFocus = true;
                    int height = 0;

                    // determine drop down items here
                    List<Tuple<string, string, GridBoxWindow.GridBoxPurpose>> dropDownMenuItemTextList = new List<Tuple<string,string, GridBoxWindow.GridBoxPurpose>>(); // text for drop down menu item, command sent when clicked
                    GridBoxWindow gridBox = GuiManager.GetControl(Owner) as GridBoxWindow;

                    if(RepresentedItem != null)
                    {
                        GridBoxUpdateRequests = new List<GridBoxWindow.GridBoxPurpose>();

                        switch(gridBox.GridBoxPurposeType)
                        {
                            case GridBoxWindow.GridBoxPurpose.Altar:
                            case GridBoxWindow.GridBoxPurpose.Counter:
                            case GridBoxWindow.GridBoxPurpose.Ground:
                                // cannot manipulate anything not on the ground by our feet so there is no drop down displayed
                                if (gridBox.GridBoxPurposeType != GridBoxWindow.GridBoxPurpose.Ground || (gridBox.GridBoxPurposeType == GridBoxWindow.GridBoxPurpose.Ground && GameHUD.ExaminedCell == GameHUD.GetCurrentCharacterCell()))
                                {
                                    string location = gridBox.GridBoxPurposeType.ToString().ToLower();
                                    //dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.name, "look at " + RepresentedItem.worldItemID + " on altar"));
                                    dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + GetNItemName(this) + " on " + location, GridBoxWindow.GridBoxPurpose.None));
                                    //dropDownMenuItemTextList.Add(Tuple.Create("to sack", "scoop " + RepresentedItem.worldItemID + " from altar"));
                                    dropDownMenuItemTextList.Add(Tuple.Create("sack", "take " + GetNItemName(this) + " from " + location + ";put " + RepresentedItem.Name + " in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                    //dropDownMenuItemTextList.Add(Tuple.Create("to pouch", "pscoop " + RepresentedItem.worldItemID + " from altar"));
                                    dropDownMenuItemTextList.Add(Tuple.Create("pouch", "take " + GetNItemName(this) + " from " + location + ";put " + RepresentedItem.Name + " in sack", GridBoxWindow.GridBoxPurpose.Pouch));
                                    // if more than 1 scoop/pscoop all
                                    if (GridBoxWindow.GetItemsCount(RepresentedItem.Name, Owner) > 1)
                                    {
                                        if (location == "ground") location = "";
                                        else location = " from " + location;

                                        dropDownMenuItemTextList.Add(Tuple.Create("scoop all", "scoop all " + RepresentedItem.Name + "s" + location, GridBoxWindow.GridBoxPurpose.Sack));
                                        if (!RepresentedItem.Name.StartsWith("coin"))
                                            dropDownMenuItemTextList.Add(Tuple.Create("pouch all", "pscoop all " + RepresentedItem.Name + "s" + location, GridBoxWindow.GridBoxPurpose.Pouch));
                                    }
                                    // search all corpses if even one exists
                                    if (RepresentedItem.Name.ToLower() == "corpse")
                                        dropDownMenuItemTextList.Add(Tuple.Create("search", "search all " + RepresentedItem.Name + "s" + location, GridBoxWindow.GridBoxPurpose.None));
                                }
                                break;
                            case GridBoxWindow.GridBoxPurpose.Belt:
                                //dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.name, "look at " + RepresentedItem.worldItemID + " on belt"));
                                dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " on belt", GridBoxWindow.GridBoxPurpose.None));
                                if (Character.CurrentCharacter.RightHand == null || Character.CurrentCharacter.LeftHand == null)
                                {
                                    //dropDownMenuItemTextList.Add(Tuple.Create("wield", "wield " + RepresentedItem.worldItemID));
                                    dropDownMenuItemTextList.Add(Tuple.Create("wield", "wield " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.None));
                                    dropDownMenuItemTextList.Add(Tuple.Create("wield and drop", "wield " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                    if (GameHUD.CurrentTarget != null) // NOTE: some items can be thrown from the belt without wielding them
                                        dropDownMenuItemTextList.Add(Tuple.Create("throw at " + GameHUD.CurrentTarget.Name, "wield " + RepresentedItem.Name + ";throw it at " + GameHUD.CurrentTarget.ID, GridBoxWindow.GridBoxPurpose.None));
                                }
                                break;
                            case GridBoxWindow.GridBoxPurpose.Locker:
                                //dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.name, "look at " + RepresentedItem.worldItemID + " in locker"));
                                dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in locker", GridBoxWindow.GridBoxPurpose.None));
                                //dropDownMenuItemTextList.Add(Tuple.Create("take " + RepresentedItem.name, "take " + RepresentedItem.worldItemID + " from locker"));
                                dropDownMenuItemTextList.Add(Tuple.Create("take " + RepresentedItem.Name, "take " + GetNItemName(this) + " from locker", GridBoxWindow.GridBoxPurpose.None));
                                break;
                            case GridBoxWindow.GridBoxPurpose.Pouch:
                                dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in pouch", GridBoxWindow.GridBoxPurpose.None));
                                dropDownMenuItemTextList.Add(Tuple.Create("drop", "take " + GetNItemName(this) + " from pouch;drop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                dropDownMenuItemTextList.Add(Tuple.Create("dump all", "pdump all " + RepresentedItem.Name + "s", GridBoxWindow.GridBoxPurpose.Ground));
                                break;
                            case GridBoxWindow.GridBoxPurpose.Rings:
                                dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in rings", GridBoxWindow.GridBoxPurpose.None));
                                break;
                            case GridBoxWindow.GridBoxPurpose.Sack:
                                dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in sack", GridBoxWindow.GridBoxPurpose.None));
                                dropDownMenuItemTextList.Add(Tuple.Create("drop", "take " + GetNItemName(this) + " from sack;drop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                dropDownMenuItemTextList.Add(Tuple.Create("dump all", "dump all " + RepresentedItem.Name + "s", GridBoxWindow.GridBoxPurpose.Ground));
                                // if next to counter, dump all on counter/altar
                                break;
                        }

                        foreach(Tuple<string, string, GridBoxWindow.GridBoxPurpose> tuple in dropDownMenuItemTextList)
                        {
                            height += 20;
                            DropDownMenu.AddDropDownMenuItem(tuple.Item1, Name, new VisualKey("WhiteSpace"), "Send_Command", tuple.Item2, false);
                            GridBoxUpdateRequests.Add(tuple.Item3);
                        }
                    }

                    DropDownMenu.Height = height;
                }
            }
        }

        protected override void OnDoubleLeftClick()
        {
            if (!Client.HasFocus) return;

            // do something

            m_leftClickCount = 0;
        }

        protected override void OnDoubleRightClick()
        {
            if (!Client.HasFocus) return;

            // do something

            m_rightClickCount = 0;
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            base.OnMouseRelease(ms);

            if(!HasEnteredGridBoxWindow && m_draggingToDrop)
                StopDragging();
            else if (HasEnteredGridBoxWindow && m_draggingToDrop)
            {
                GameHUD.DragAndDropLogic(this);

                StopDragging();
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];

            if (cursor.DraggedButton == null || cursor.DraggedButton == this || AcceptingDroppedButtons)
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

        public override bool MouseHandler(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            if (DropDownMenu != null)
                DropDownMenu.MouseHandler(ms);

            return base.MouseHandler(ms);
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

            if (!IsVisible || IsDisabled || RepresentedItem == null)
            {
                if (DropDownMenu != null)
                    DropDownMenu = null;
            }

            if (DropDownMenu != null)
                DropDownMenu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(DrawBlackBackground)
            {
                if (!GuiManager.Visuals.ContainsKey("WhiteSpace"))
                {
                    Utils.LogOnce("Failed to find visual key [ WhiteSpace ] for Control [ " + m_name + " ]");
                    return;
                }

                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];

                Color color = Color.Black;

                if (m_hasTintOverColor && ControlState == Enums.EControlState.Over)
                    color = new Color(m_tintOverColor.R, m_tintOverColor.G, m_tintOverColor.B, VisualAlpha);

                if (m_disabled)
                    color = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, VisualAlpha);

                if (!m_visualTiled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, color);
                }
                else // Tiled visual (borders, window titles)
                {
                    // This code needs some work. 2/18/2017. Have to move on.
                    int desiredWidth = (int)(this.Width / vi.Width);
                    int desiredHeight = this.Height;

                    // What uses tiled visuals?
                    int xAmount = (int)(this.Width / desiredWidth);
                    int yAmount = (int)(this.Height / desiredHeight);

                    int countWidth = 0;
                    int countHeight = 0;

                    // this goes columns first, then rows.
                    // may have to modify for things such as vertical borders/titles

                    for (int x = 0; countWidth <= this.Width; x++, countWidth += desiredWidth)
                    {
                        for (int y = 0; countHeight <= this.Height; y++, countHeight += desiredHeight)
                        {
                            Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(x * desiredWidth, y * desiredHeight, desiredWidth, desiredHeight), vi.Rectangle, color);
                        }
                    }
                }
            }

            base.Draw(gameTime);

            if (Border != null && Border.IsVisible) Border.Draw(gameTime);

            if (DropDownMenu != null)
                DropDownMenu.Draw(gameTime);
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            DropDownMenu = null;

            base.OnClientResize(prev, now, ownerOverride);
        }

        public void StopDragging()
        {
            Position = new Point(m_originalPosition.X, m_originalPosition.Y);
            //ZDepth = m_originalZDepth;
            GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedButton = null;
            m_draggingToDrop = false;
        }

        public string GetNItemName(DragAndDropButton draggedButton)
        {
            if (draggedButton.RepresentedItem.Name.ToLower().Contains("coin"))
                return "coins";

            return GetNumberItem(draggedButton).ToString() + " " + draggedButton.RepresentedItem.Name;
        }

        public int GetNumberItem(DragAndDropButton draggedButton)
        {
            GridBoxWindow box = GuiManager.GetControl(draggedButton.Owner) as GridBoxWindow;
            List<Control> buttons = new List<Control>(box.Controls);

            buttons.RemoveAll(button => !(button is DragAndDropButton));
            buttons.RemoveAll(button => (button as DragAndDropButton).RepresentedItem.Name != draggedButton.RepresentedItem.Name);

            int count = 1;

            if (!Client.ClientSettings.GroupSimiliarItemsInGridBoxes)
            {
                foreach (DragAndDropButton dadButton in buttons)
                {
                    if (dadButton == draggedButton)
                        return count;
                    count++;
                }
            }
            else
            {
                List<Item> itemsList = new List<Item>(Character.GetItemsList(Character.CurrentCharacter, box.GridBoxPurposeType));
                itemsList.RemoveAll(k => k.Name != draggedButton.RepresentedItem.Name);

                foreach (Item item2 in itemsList)
                {
                    if (item2.VisualKey == draggedButton.RepresentedItem.VisualKey)
                        count++;
                    else return count;
                }
            }

            return 1;
        }
    }
}
