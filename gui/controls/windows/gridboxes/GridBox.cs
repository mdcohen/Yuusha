using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class GridBox : Window
    {
        public enum GridBoxFunction
        {
            Spellbook,
            Sack,
            Pouch,
            Belt,
            Locker,
            Ground,
            Counter
        }

        protected List<Button> GridBoxItemsList;
        public GridBoxFunction GridBoxFunctionType;
        public bool HasNewData;

        public GridBox(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow, Map.Direction shadowDirection,
            int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride, int numRows, int numColumns, GridBoxFunction gridBoxFunction) : base(name, owner, rectangle, visible, locked,
                disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            HasNewData = false;
            GridBoxFunctionType = gridBoxFunction;
            GridBoxItemsList = new List<Button>();
        }

        public static GridBox CreateGridBox(GridBoxFunction purpose, int rows, int columns, int rowHeight, int columnWidth)
        {
            
            if (GuiManager.GetControl(purpose.ToString() + "GridBox") != null)
            {
                GridBox existingBox = GuiManager.GetControl(purpose.ToString() + "GridBox") as GridBox;
                existingBox.RemoveDragAndDropButtons();
                return existingBox;
            }

            GridBox box = new GridBox(purpose.ToString() + "GridBox", "",
                new Rectangle(40, 40, (columns * columnWidth) + (Client.UserSettings.GridBoxButtonsBorderWidth * 2), (rows * rowHeight) + Client.UserSettings.GridBoxTitleHeight + (Client.UserSettings.GridBoxButtonsBorderWidth * 2)),
                false, false, false, Client.UserSettings.GridBoxWindowFont, new VisualKey("WhiteSpace"),
                Client.UserSettings.ColorGridBoxWindowTintColor, Client.UserSettings.GridBoxWindowVisualKeyAlpha, Client.UserSettings.GridBoxWindowBorderAlpha, true,
                Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left }, "", rows, columns, purpose);
            box.m_cursorOverride = "Dragging";

            WindowTitle boxTitle = new WindowTitle(box.Name + "Title", box.Name, box.Font, purpose.ToString(), Client.UserSettings.GridBoxTitleTextColor, Client.UserSettings.GridBoxTitleTintColor, 255,
                BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false, new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"),
                Client.UserSettings.GridBoxTitleCloseBoxDistanceFromRight, Client.UserSettings.GridBoxTitleCloseBoxDistanceFromTop,
                Client.UserSettings.GridBoxTitleCloseBoxWidth, Client.UserSettings.GridBoxTitleCloseBoxHeight, Client.UserSettings.GridBoxTitleCloseBoxTintColor, Client.UserSettings.GridBoxTitleHeight);

            SquareBorder boxBorder = new SquareBorder(box.Name + "Border", box.Name, 1, new gui.VisualKey("WhiteSpace"), false, Client.UserSettings.GridBoxBorderTintColor, 255);

            GuiManager.GenericSheet.AddControl(box);
            GuiManager.GenericSheet.AttachControlToWindow(boxTitle);
            GuiManager.GenericSheet.AttachControlToWindow(boxBorder);
            return box;
        }

        public static void CreateGridBox(GridBoxFunction purpose)
        {
            int rows = 0, columns = 0, x = 0, y = 0, count = 0, size = 64;
            GridBox box = null;
            List<Item> itemsList = new List<Item>();

            switch(purpose)
            {
                case GridBoxFunction.Belt:
                    rows = 1;
                    columns = 5;
                    box = CreateGridBox(GridBoxFunction.Belt, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Belt != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Belt);
                    else return;
                    break;
                case GridBoxFunction.Pouch:
                    #region Pouch
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxFunction.Sack, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Pouch != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Pouch);
                    else return;
                    #endregion
                    break;
                case GridBoxFunction.Sack:
                    #region Sack
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxFunction.Sack, rows, columns, size, size);
                    x = Client.UserSettings.GridBoxButtonsBorderWidth;
                    y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Sack != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Sack);
                    else return;
                    #endregion
                    break;
            }

            foreach (Item item in itemsList)
            {
                DragAndDropButton button = new DragAndDropButton(purpose.ToString() + "DragAndDropButton" + count, box.Name,
                    new Rectangle(x, y, size, size), item.name, true, Color.White, true, false, "courier12", new VisualKey("WhiteSpace"),
                    Color.Black, 255, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "",
                    BitmapFont.TextAlignment.Center, 0, 0, Color.PaleGreen, true, Color.DarkMagenta, true, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top },
                    false, Map.Direction.None, 0, item.name);
                button.RepresentedItem = item;
                button.AcceptingDroppedButtons = false;

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
        }

        public override void OnClose()
        {
            base.OnClose();
            GuiManager.GenericSheet.RemoveControl(this);
        }

        public void RemoveDragAndDropButtons()
        {
            foreach(Control c in new List<Control>(Controls))
            {
                if (c is DragAndDropButton)
                    Controls.Remove(c);
            }
        }
    }
}
