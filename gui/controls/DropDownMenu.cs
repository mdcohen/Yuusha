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
            this.m_name = name;
            this.m_menuOwner = owner;
            this.m_owner = owner.Name;
            this.m_title = title;
            this.m_rectangle = rectangle;
            this.m_visible = visible;
            this.m_font = font;
            this.m_visualKey = visualKey;
            this.m_tintColor = tintColor;
            this.m_visualAlpha = visualAlpha;
            this.m_dropShadow = dropShadow;
            this.m_shadowDirection = shadowDirection;
            this.m_shadowDistance = shadowDistance;
            this.m_menuItems = new List<DropDownMenuItem>();

            this.Border = new SquareBorder(this.Name + "SquareBorder", this.Name, 1, new VisualKey("WhiteSpace"), false, Client.UserSettings.ColorDropDownMenuBorder);
            this.Border.IsVisible = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Draw(gameTime);

            if (this.Border != null)
                this.Border.Draw(gameTime);

            if (this.m_title != null && this.m_title.Length > 0 && BitmapFont.ActiveFonts.ContainsKey(this.Font))
            {
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                BitmapFont.ActiveFonts[Font].Alignment = BitmapFont.TextAlignment.Center;
                // Rectangle rect = new Rectangle(this.m_rectangle.X + m_xTextOffset, m_textRectangle.Y + m_yTextOffset, m_textRectangle.Width, m_textRectangle.Height);
                Rectangle rect = new Rectangle(this.m_rectangle.X, m_rectangle.Y, m_rectangle.Width, 30);
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.UserSettings.ColorDropDownMenuTitleText, this.m_title);
            }

            foreach (DropDownMenuItem menuItem in m_menuItems)
                menuItem.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Border != null)
                this.Border.Update(gameTime);

            foreach (DropDownMenuItem menuItem in m_menuItems)
                menuItem.Update(gameTime);
        }

        public override bool MouseHandler(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            bool handled = false;

            foreach (DropDownMenuItem menuItem in this.m_menuItems)
            {
                if (menuItem.MouseHandler(ms))
                    handled = true;
            }

            if (base.MouseHandler(ms))
                handled = true;

            return handled;
        }

        public void AddDropDownMenuItem(string text, string owner, VisualKey visualKey, string onMouseDown)
        {
            // change DropDownMenu rectangle based on length of longest text in menu item collection

            int yMod = 0;

            if (this.m_title != null && this.m_title.Length > 0)
                yMod += 20;

            DropDownMenuItem menuItem = new DropDownMenuItem(this.Name + "MenuItem" + (m_menuItems.Count + 1).ToString(), text, this, visualKey, onMouseDown);
            menuItem.Rectangle = new Rectangle(this.m_rectangle.X, this.m_rectangle.Y + yMod + (this.m_menuItems.Count * 20), this.m_rectangle.Width, 18);
            menuItem.IsVisible = true;

            this.m_menuItems.Add(menuItem);
        }

        protected override void OnMouseLeave(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            this.IsVisible = false;
            this.IsDisabled = true;

            Control owner = GuiManager.GetControl(this.m_owner);

            if (owner != null)
            {
                if (owner is CritterListLabel)
                {
                    (owner as CritterListLabel).DropDownMenu = null;
                    return;
                }
            }
        }
    }
}
