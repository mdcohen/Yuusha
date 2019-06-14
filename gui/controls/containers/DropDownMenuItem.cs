using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DropDownMenuItem : Control // should these inherit from Button? 6/11/2019
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

        public DropDownMenuItem(string name, string text, DropDownMenu menu, VisualKey visualKey, string onMouseDown, string command) : base()
        {
            m_name = name;
            m_text = text;
            m_dropDownMenu = menu;
            m_visualKey = visualKey;
            m_onMouseDown = onMouseDown;
            m_onMouseDownSent = false;
            Command = command;

            m_textColor = Client.UserSettings.ColorDropDownMenuItemText;
            m_tintColor = Client.UserSettings.ColorDropDownMenuItemBackground;
            m_textOverColor = Client.UserSettings.ColorDropDownMenuItemHighlight;
            m_font = Client.ClientSettings.DefaultDropDownMenuFont;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                if (m_text != null && m_text.Length > 0)
                {
                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
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
        }

        protected override void OnMouseOver(MouseState ms)
        {
            if(!m_disabled)
                this.TextColor = Client.UserSettings.ColorDropDownMenuItemHighlight;

            base.OnMouseOver(ms);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            if(!m_disabled)
                this.TextColor = Client.UserSettings.ColorDropDownMenuItemText;

            base.OnMouseLeave(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (m_disabled)
                return;

            if (!m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
            {
                GuiManager.AwaitMouseButtonRelease = true;

                if (m_onMouseDown != "")
                {
                    Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), m_onMouseDown, true), this);
                    m_onMouseDownSent = true;
                }

                if(DropDownMenu != null)
                {
                    if(DropDownMenu.DropDownMenuOwner is DragAndDropButton dButton)
                    {
                        if (DropDownMenu.DropDownMenuOwner.Owner.Contains("GridBoxWindow"))
                        {
                            GridBoxWindow.GridBoxPurpose purpose = (DropDownMenu.DropDownMenuOwner as DragAndDropButton).GridBoxUpdateRequests[DropDownMenu.MenuItems.IndexOf(this)];
                            GridBoxWindow.GridBoxPurpose ownerPurpose = (GuiManager.GetControl(DropDownMenu.DropDownMenuOwner.Owner) as GridBoxWindow).GridBoxPurposeType;

                            GridBoxWindow.RequestUpdateFromServer(purpose);
                            GridBoxWindow.RequestUpdateFromServer(ownerPurpose);
                        }
                    }

                    DropDownMenu.IsVisible = false;
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            // this is likely never called as DropDownMenus are nulled upon click of DropDownMenuItem

            if (m_disabled)
                return;

            m_onMouseDownSent = false;
        }
    }
}
