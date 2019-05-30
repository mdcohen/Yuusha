using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

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
            GridBox box = new GridBox(purpose.ToString() + "GridBox", "",
                new Rectangle(40, 40, (columns * columnWidth) + (Client.UserSettings.GridBoxButtonsBorderWidth * 2), (rows * rowHeight) + Client.UserSettings.GridBoxTitleHeight + (Client.UserSettings.GridBoxButtonsBorderWidth * 2)),
                false, false, false, Client.UserSettings.GridBoxWindowFont, new VisualKey("WhiteSpace"),
                Client.UserSettings.ColorGridBoxWindowTintColor, Client.UserSettings.GridBoxWindowVisualKeyAlpha, Client.UserSettings.GridBoxWindowBorderAlpha, true,
                Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left }, "", rows, columns, purpose);

            WindowTitle boxTitle = new WindowTitle(box.Name + "Title", box.Name, box.Font, purpose.ToString(), Client.UserSettings.GridBoxTitleTextColor, Client.UserSettings.GridBoxTitleTintColor, 255,
                BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false, new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"),
                Client.UserSettings.GridBoxTitleCloseBoxDistanceFromRight, Client.UserSettings.GridBoxTitleCloseBoxDistanceFromTop,
                Client.UserSettings.GridBoxTitleCloseBoxWidth, Client.UserSettings.GridBoxTitleCloseBoxHeight, Client.UserSettings.GridBoxTitleCloseBoxTintColor, Client.UserSettings.GridBoxTitleHeight);

            SquareBorder boxBorder = new SquareBorder(box.Name + "Border", box.Name, 1, new gui.VisualKey("WhiteSpace"), false, Client.UserSettings.GridBoxBorderTintColor, 255);

            GuiManager.GenericSheet.AddControl(box);
            GuiManager.GenericSheet.AttachControlToWindow(boxTitle);
            GuiManager.GenericSheet.AttachControlToWindow(boxBorder);

            box.IsLocked = true;

            return box;
        }

        public static void CreateGridBox(GridBoxFunction purpose)
        {
            int rows, columns = 0;
            GridBox box;

            switch(purpose)
            {
                case GridBoxFunction.Sack:
                    rows = 4;
                    columns = 5;
                    int size = 64;
                    box = CreateGridBox(GridBoxFunction.Sack, rows, columns, size, size);
                    int x = Client.UserSettings.GridBoxButtonsBorderWidth, y = Client.UserSettings.GridBoxTitleHeight + Client.UserSettings.GridBoxButtonsBorderWidth;
                    for(int i = 0; i < 20; i++)
                    {
                        string text = "balm";
                        if(Character.CurrentCharacter != null && Character.CurrentCharacter.Sack != null && Character.CurrentCharacter.Sack.Count >= i + 1)
                        {
                            text = Character.CurrentCharacter.Sack[i].name;
                        }
                        DragAndDropButton button = new DragAndDropButton(purpose.ToString() + "DragAndDropButton" + i, box.Name,
                            new Rectangle(x, y, size, size), text, false, Color.White, true, false, box.Font, new VisualKey("WhiteSpace"),
                            Color.Black, 255, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "",
                            BitmapFont.TextAlignment.Center, 0, 0, Color.PaleGreen, true, Color.DarkMagenta, true, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top },
                            false, Map.Direction.None, 0, "");
                        GuiManager.GenericSheet.AddControl(button);
                        x += size;
                        if (x > size * (columns - 1) + Client.UserSettings.GridBoxButtonsBorderWidth)
                        {
                            x = Client.UserSettings.GridBoxButtonsBorderWidth; y += size;
                        }
                    }
                    box.IsVisible = true;
                    break;
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            GuiManager.GenericSheet.RemoveControl(this);
        }
    }
}
