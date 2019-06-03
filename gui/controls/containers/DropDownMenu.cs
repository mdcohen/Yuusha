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

            Border = new SquareBorder(this.Name + "SquareBorder", this.Name, 1, new VisualKey("WhiteSpace"), false, Client.UserSettings.ColorDropDownMenuBorder, visualAlpha)
            {
                IsVisible = true
            };
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);

            if (this.m_title != null && this.m_title.Length > 0 && BitmapFont.ActiveFonts.ContainsKey(this.Font))
            {
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                BitmapFont.ActiveFonts[Font].Alignment = BitmapFont.TextAlignment.Center;
                Rectangle rect = new Rectangle(this.m_rectangle.X, m_rectangle.Y, m_rectangle.Width, 30);
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.UserSettings.ColorDropDownMenuTitleText, this.m_title);
            }

            foreach (DropDownMenuItem menuItem in m_menuItems)
                if(menuItem.IsVisible) menuItem.Draw(gameTime);
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

            foreach (DropDownMenuItem menuItem in m_menuItems)
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

            DropDownMenuItem menuItem = new DropDownMenuItem(this.Name + "MenuItem" + (m_menuItems.Count + 1).ToString(), text, this, visualKey, onMouseDown)
            {
                Rectangle = new Rectangle(this.m_rectangle.X, this.m_rectangle.Y + yMod + (this.m_menuItems.Count * 20), this.m_rectangle.Width, 18),
                IsVisible = true
            };

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
