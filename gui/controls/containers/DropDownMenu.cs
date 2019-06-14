using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class DropDownMenu : Control
    {
        protected Border m_border;
        protected List<DropDownMenuItem> m_menuItems;
        protected string m_title;
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

        public DropDownMenu(string name, Control owner, string title, Rectangle rectangle, bool visible, string font, VisualKey visualKey, Color tintColor,
            int visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance) : base()
        {
            m_name = name;
            m_menuOwner = owner;
            m_owner = owner.Name;
            m_title = title;
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
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Draw(gameTime);

            if (m_title != null && this.m_title.Length > 0 && BitmapFont.ActiveFonts.ContainsKey(this.Font))
            {
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                BitmapFont.ActiveFonts[Font].Alignment = BitmapFont.TextAlignment.Center;
                Rectangle rect = new Rectangle(this.m_rectangle.X, m_rectangle.Y, m_rectangle.Width, 30);
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.UserSettings.ColorDropDownMenuTitleText, this.m_title);
            }

            foreach (DropDownMenuItem menuItem in m_menuItems)
                if(menuItem.IsVisible) menuItem.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
            else Utils.LogOnce(Name + " border is null.");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Border != null)
                Border.Update(gameTime);

            foreach (DropDownMenuItem menuItem in m_menuItems)
                menuItem.Update(gameTime);
        }

        public override bool MouseHandler(Microsoft.Xna.Framework.Input.MouseState ms)
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

            if (m_title != null && m_title.Length > 0)
                yMod += 20;

            DropDownMenuItem menuItem = new DropDownMenuItem(Name + "MenuItem" + (m_menuItems.Count + 1).ToString(), text, this, visualKey, onMouseDown, command)
            {
                Rectangle = new Rectangle(m_rectangle.X, m_rectangle.Y + yMod + (m_menuItems.Count * 20), m_rectangle.Width, 18),
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
                    return;
                }
                else if(DropDownMenuOwner is TextBox)
                {
                    GuiManager.Dispose((DropDownMenuOwner as TextBox).DropDownMenu);
                    (DropDownMenuOwner as TextBox).DropDownMenu = null;
                    return;
                }
                else if (DropDownMenuOwner is DragAndDropButton)
                {
                    GuiManager.Dispose((DropDownMenuOwner as DragAndDropButton).DropDownMenu);
                    (DropDownMenuOwner as DragAndDropButton).DropDownMenu = null;
                    return;
                }
            }
        }
    }
}
