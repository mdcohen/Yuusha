﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DropDownMenu : Control
    {
        protected Border m_border;
        protected List<DropDownMenuItem> m_menuItems;
        public string Title
        { get; set; }
        protected Control m_menuOwner;

        public List<DropDownMenuItem> MenuItems
        { get { return m_menuItems; } }

        public Control DropDownMenuOwner
        {
            get { return m_menuOwner; }
        }

        public Border Border
        {
            get { return m_border; }
            set { m_border = value; }
        }

        public DropDownMenu(string name, string owner, string title, Rectangle rectangle, bool visible, string font, VisualKey visualKey, Color tintColor,
            int visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance) : base()
        {
            m_name = name;
            m_menuOwner = GuiManager.GetControl(owner);
            m_owner = owner;
            Title = title;
            m_rectangle = rectangle;
            m_visible = visible;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_dropShadow = dropShadow;
            m_shadowDirection = shadowDirection;
            m_shadowDistance = shadowDistance;
            m_menuItems = new List<DropDownMenuItem>();
            IsLocked = true;

            GuiManager.ActiveDropDownMenu = this;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Draw(gameTime);

            if (Title != null && Title.Length > 0 && BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                BitmapFont.ActiveFonts[Font].Alignment = BitmapFont.TextAlignment.Center;
                Rectangle rect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, BitmapFont.ActiveFonts[Font].LineHeight);
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.ClientSettings.ColorDropDownMenuTitleText, Title);
            }

            foreach (DropDownMenuItem menuItem in m_menuItems)
                if (menuItem.IsVisible) menuItem.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // an override due to an issue with ScrollableTextBox drop down menus
            if(!Contains(GuiManager.MouseState.Position))
                OnMouseLeave(GuiManager.MouseState);

            Point oldPosition = Position;

            if (Position.X + Width > Client.Width)
            {
                while (Position.X + Width > Client.Width)
                {
                    Position = new Point(Position.X - 1, Position.Y);
                }
            }
            if (Position.X < 0)
            {
                while (Position.X < 0)
                {
                    Position = new Point(Position.X + 1, Position.Y);
                }
            }
            if (Position.Y + Height > Client.Height)
            {
                while (Position.Y + Height > Client.Height)
                {
                    Position = new Point(Position.X, Position.Y - 1);
                }
            }
            if (Position.Y < 0)
            {
                while (Position.Y < 0)
                {
                    Position = new Point(Position.X, Position.Y + 1);
                }
            }

            if (oldPosition.X != Position.X || oldPosition.Y != Position.Y)
            {
                foreach (DropDownMenuItem menuItem in MenuItems)
                    menuItem.Position = new Point(menuItem.Position.X + (Position.X - oldPosition.X), menuItem.Position.Y + (Position.Y - oldPosition.Y));
            }

            base.Update(gameTime);

            try
            {
                Width = 0;

                if (!string.IsNullOrEmpty(Title))
                    Width = BitmapFont.ActiveFonts[Font].MeasureString(Title);

                foreach (DropDownMenuItem menuItem in MenuItems)
                {
                    int textWidth = BitmapFont.ActiveFonts[Client.ClientSettings.DefaultDropDownMenuFont].MeasureString(menuItem.Text);

                    if (Width < textWidth)
                        Width = textWidth + 1;

                    menuItem.Width = Width;
                }

                foreach (DropDownMenuItem menuItem in m_menuItems)
                    menuItem.Update(gameTime);

                if (Border != null)
                {
                    Border.Update(gameTime);
                    if (Border is SquareBorder sq) sq.ManuallySetRectangles(this);
                }
            }
            catch(System.Exception)
            {

            }
        }

        public override bool MouseHandler(MouseState ms)
        {
            foreach (DropDownMenuItem menuItem in m_menuItems)
                if (menuItem.MouseHandler(ms)) return true;

            if (base.MouseHandler(ms))
                return true;

            return false;
        }

        public void AddDropDownMenuItem(string text, string owner, VisualKey visualKey, string onMouseDown, string command, bool isDisabled)
        {
            // change DropDownMenu rectangle based on length of longest text in menu item collection

            int yMod = 0;

            if (Title != null && Title.Length > 0)
                yMod += 20;

            DropDownMenuItem menuItem = new DropDownMenuItem(Name + "MenuItem" + (m_menuItems.Count + 1).ToString(), text, this, visualKey, onMouseDown, command)
            {
                Rectangle = new Rectangle(m_rectangle.X, m_rectangle.Y + yMod + (m_menuItems.Count * 20), Width, BitmapFont.ActiveFonts[Client.ClientSettings.DefaultDropDownMenuFont].LineHeight),
                IsVisible = true,
                IsDisabled = isDisabled,
            };

            // automatically shrink or grow drop down menus for the longest menu item
            //int width = BitmapFont.ActiveFonts[Font].MeasureString(text);
            //if (Width + (Border == null ? 0 : Border.Width) < width)
            //    Width = width;

            m_menuItems.Add(menuItem);
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            Keys[] keys = ks.GetPressedKeys();

            foreach(Keys k in keys)
            {
                foreach (DropDownMenuItem item in MenuItems)
                {
                    if (item.Text.ToLower().StartsWith(((char)k).ToString().ToLower()))
                    {
                        if (!item.MouseDownSent)
                        {
                            GuiManager.AwaitKeyRelease.Add(k);
                            item.MouseDownSent = true;

                            GuiManager.AwaitMouseButtonRelease = true;

                            //if(GuiManager.GetControl(Owner) is CritterListLabel critterListLabel)
                            //    Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), item.MouseDown, true), critterListLabel);
                            //else
                            Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), item.MouseDown, true), item);

                            if (item.DropDownMenu != null)
                            {
                                if (item.DropDownMenu.DropDownMenuOwner is DragAndDropButton dButton)
                                {
                                    if (item.DropDownMenu.DropDownMenuOwner.Owner.Contains("GridBoxWindow"))
                                    {
                                        GridBoxWindow.GridBoxPurpose purpose = (item.DropDownMenu.DropDownMenuOwner as DragAndDropButton).GridBoxUpdateRequests[item.DropDownMenu.MenuItems.IndexOf(item)];
                                        GridBoxWindow.GridBoxPurpose ownerPurpose = (GuiManager.GetControl(item.DropDownMenu.DropDownMenuOwner.Owner) as GridBoxWindow).GridBoxPurposeType;
                                        GridBoxWindow.RequestUpdateFromServer(purpose);
                                        GridBoxWindow.RequestUpdateFromServer(ownerPurpose);
                                    }
                                    // otherwise Inventory
                                }

                                item.DropDownMenu.IsVisible = false;
                                item.DropDownMenu.OnDispose();
                                return true;
                            }
                        }
                    }
                }
            }

            return base.OnKeyDown(ks);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            IsVisible = false;
            IsDisabled = true;

            if (DropDownMenuOwner != null)
            {
                if (DropDownMenuOwner is CritterListLabel)
                {
                    GuiManager.Dispose((DropDownMenuOwner as CritterListLabel).DropDownMenu);
                    (DropDownMenuOwner as CritterListLabel).DropDownMenu = null;
                }
                else if(DropDownMenuOwner is TextBox)
                {
                    GuiManager.Dispose((DropDownMenuOwner as TextBox).DropDownMenu);
                    (DropDownMenuOwner as TextBox).DropDownMenu = null;
                }
                else if (DropDownMenuOwner is DragAndDropButton)
                {
                    GuiManager.Dispose((DropDownMenuOwner as DragAndDropButton).DropDownMenu);
                    (DropDownMenuOwner as DragAndDropButton).DropDownMenu = null;
                }
                else if (DropDownMenuOwner is ScrollableTextBox)
                {
                    GuiManager.Dispose((DropDownMenuOwner as ScrollableTextBox).DropDownMenu);
                    (DropDownMenuOwner as ScrollableTextBox).DropDownMenu = null;
                }
            }
        }

        public override void OnDispose()
        {
            MenuItems.Clear();

            if (GuiManager.ActiveDropDownMenu == this)
                GuiManager.ActiveDropDownMenu = null;

                base.OnDispose();
        }
    }
}
