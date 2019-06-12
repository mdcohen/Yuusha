using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class GridBoxWindow : Window
    {
        public enum GridBoxPurpose
        {
            Altar,
            Belt,
            Counter,
            Ground,
            Locker,
            Pouch,
            Rings,
            Sack,
            Skills,
            Spellbook,            
        }

        protected List<Button> GridBoxItemsList;
        public GridBoxPurpose GridBoxPurposeType;
        public bool HasNewData;

        public GridBoxWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow, Map.Direction shadowDirection,
            int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride, int numRows, int numColumns, GridBoxPurpose gridBoxPurpose) : base(name, owner, rectangle, visible, locked,
                disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            HasNewData = false;
            GridBoxPurposeType = gridBoxPurpose;
            GridBoxItemsList = new List<Button>();
        }

        public static GridBoxWindow CreateGridBox(GridBoxPurpose purpose, int rows, int columns, int rowHeight, int columnWidth)
        {
            if (GuiManager.GetControl(purpose.ToString() + "GridBox") != null)
            {
                //TextCue.AddClientInfoTextCue(purpose.ToString() + "GridBox exists.");
                GridBoxWindow existingBox = GuiManager.GetControl(purpose.ToString() + "GridBox") as GridBoxWindow;
                existingBox.RemoveDragAndDropButtons();
                return existingBox;
            }

            GridBoxWindow box = new GridBoxWindow(purpose.ToString() + "GridBox", "",
                new Rectangle(40, 40, (columns * columnWidth) + (Client.UserSettings.GridBoxButtonsBorderWidth * 2), (rows * rowHeight) + Client.UserSettings.GridBoxTitleHeight + (Client.UserSettings.GridBoxButtonsBorderWidth * 2)),
                false, false, false, Client.UserSettings.GridBoxWindowFont, new VisualKey("WhiteSpace"),
                Client.UserSettings.ColorGridBoxWindowTintColor, Client.UserSettings.GridBoxWindowVisualKeyAlpha, Client.UserSettings.GridBoxWindowBorderAlpha, true,
                Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left }, "", rows, columns, purpose);
            box.m_cursorOverride = "Dragging";

            WindowTitle boxTitle = new WindowTitle(box.Name + "Title", box.Name, box.Font, purpose.ToString(), Client.UserSettings.GridBoxTitleTextColor, Client.UserSettings.GridBoxTitleTintColor, 255,
                BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false, new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"),
                Client.UserSettings.GridBoxTitleCloseBoxDistanceFromRight, Client.UserSettings.GridBoxTitleCloseBoxDistanceFromTop,
                Client.UserSettings.GridBoxTitleCloseBoxWidth, Client.UserSettings.GridBoxTitleCloseBoxHeight, Client.UserSettings.GridBoxTitleCloseBoxTintColor, Client.UserSettings.GridBoxTitleHeight);

            SquareBorder boxBorder = new SquareBorder(box.Name + "Border", box.Name, 1, new VisualKey("WhiteSpace"), false, Client.UserSettings.GridBoxBorderTintColor, 255);

            GuiManager.GenericSheet.AddControl(box);
            GuiManager.GenericSheet.AttachControlToWindow(boxTitle);
            GuiManager.GenericSheet.AttachControlToWindow(boxBorder);
            return box;
        }

        public static void CreateGridBox(GridBoxPurpose purpose)
        {
            int rows = 0, columns = 0, x = 0, y = 0, count = 0, size = 64;
            GridBoxWindow box = null;
            List<Item> itemsList = new List<Item>();

            switch(purpose)
            {
                case GridBoxPurpose.Belt: // 5 slots
                    rows = 1;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Belt, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Belt != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Belt);
                    else return;
                    break;
                case GridBoxPurpose.Locker: // 20 slots
                    #region Locker
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Locker, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Locker != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Locker);
                    else return;
                    #endregion
                    break;
                case GridBoxPurpose.Ground:
                    #region Ground
                    if (GameHUD.ExaminedCell == null) return;
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Ground, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    itemsList = new List<Item>(GameHUD.ExaminedCell.Items);
                    if(GameHUD.ExaminedCell != null && box.WindowTitle != null)
                    {
                        if (GameHUD.ExaminedCell.displayGraphic == "mm") box.WindowTitle.Text = "Altar";
                        else if (GameHUD.ExaminedCell.displayGraphic == "==") box.WindowTitle.Text = "Counter";
                    }
                    break;
                #endregion
                case GridBoxPurpose.Pouch: // 20 slots
                    #region Pouch
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Pouch, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Pouch != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Pouch);
                    else return;
                    #endregion
                    break;
                case GridBoxPurpose.Rings: // 20 slots
                    #region Rings
                    rows = 4;
                    columns = 2;
                    box = CreateGridBox(GridBoxPurpose.Rings, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Rings != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Rings);
                    else return;
                    break;
                #endregion
                case GridBoxPurpose.Sack: // 20 slots
                    #region Sack
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Sack, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Sack != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Sack);
                    else return;
                    #endregion
                    break;
            }

            // some grid boxes have Item lists, others have string lists
            foreach (Item item in itemsList)
            {
                DragAndDropButton button = new DragAndDropButton(purpose.ToString() + "DragAndDropButton" + count, box.Name,
                    new Rectangle(x, y, size, size), item.name, false, Color.White, true, false, "courier12", new VisualKey(item.visualKey),
                    Color.White, 255, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "",
                    BitmapFont.TextAlignment.Center, 0, 0, Color.PaleGreen, true, Color.FloralWhite, true, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top },
                    false, Map.Direction.None, 0, item.name);
                button.RepresentedItem = item;
                button.AcceptingDroppedButtons = false;
                //button.DrawBlackBackground = true;

                Control existingButton = GuiManager.GenericSheet[button.Name];
                if (existingButton != null)
                    GuiManager.GenericSheet.RemoveControl(existingButton);

                GuiManager.GenericSheet.AddControl(button);
                x += size;
                if (x > size * (columns - 1) + Client.UserSettings.GridBoxButtonsBorderWidth)
                {
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y += size;
                }
                count++;
            }

            box.IsVisible = true;
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];

            if (cursor != null && cursor.DraggedButton != null && (cursor.Owner == "" || cursor.DraggedButton.Owner != this.Owner))
            {
                GuiManager.MouseOverDropAcceptingControl = this;
                cursor.DraggedButton.HasEnteredGridBoxWindow = true;
            }

            if ((GuiManager.Cursors[GuiManager.GenericSheet.Cursor] as MouseCursor).DraggedButton != null)
            {
                if (WindowBorder != null)
                    WindowBorder.TintColor = Client.UserSettings.AcceptingGridBoxBorderColor;

                if (WindowTitle != null)
                {
                    WindowTitle.TintColor = Client.UserSettings.AcceptingGridBoxTitleColor;
                    WindowTitle.TextColor = Client.UserSettings.AcceptingGridBoxTitleTextColor;
                }
            }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];

            if (cursor != null && cursor.DraggedButton != null && cursor.DraggedButton.Owner == this.Owner)
            {
                if (GuiManager.MouseOverDropAcceptingControl == this) GuiManager.MouseOverDropAcceptingControl = null;
                cursor.DraggedButton.HasEnteredGridBoxWindow = false;
            }

            if (WindowBorder != null)
                WindowBorder.TintColor = Client.UserSettings.GridBoxBorderTintColor;

            if (WindowTitle != null)
            {
                WindowTitle.TintColor = Client.UserSettings.GridBoxTitleTintColor;
                WindowTitle.TextColor = Client.UserSettings.GridBoxTitleTextColor;
            }
        }

        public override void OnClose()
        {
            //if (GridBoxPurposeType == GridBoxPurpose.Ground)
            //    GameHUD.ExaminedCell = null;

            base.OnClose();
        }

        public void RemoveDragAndDropButtons()
        {
            foreach(Control c in new List<Control>(Controls))
            {
                if (c is DragAndDropButton)
                    Controls.Remove(c);
            }
        }

        public int GetItemsCount(string name)
        {
            int count = 0;

            foreach(Control c in Controls)
            {
                if(c is DragAndDropButton dbutton)
                {
                    if (dbutton.RepresentedItem != null && dbutton.RepresentedItem.name == name)
                        count++;
                }
            }

            return count;
        }
    }
}
