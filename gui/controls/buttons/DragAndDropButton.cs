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
        public bool HasOriginalBorderColor
        { get; set; }
        public Color OriginalBorderColor
        { get; set; }

        public List<GridBoxWindow.GridBoxPurpose> GridBoxUpdateRequests
        { get; set; }

        public Item RepresentedItem
        { get; set; }

        public bool HasEnteredGridBoxWindow { get; set; }
        public bool AcceptingDroppedButtons { get; set; }
        public DropDownMenu DropDownMenu
        { get; set; }


        /// <summary>
        /// Dragged from a GridWindow to a GridWindow or a HotButton a GridWindow is attached to.
        /// </summary>
        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText, bool isLocked)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, "", popUpText)
        {
            HasEnteredGridBoxWindow = false;
            AcceptingDroppedButtons = false; // anything created dynamically in code should be wary of this property
            IsLocked = isLocked; // when a drag and drop button is locked it cannot be dragged -- however it can still accept dropped buttons...
        }

        public DragAndDropButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText, bool isLocked, bool acceptingDroppedButtons)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, "", popUpText)
        {
            HasEnteredGridBoxWindow = false;
            AcceptingDroppedButtons = acceptingDroppedButtons;
            IsLocked = isLocked; // when a drag and drop button is locked it cannot be dragged -- however it can still accept dropped buttons...
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed && (DropDownMenu == null || !DropDownMenu.IsVisible))
            {
                if (!m_draggingToDrop && !m_locked)
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
            else if (ms.RightButton == ButtonState.Pressed)
            {
                try
                {
                    // create drop down menu
                    if (RepresentedItem != null && DropDownMenu == null)
                    {
                        Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 200, 100); // default height for 5 drop down menu items

                        // readjust Y if out of client width bounds
                        if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                            dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                        //must come up with a solution for calling these
                        if (Sheet != "Generic")
                        {
                            GuiManager.CurrentSheet.CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                            Font, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, VisualAlpha, true, Map.Direction.Northwest, 5);

                            //GuiManager.CurrentSheet.CreateSquareBorder(DropDownMenu.Name + "Border", DropDownMenu.Name, Client.ClientSettings.DropDownMenuBorderWidth, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);
                        }
                        else
                        {
                            GuiManager.GenericSheet.CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                            Font, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, VisualAlpha, true, Map.Direction.Northwest, 5);

                            //GuiManager.GenericSheet.CreateSquareBorder(DropDownMenu.Name + "Border", DropDownMenu.Name, Client.ClientSettings.DropDownMenuBorderWidth, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);
                        }

                        DropDownMenu.HasFocus = true;
                        int height = 0;

                        // determine drop down items here
                        List<Tuple<string, string, GridBoxWindow.GridBoxPurpose>> dropDownMenuItemTextList = new List<Tuple<string, string, GridBoxWindow.GridBoxPurpose>>(); // text for drop down menu item, command sent when clicked

                        if (RepresentedItem != null)
                        {
                            GridBoxUpdateRequests = new List<GridBoxWindow.GridBoxPurpose>();

                            if (GuiManager.GetControl(Owner) is GridBoxWindow gridBox)
                            {
                                GridBoxUpdateRequests.Add(gridBox.GridBoxPurposeType);

                                switch (gridBox.GridBoxPurposeType)
                                {
                                    case GridBoxWindow.GridBoxPurpose.Altar:
                                    case GridBoxWindow.GridBoxPurpose.Counter:
                                    case GridBoxWindow.GridBoxPurpose.Ground:
                                        string location = gridBox.GridBoxPurposeType.ToString().ToLower();
                                        dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + GetNItemName(this) + " on " + location, GridBoxWindow.GridBoxPurpose.None));
                                        int itemsCount = gridBox.GetItemsCount(RepresentedItem.Name);
                                        if (gridBox.GridBoxPurposeType == GridBoxWindow.GridBoxPurpose.Ground && Character.CurrentCharacter.HasFreeHand)
                                            dropDownMenuItemTextList.Add(Tuple.Create("take", "take " + GetNItemName(this), GridBoxWindow.GridBoxPurpose.Ground));
                                        //else if (itemsCount == 1)
                                        //{
                                        //    dropDownMenuItemTextList.Add(Tuple.Create("sack", "scoop " + RepresentedItem.Name + location == "ground" ? "" : " from " + location, GridBoxWindow.GridBoxPurpose.Sack));
                                        //    dropDownMenuItemTextList.Add(Tuple.Create("pouch", "pscoop " + RepresentedItem.Name + location == "ground" ? "" : " from " + location, GridBoxWindow.GridBoxPurpose.Pouch));
                                        //}
                                        if (RepresentedItem.Name != "corpse" && !RepresentedItem.Name.StartsWith("coin") && Character.CurrentCharacter.HasFreeHand)
                                            dropDownMenuItemTextList.Add(Tuple.Create("sack", "take " + GetNItemName(this) + " from " + location + ";put " + RepresentedItem.Name + " in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                        else if (RepresentedItem.Name.StartsWith("coin"))
                                        {
                                            if(gridBox.GridBoxPurposeType != GridBoxWindow.GridBoxPurpose.Ground)
                                                dropDownMenuItemTextList.Add(Tuple.Create("sack", "scoop coins from " + location, GridBoxWindow.GridBoxPurpose.Sack));
                                            else dropDownMenuItemTextList.Add(Tuple.Create("sack", "scoop coins", GridBoxWindow.GridBoxPurpose.Sack));
                                        }
                                        else if (!RepresentedItem.Name.StartsWith("coin") && RepresentedItem.Name != "corpse" && itemsCount == 1)
                                            dropDownMenuItemTextList.Add(Tuple.Create("sack", "scoop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Sack));
                                        if (!RepresentedItem.Name.StartsWith("coin") && RepresentedItem.Name != "corpse" && Character.CurrentCharacter.HasFreeHand)
                                        {
                                            dropDownMenuItemTextList.Add(Tuple.Create("pouch", "take " + GetNItemName(this) + " from " + location + ";put " + RepresentedItem.Name + " in pouch", GridBoxWindow.GridBoxPurpose.Pouch));
                                            dropDownMenuItemTextList.Add(Tuple.Create("belt", "take " + GetNItemName(this) + " from " + location + ";belt " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Belt));
                                        }
                                        else if(!RepresentedItem.Name.StartsWith("coin") && RepresentedItem.Name != "corpse" && itemsCount == 1)
                                            dropDownMenuItemTextList.Add(Tuple.Create("pouch", "pscoop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Pouch));
                                        // if more than 1 scoop/pscoop all
                                        if (RepresentedItem.Name != "corpse" && itemsCount > 1)
                                        {
                                            if (location == "ground") location = "";
                                            else location = " from " + location;

                                            dropDownMenuItemTextList.Add(Tuple.Create("scoop all", "scoop all " + RepresentedItem.Name + "s" + location, GridBoxWindow.GridBoxPurpose.Sack));
                                            if (!RepresentedItem.Name.StartsWith("coin"))
                                                dropDownMenuItemTextList.Add(Tuple.Create("pouch all", "pscoop all " + RepresentedItem.Name + "s" + location, GridBoxWindow.GridBoxPurpose.Pouch));
                                        }
                                        // search all corpses if even one exists
                                        if (gridBox.GridBoxPurposeType == GridBoxWindow.GridBoxPurpose.Ground && RepresentedItem.Name.ToLower() == "corpse")
                                            dropDownMenuItemTextList.Add(Tuple.Create("search", "search all", GridBoxWindow.GridBoxPurpose.None));
                                        break;
                                    case GridBoxWindow.GridBoxPurpose.Belt:
                                        dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " on belt", GridBoxWindow.GridBoxPurpose.None));
                                        if (Character.CurrentCharacter.HasFreeHand)
                                        {
                                            dropDownMenuItemTextList.Add(Tuple.Create("wield", "wield " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.None));
                                            dropDownMenuItemTextList.Add(Tuple.Create("wield and drop", "wield " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                            if (GameHUD.CurrentTarget != null) // NOTE: some items can be thrown from the belt without wielding them
                                                dropDownMenuItemTextList.Add(Tuple.Create("throw at " + GameHUD.CurrentTarget.Name, "wield " + RepresentedItem.Name + ";throw it at " + GameHUD.CurrentTarget.ID, GridBoxWindow.GridBoxPurpose.None));
                                            if (Character.CurrentCharacter.IsNextToCounter(out string uLocationName))
                                                dropDownMenuItemTextList.Add(Tuple.Create("put on " + uLocationName, "wield " + RepresentedItem.Name + ";put it on " + uLocationName, GridBoxWindow.GridBoxPurpose.Ground));
                                        }
                                        break;
                                    case GridBoxWindow.GridBoxPurpose.Locker:
                                        dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in locker", GridBoxWindow.GridBoxPurpose.None));
                                        if (Character.CurrentCharacter.HasFreeHand)
                                        {
                                            dropDownMenuItemTextList.Add(Tuple.Create("take", "take " + GetNItemName(this) + " from locker", GridBoxWindow.GridBoxPurpose.None));
                                            dropDownMenuItemTextList.Add(Tuple.Create("sack", "take " + GetNItemName(this) + " from locker;put " + RepresentedItem.Name + " in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                            dropDownMenuItemTextList.Add(Tuple.Create("pouch", "take " + GetNItemName(this) + " from locker;put " + RepresentedItem.Name + " in pouch", GridBoxWindow.GridBoxPurpose.Pouch));
                                            dropDownMenuItemTextList.Add(Tuple.Create("belt", "take " + GetNItemName(this) + " from locker;belt " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Belt));
                                        }
                                        break;
                                    case GridBoxWindow.GridBoxPurpose.Pouch:
                                        dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in pouch", GridBoxWindow.GridBoxPurpose.None));
                                        if (Character.CurrentCharacter.HasFreeHand)
                                        {
                                            dropDownMenuItemTextList.Add(Tuple.Create("take", "take " + GetNItemName(this) + " from pouch", GridBoxWindow.GridBoxPurpose.None));
                                            dropDownMenuItemTextList.Add(Tuple.Create("drop", "take " + GetNItemName(this) + " from pouch;drop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                        }
                                        itemsCount = gridBox.GetItemsCount(RepresentedItem.Name);
                                        if (itemsCount > 1)
                                            dropDownMenuItemTextList.Add(Tuple.Create("dump all on ground", "pdump all " + RepresentedItem.Name + "s", GridBoxWindow.GridBoxPurpose.Ground));
                                        else if(itemsCount == 1 && !Character.CurrentCharacter.HasFreeHand)
                                            dropDownMenuItemTextList.Add(Tuple.Create("dump on ground", "pdump all " + RepresentedItem.Name + "s", GridBoxWindow.GridBoxPurpose.Ground));
                                        string locationName = "counter";
                                        if (Character.CurrentCharacter.IsNextToCounter(out locationName))
                                        {
                                            dropDownMenuItemTextList.Add(Tuple.Create("put on " + locationName, "take " + GetNItemName(this) + " from pouch;put " + RepresentedItem.Name + " on " + locationName, GridBoxWindow.GridBoxPurpose.Counter));
                                            if (itemsCount > 1)
                                                dropDownMenuItemTextList.Add(Tuple.Create("dump all on counter", "pdump all " + RepresentedItem.Name + "s on counter", GridBoxWindow.GridBoxPurpose.Counter));
                                            else if(itemsCount == 1 && !Character.CurrentCharacter.HasFreeHand)
                                                dropDownMenuItemTextList.Add(Tuple.Create("dump on counter", "pdump " + RepresentedItem.Name + " on counter", GridBoxWindow.GridBoxPurpose.Counter));
                                        }
                                        break;
                                    case GridBoxWindow.GridBoxPurpose.Rings:
                                        //dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in rings", GridBoxWindow.GridBoxPurpose.None));
                                        break;
                                    case GridBoxWindow.GridBoxPurpose.Sack:
                                        dropDownMenuItemTextList.Add(Tuple.Create("look at " + RepresentedItem.Name, "look at " + GetNItemName(this) + " in sack", GridBoxWindow.GridBoxPurpose.None));
                                        if(Character.CurrentCharacter.HasFreeHand)
                                            dropDownMenuItemTextList.Add(Tuple.Create("drop", "take " + GetNItemName(this) + " from sack;drop " + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.Ground));
                                        itemsCount = gridBox.GetItemsCount(RepresentedItem.Name);
                                        if (RepresentedItem.Name.StartsWith("coin") || itemsCount > 1)
                                            dropDownMenuItemTextList.Add(Tuple.Create("dump all", "dump all " + RepresentedItem.Name + "s", GridBoxWindow.GridBoxPurpose.Ground));
                                        locationName = "counter";
                                        if (Character.CurrentCharacter.IsNextToCounter(out locationName))
                                        {
                                            if(Character.CurrentCharacter.HasFreeHand)
                                                dropDownMenuItemTextList.Add(Tuple.Create("put on " + locationName, "take " + GetNItemName(this) + " from sack;put " + RepresentedItem.Name + " on " + locationName, GridBoxWindow.GridBoxPurpose.Counter));
                                            if (itemsCount > 1)
                                                dropDownMenuItemTextList.Add(Tuple.Create("dump all on " + locationName, "dump all " + RepresentedItem.Name + "s on " + locationName, GridBoxWindow.GridBoxPurpose.Counter));
                                            else if(itemsCount == 1)
                                                dropDownMenuItemTextList.Add(Tuple.Create("dump on " + locationName, "dump " + RepresentedItem.Name + " on " + locationName, GridBoxWindow.GridBoxPurpose.Counter));
                                        }
                                        break;
                                }
                            }
                            else if (GuiManager.GetControl(Owner) is Window window)
                            {
                                switch (Owner.Replace("Window", ""))
                                {
                                    case "Inventory":                                        
                                        //GridBoxUpdateRequests.Add(GridBoxWindow.GridBoxPurpose.Inventory);
                                        Character.WearOrientation wearOrientation = RepresentedItem.wearOrientation;
                                        dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + RepresentedItem.Name + " in inventory", GridBoxWindow.GridBoxPurpose.None));
                                        if (Character.CurrentCharacter.HasFreeHand)
                                        {
                                            if (wearOrientation == Character.WearOrientation.None ||
                                                (wearOrientation == Character.WearOrientation.Right && Character.CurrentCharacter.LeftHand == null ||
                                                wearOrientation == Character.WearOrientation.Left && Character.CurrentCharacter.RightHand == null))
                                                dropDownMenuItemTextList.Add(Tuple.Create("remove", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name, GridBoxWindow.GridBoxPurpose.None));
                                            dropDownMenuItemTextList.Add(Tuple.Create("belt", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";belt it", GridBoxWindow.GridBoxPurpose.Sack));
                                            dropDownMenuItemTextList.Add(Tuple.Create("drop", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";drop it", GridBoxWindow.GridBoxPurpose.Ground));
                                            dropDownMenuItemTextList.Add(Tuple.Create("sack", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";put it in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                            dropDownMenuItemTextList.Add(Tuple.Create("pouch", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";put it in pouch", GridBoxWindow.GridBoxPurpose.Pouch));
                                            if (Character.CurrentCharacter.IsNextToCounter(out string locationName))
                                            {
                                                GridBoxWindow.GridBoxPurpose p = GridBoxWindow.GridBoxPurpose.Counter;
                                                if (locationName != "counter")
                                                    p = GridBoxWindow.GridBoxPurpose.Altar;
                                                dropDownMenuItemTextList.Add(Tuple.Create(locationName, "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";put it on " + locationName, p));
                                            }
                                            if (Character.CurrentCharacter.Cell.IsLockers)
                                                dropDownMenuItemTextList.Add(Tuple.Create("locker", "remove" + (wearOrientation != Character.WearOrientation.None ? " " + wearOrientation.ToString().ToLower() + " " : " ") + RepresentedItem.Name + ";put it in locker", GridBoxWindow.GridBoxPurpose.Locker));
                                            if(GridBoxUpdateRequests.Find(r => r.ToString() == "Inventory") != GridBoxWindow.GridBoxPurpose.Inventory) GridBoxUpdateRequests.Add(GridBoxWindow.GridBoxPurpose.Inventory);
                                        }
                                        break;
                                    case "Rings":
                                        Character.WearOrientation ringWearOrientation = RepresentedItem.wearOrientation;
                                        if (ringWearOrientation.ToString().StartsWith("Left"))
                                        {
                                            string num = ringWearOrientation.ToString().Replace("LeftRing", "");
                                            dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + num + " ring on left", GridBoxWindow.GridBoxPurpose.None));
                                            if (Character.CurrentCharacter.RightHand == null)
                                            {
                                                GridBoxUpdateRequests.Add(GridBoxWindow.GridBoxPurpose.Rings);
                                                dropDownMenuItemTextList.Add(Tuple.Create("remove", "remove " + num + " ring from left", GridBoxWindow.GridBoxPurpose.None));
                                                dropDownMenuItemTextList.Add(Tuple.Create("drop", "remove " + num + " ring from left;drop right", GridBoxWindow.GridBoxPurpose.Ground));
                                                dropDownMenuItemTextList.Add(Tuple.Create("sack", "remove " + num + " ring from left;put right in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                                dropDownMenuItemTextList.Add(Tuple.Create("pouch", "remove " + num + " ring from left;put right in pouch", GridBoxWindow.GridBoxPurpose.Pouch));
                                                if (Character.CurrentCharacter.IsNextToCounter(out string locationName))
                                                {
                                                    GridBoxWindow.GridBoxPurpose p = GridBoxWindow.GridBoxPurpose.Counter;
                                                    if (locationName != "counter")
                                                        p = GridBoxWindow.GridBoxPurpose.Altar;
                                                    dropDownMenuItemTextList.Add(Tuple.Create(locationName, "remove " + num + " ring from left;put right on " + locationName, p));
                                                }
                                                if (Character.CurrentCharacter.Cell.IsLockers)
                                                    dropDownMenuItemTextList.Add(Tuple.Create("locker", "remove " + num + " ring from left;put right in locker", GridBoxWindow.GridBoxPurpose.Locker));
                                            }
                                        }
                                        else
                                        {
                                            string num = ringWearOrientation.ToString().Replace("RightRing", "");
                                            dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + num + " ring on right", GridBoxWindow.GridBoxPurpose.None));
                                            if (Character.CurrentCharacter.LeftHand == null)
                                            {
                                                GridBoxUpdateRequests.Add(GridBoxWindow.GridBoxPurpose.Rings);
                                                dropDownMenuItemTextList.Add(Tuple.Create("remove", "remove " + num + " ring from right", GridBoxWindow.GridBoxPurpose.None));
                                                dropDownMenuItemTextList.Add(Tuple.Create("drop", "remove " + num + " ring from right;drop left", GridBoxWindow.GridBoxPurpose.Ground));
                                                dropDownMenuItemTextList.Add(Tuple.Create("sack", "remove " + num + " ring from right;put left in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                                dropDownMenuItemTextList.Add(Tuple.Create("pouch", "remove " + num + " ring from right;put left in pouch", GridBoxWindow.GridBoxPurpose.Pouch));
                                                if (Character.CurrentCharacter.IsNextToCounter(out string locationName))
                                                {
                                                    GridBoxWindow.GridBoxPurpose p = GridBoxWindow.GridBoxPurpose.Counter;
                                                    if (locationName != "counter")
                                                        p = GridBoxWindow.GridBoxPurpose.Altar;
                                                    dropDownMenuItemTextList.Add(Tuple.Create(locationName, "remove " + num + " ring from right;put right on " + locationName, p));
                                                }
                                                if (Character.CurrentCharacter.Cell.IsLockers)
                                                    dropDownMenuItemTextList.Add(Tuple.Create("locker", "remove " + num + " ring from right;put left in locker", GridBoxWindow.GridBoxPurpose.Rings));
                                            }
                                        }
                                        break;
                                }
                            }
                            else if(Name.StartsWith("RH") || Name.StartsWith("LH"))
                            {
                                dropDownMenuItemTextList.Add(Tuple.Create("look", "look at " + (Name.StartsWith("RH") ? "right" : "left"), GridBoxWindow.GridBoxPurpose.None));
                                if(RepresentedItem.Name != "corpse" && !RepresentedItem.Name.StartsWith("coin"))
                                    dropDownMenuItemTextList.Add(Tuple.Create("belt", "belt " + (Name.StartsWith("RH") ? "right" : "left"), GridBoxWindow.GridBoxPurpose.Belt));
                                if(RepresentedItem.Name != "corpse")
                                    dropDownMenuItemTextList.Add(Tuple.Create("sack", "put " + (Name.StartsWith("RH") ? "right" : "left") + " in sack", GridBoxWindow.GridBoxPurpose.Sack));
                                if (RepresentedItem.Name != "corpse" && !RepresentedItem.Name.StartsWith("coin"))
                                    dropDownMenuItemTextList.Add(Tuple.Create("pouch", "put " + (Name.StartsWith("RH") ? "right" : "left") + " in pouch", GridBoxWindow.GridBoxPurpose.Sack));
                                dropDownMenuItemTextList.Add(Tuple.Create("drop", "drop " + (Name.StartsWith("RH") ? "right" : "left"), GridBoxWindow.GridBoxPurpose.Ground));
                                string locationName = "counter";
                                if (Character.CurrentCharacter != null && Character.CurrentCharacter.IsNextToCounter(out locationName))
                                    dropDownMenuItemTextList.Add(Tuple.Create("put on " + locationName, "put " + (Name.StartsWith("RH") ? "right" : "left") + " on " + locationName, GridBoxWindow.GridBoxPurpose.Sack));
                            }

                            foreach (Tuple<string, string, GridBoxWindow.GridBoxPurpose> tuple in dropDownMenuItemTextList)
                            {
                                height += 20;
                                DropDownMenu.AddDropDownMenuItem(tuple.Item1, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Send_Command", tuple.Item2, false);
                                GridBoxUpdateRequests.Add(tuple.Item3);
                            }
                        }

                        if(DropDownMenu != null)
                            DropDownMenu.Height = height;
                    }
                }
                catch (Exception e)
                {
                    Utils.LogException(e);
                }
            }
        }

        public override bool KeyboardHandler(KeyboardState ks)
        {
            if (DropDownMenu != null)
                return DropDownMenu.KeyboardHandler(ks);

            return false;
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

            if (GuiManager.ActiveDropDownMenu == null)
            {
                if (cursor.DraggedButton == null || cursor.DraggedButton == this || AcceptingDroppedButtons)
                {
                    if (Border == null && !IsLocked || (Border == null && AcceptingDroppedButtons && GuiManager.MouseOverDropAcceptingControl == this))
                    {
                        SquareBorder border = new SquareBorder(Name + "SquareBorder", Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.DragAndDropBorderColor, 255);

                        if (Sheet == "Generic")
                            GuiManager.GenericSheet.AddControl(border);
                        else GuiManager.CurrentSheet.AddControl(border);
                    }

                    if (Border != null)
                    {
                        if (GuiManager.ActiveDropDownMenu == "" || (DropDownMenu != null && GuiManager.ActiveDropDownMenu == DropDownMenu.Name))
                        {
                            Border.IsVisible = true;
                        }

                        if (OriginalBorderColor != null)
                        { Border.TintColor = Client.ClientSettings.DragAndDropBorderColor; }
                    }
                }
            }

            if (AcceptingDroppedButtons && cursor != null && cursor.DraggedButton != null)// && GuiManager.MouseState.LeftButton == ButtonState.Pressed)
            {
                GuiManager.MouseOverDropAcceptingControl = this;
                cursor.DraggedButton.HasEnteredGridBoxWindow = true;
                if (Border != null)
                    Border.TintColor = Client.ClientSettings.AcceptingGridBoxBorderColor;
            }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
            {
                if (!HasOriginalBorderColor ||
                    Owner.Contains("GridBox") ||
                    Owner == "InventoryWindow" ||
                    Owner == "RingsWindow" ||
                    ((Name.StartsWith("RH") || Name.StartsWith("LH")) && Height < 60)) // bad code again :(
                    Border.IsVisible = false;
                else
                    Border.TintColor = OriginalBorderColor;
            }

            TextCue.ClearMouseCursorTextCue();
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (DropDownMenu != null) { DropDownMenu.MouseHandler(ms); }

            return base.MouseHandler(ms);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GuiManager.MouseState.LeftButton != ButtonState.Pressed)
                OnMouseRelease(GuiManager.MouseState);

            if (m_draggingToDrop)
                Position = new Point(GuiManager.MouseState.X - 10, GuiManager.MouseState.Y - 10);

            if (!HasOriginalBorderColor && !Contains(GuiManager.MouseState.Position))
                Border = null;

            if (Border != null)
            {
                if (DropDownMenu != null && GuiManager.ActiveDropDownMenu == DropDownMenu.Name)
                    Border.IsVisible = true;

                Border.Update(gameTime);
            }

            //if (!IsVisible || IsDisabled || RepresentedItem == null)
            //{
            //    if (DropDownMenu != null)
            //        DropDownMenu = null;
            //}

            //if (DropDownMenu != null)
            //    DropDownMenu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);

            if (DropDownMenu != null) DropDownMenu.Draw(gameTime);
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            //DropDownMenu = null;

            base.OnClientResize(prev, now, ownerOverride);
        }

        public void StopDragging()
        {
            Position = new Point(m_originalPosition.X, m_originalPosition.Y);
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
                    if (item2 == draggedButton.RepresentedItem || item2.VisualKey == draggedButton.RepresentedItem.VisualKey)
                        return count;

                    if (item2.VisualKey != draggedButton.RepresentedItem.VisualKey)
                        count++;
                }
            }

            return count;
        }
    }
}
