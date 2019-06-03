using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DropDownMenuItem : Control
    {
        private bool m_onMouseDownSent;
        private DropDownMenu m_dropDownMenu;

        public DropDownMenu DropDownMenu
        {
            get { return m_dropDownMenu; }
        }

        public Rectangle Rectangle
        {
            get { return m_rectangle; }
            set { m_rectangle = value; }
        }

        public DropDownMenuItem(string name, string text, DropDownMenu menu, VisualKey visualKey, string onMouseDown) : base()
        {
            m_name = name;
            m_text = text;
            m_dropDownMenu = menu;
            m_visualKey = visualKey;
            m_onMouseDown = onMouseDown;
            m_onMouseDownSent = false;

            m_textColor = Client.UserSettings.ColorDropDownMenuItemText;
            m_tintColor = Client.UserSettings.ColorDropDownMenuItemBackground;
            m_font = Client.ClientSettings.DefaultDropDownMenuFont;
        }

        public override void Draw(GameTime gameTime)
        {
            //if (Text == "") return;

            base.Draw(gameTime);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                if (m_text != null && m_text.Length > 0)
                {
                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = m_textAlignment;
                    // draw string in textbox
                    Rectangle rect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
                    // change color of text if mouse over text color is not null
                    BitmapFont.ActiveFonts[Font].TextBox(rect, m_textColor, m_text);
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for DropDownMenuItem [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Text == "" || this.Text.Length <= 0)
            {
                this.IsVisible = false;
                return;
            }

            base.Update(gameTime);

            MouseState ms = Mouse.GetState();

            Point p = new Point(ms.X, ms.Y);

            if (Contains(p))
                this.TintColor = Client.UserSettings.ColorDropDownMenuItemHighlight;
            else this.TintColor = Client.UserSettings.ColorDropDownMenuItemBackground;
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (m_disabled)
                return;

            if (!m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
            {
                if (m_onMouseDown != "")
                {
                    Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), m_onMouseDown, true), this);
                    m_onMouseDownSent = true;
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            // this is likely never called as DropDownMenus are nulled upon click of DropDownMenuItem

            if (m_disabled)
                return;

            //if(ms.RightButton == ButtonState.Released)
                m_onMouseDownSent = false;
        }
    }
}
