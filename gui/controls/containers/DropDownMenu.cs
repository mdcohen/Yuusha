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

            GuiManager.ActiveDropDownMenu = Name;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Draw(gameTime);

            if (Title != null && this.Title.Length > 0 && BitmapFont.ActiveFonts.ContainsKey(this.Font))
            {
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                BitmapFont.ActiveFonts[Font].Alignment = BitmapFont.TextAlignment.Center;
                Rectangle rect = new Rectangle(this.m_rectangle.X, m_rectangle.Y, m_rectangle.Width, 30);
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.ClientSettings.ColorDropDownMenuTitleText, this.Title);
            }

            foreach (DropDownMenuItem menuItem in m_menuItems)
                if(menuItem.IsVisible) menuItem.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            // an override due to an issue with ScrollableTextBox drop down menus
            if(!Contains(GuiManager.MouseState.Position))
                OnMouseLeave(GuiManager.MouseState);

            base.Update(gameTime);

            Width = 0;
            if(Title != "")
                Width = BitmapFont.ActiveFonts[Font].MeasureString(Title);
            foreach(DropDownMenuItem menuItem in MenuItems)
            {
                int textWidth = BitmapFont.ActiveFonts[Client.ClientSettings.DefaultDropDownMenuFont].MeasureString(menuItem.Text);
                if (Width < textWidth)
                    Width = textWidth + 1;
                menuItem.Width = Width;
            }           

            if (Border != null)
                Border.Update(gameTime);

            foreach (DropDownMenuItem menuItem in m_menuItems)
                menuItem.Update(gameTime);
        }

        public override bool MouseHandler(MouseState ms)
        {
            bool handled = false;

            foreach (DropDownMenuItem menuItem in m_menuItems)
            {
                if (menuItem.MouseHandler(ms))
                    handled = true;
            }

            if (base.MouseHandler(ms))
                handled = true;

            return handled;
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

        protected override void OnMouseLeave(Microsoft.Xna.Framework.Input.MouseState ms)
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

            if (GuiManager.ActiveDropDownMenu == DropDownMenuOwner.Name)
                GuiManager.ActiveDropDownMenu = "";
        }

        public override void OnDispose()
        {
            if(GuiManager.ActiveDropDownMenu == Name)
                GuiManager.ActiveDropDownMenu = "";

            base.OnDispose();
        }
    }
}
