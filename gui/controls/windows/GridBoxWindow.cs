using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class GridBoxWindow : Window
    {
        public enum GridBoxPurpose
        {
            None,
            Altar,
            Counter,
            Ground,
            Belt,            
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
        private int Rows;
        private int Columns;
        private int RowHeight;
        private int ColumnWidth;

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
            if (GuiManager.GetControl(purpose.ToString() + "GridBoxWindow") != null)
            {
                GridBoxWindow existingBox = GuiManager.GetControl(purpose.ToString() + "GridBoxWindow") as GridBoxWindow;
                existingBox.RemoveDragAndDropButtons();
                if (existingBox.WindowTitle != null) existingBox.WindowTitle.Text = purpose.ToString(); // counter, altar, ground use same gridbox
                return existingBox;
            }

            GridBoxWindow box = new GridBoxWindow(purpose.ToString() + "GridBoxWindow", "",
                new Rectangle(40, 40, (columns * columnWidth) + (Client.ClientSettings.GridBoxButtonsBorderWidth * 2), (rows * rowHeight) + Client.ClientSettings.GridBoxTitleHeight + (Client.ClientSettings.GridBoxButtonsBorderWidth * 2)),
                false, false, false, Client.ClientSettings.GridBoxWindowFont, new VisualKey("WhiteSpace"),
                Client.ClientSettings.GridBoxWindowTintColor, Client.ClientSettings.GridBoxWindowVisualKeyAlpha, Client.ClientSettings.GridBoxWindowBorderAlpha, true,
                Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left }, "", rows, columns, purpose);
            box.Rows = rows;
            box.Columns = columns;
            box.RowHeight = rowHeight;
            box.ColumnWidth = columnWidth;
            box.m_cursorOverride = "Dragging";

            WindowTitle boxTitle = new WindowTitle(box.Name + "Title", box.Name, box.Font, purpose.ToString(), Client.ClientSettings.GridBoxTitleTextColor, Client.ClientSettings.GridBoxTitleTintColor, 255,
                BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false, new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"),
                Client.ClientSettings.GridBoxTitleCloseBoxDistanceFromRight, Client.ClientSettings.GridBoxTitleCloseBoxDistanceFromTop,
                Client.ClientSettings.GridBoxTitleCloseBoxWidth, Client.ClientSettings.GridBoxTitleCloseBoxHeight, Client.ClientSettings.GridBoxTitleCloseBoxTintColor, Client.ClientSettings.GridBoxTitleHeight);

            SquareBorder boxBorder = new SquareBorder(box.Name + "Border", box.Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.GridBoxBorderTintColor, 255);

            GuiManager.GenericSheet.AddControl(box);
            GuiManager.GenericSheet.AttachControlToWindow(boxTitle);
            GuiManager.GenericSheet.AttachControlToWindow(boxBorder);
            return box;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int count = 0;
            foreach(Control control in Controls)
            {
                if (control is DragAndDropButton)
                    count++;
            }

            if (count > (Rows * Columns))
                Height = (count++ / Columns) * RowHeight + (WindowTitle is null ? 0 : WindowTitle.Height) + RowHeight;
            else Height = RowHeight * Rows + (WindowTitle is null ? 0 : WindowTitle.Height);
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
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Belt != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Belt);
                    else return;
                    break;
                case GridBoxPurpose.Locker: // 20 slots
                    #region Locker
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Locker, rows, columns, size, size);
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Locker != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Locker);
                    else return;
                    #endregion
                    break;
                case GridBoxPurpose.Altar:
                case GridBoxPurpose.Counter:
                case GridBoxPurpose.Ground:
                    #region Ground
                    if (GameHUD.ExaminedCell == null) return;
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(purpose, rows, columns, size, size);
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight;
                    itemsList = new List<Item>(GameHUD.ExaminedCell.Items);
                    if(GameHUD.ExaminedCell != null && box.WindowTitle != null)
                    {
                        if (GameHUD.ExaminedCell.DisplayGraphic == "mm") box.WindowTitle.Text = "Altar";
                        else if (GameHUD.ExaminedCell.DisplayGraphic == "==") box.WindowTitle.Text = "Counter";
                    }
                    break;
                #endregion
                case GridBoxPurpose.Pouch: // 20 slots
                    #region Pouch
                    rows = 4;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Pouch, rows, columns, size, size);
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight;
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
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Rings != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Rings);
                    else return;
                    break;
                #endregion
                case GridBoxPurpose.Sack: // 20 slots
                    #region Sack
                    rows = 5;
                    columns = 5;
                    box = CreateGridBox(GridBoxPurpose.Sack, rows, columns, size, size);
                    x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                    y = Client.ClientSettings.GridBoxTitleHeight + Client.ClientSettings.GridBoxButtonsBorderWidth;
                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Sack != null)
                        itemsList = new List<Item>(Character.CurrentCharacter.Sack);
                    else return;
                    #endregion
                    break;
            }

            // some grid boxes have Item lists, others have string lists
            //itemsList.Reverse();
            Dictionary<string, int> countDictionary = new Dictionary<string, int>(); // visualKey, count
            foreach (Item item in itemsList)
            {
                // determine if we're sorting here or not -- an option
                DragAndDropButton button;
                if (!Client.ClientSettings.GroupSimiliarItemsInGridBoxes ||
                    (Client.ClientSettings.GroupSimiliarItemsInGridBoxes && (!countDictionary.ContainsKey(item.VisualKey) || item.VisualKey.ToLower() == "unknown")))
                {
                    if(Client.ClientSettings.GroupSimiliarItemsInGridBoxes && item.VisualKey.ToLower() != "unknown")
                        countDictionary.Add(item.VisualKey, 1);
                    button = new DragAndDropButton(purpose.ToString() + "DragAndDropButton" + count, box.Name,
                    new Rectangle(x, y, size, size), item.Name, false, Client.ClientSettings.DragAndDropTextColor, true, false, "courier12", new VisualKey(item.VisualKey),
                    Color.White, 255, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "", BitmapFont.TextAlignment.Center, 0, 0,
                    Client.ClientSettings.DragAndDropTextOverColor, Client.ClientSettings.DragAndDropHasTextOverColor, Client.ClientSettings.DragAndDropTintOverColor,
                    Client.ClientSettings.DragAndDropHasTintOverColor, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top },
                    false, Map.Direction.None, 0, item.Name, false)
                    {
                        RepresentedItem = item,
                        AcceptingDroppedButtons = false
                    };
                    x += size;
                    if (x > size * (columns - 1) + Client.ClientSettings.GridBoxButtonsBorderWidth)
                    {
                        x = Client.ClientSettings.GridBoxButtonsBorderWidth;
                        y += size;
                    }

                    Control existingButton = GuiManager.GenericSheet[button.Name];
                    if (existingButton != null)
                        GuiManager.GenericSheet.RemoveControl(existingButton);

                    GuiManager.GenericSheet.AddControl(button);
                }
                else
                {
                    button = box.Controls.Find(b1 => b1.VisualKey == item.VisualKey) as DragAndDropButton;
                    countDictionary[item.VisualKey]++;
                    button.Text = countDictionary[item.VisualKey].ToString();
                    button.IsTextVisible = true;
                    button.TextAlignment = BitmapFont.TextAlignment.Right;
                    button.YTextOffset = button.Height - BitmapFont.ActiveFonts[button.Font].LineHeight;
                }
                
                //button.DrawBlackBackground = true;
                
                count++;
            }

            // box isn't big enough to hold all the drag and drop buttons...
            //while(box.Height - (box.WindowTitle == null ? 0 : box.WindowTitle.Height) < count * size)
            //    box.Height += size;

            //if (!GameHUD.NextGridBoxUpdateIsSilent || Client.ClientSettings.AlwaysOpenGridBoxWindowsUponActivity)
            //{
            //    box.IsVisible = true;
            //    box.ZDepth = 1;
            //}

            //GameHUD.NextGridBoxUpdateIsSilent = false;
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
                    WindowBorder.TintColor = Client.ClientSettings.AcceptingGridBoxBorderColor;

                if (WindowTitle != null)
                {
                    WindowTitle.TintColor = Client.ClientSettings.AcceptingGridBoxTitleColor;
                    WindowTitle.TextColor = Client.ClientSettings.AcceptingGridBoxTitleTextColor;
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
                WindowBorder.TintColor = Client.ClientSettings.GridBoxBorderTintColor;

            if (WindowTitle != null)
            {
                WindowTitle.TintColor = Client.ClientSettings.GridBoxTitleTintColor;
                WindowTitle.TextColor = Client.ClientSettings.GridBoxTitleTextColor;
            }
        }

        public void RemoveDragAndDropButtons()
        {
            foreach(Control c in new List<Control>(Controls))
            {
                if (c is DragAndDropButton)
                    Controls.Remove(c);
            }
        }

        //public static int GetItemsCount(string itemName, string gridBox)
        //{
        //    GridBoxWindow box = GuiManager.GetControl(gridBox) as GridBoxWindow;
        //    return box.GetItemsCount(itemName);
        //}

        public int GetItemsCount(string name)
        {
            int count = 0;

            foreach(Control c in Controls)
            {
                if(c is DragAndDropButton dbutton)
                {
                    if(Client.ClientSettings.GroupSimiliarItemsInGridBoxes && 
                        dbutton.RepresentedItem != null && dbutton.RepresentedItem.Name == name)
                    {
                        // only has text to parse if count is 2+
                        if (int.TryParse(dbutton.Text, out int groupedCount))
                            count += groupedCount;
                        else count++;
                        continue;
                    }

                    if (dbutton.RepresentedItem != null && dbutton.RepresentedItem.Name == name)
                        count++;
                }
            }

            return count;
        }

        public static void RequestUpdateFromServer(GridBoxPurpose purpose)
        {
            switch (purpose)
            {
                case GridBoxPurpose.Altar:
                case GridBoxPurpose.Counter:
                case GridBoxPurpose.Ground:
                    Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    break;
                case GridBoxPurpose.Belt:
                    IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                    break;
                case GridBoxPurpose.Locker:
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                    break;
                case GridBoxPurpose.Pouch:
                    IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
                    break;
                case GridBoxPurpose.Rings:
                    IO.Send(Protocol.REQUEST_CHARACTER_RINGS);
                    break;
                case GridBoxPurpose.Sack:
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                    break;
                case GridBoxPurpose.Skills:
                    IO.Send(Protocol.REQUEST_CHARACTER_SKILLS);
                    break;
                case GridBoxPurpose.Spellbook:
                    IO.Send(Protocol.REQUEST_CHARACTER_SPELLS);
                    break;
                default:
                    break;
                    
            }
        }
    }
}
