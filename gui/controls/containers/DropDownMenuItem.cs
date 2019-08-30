using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class DropDownMenuItem : Control // should these inherit from Button? 6/11/2019
    {
        private readonly bool m_isLabel = false; // no mouse handling
        private readonly bool m_isSeparator = false;

        public DropDownMenu DropDownMenu
        {get;set;}

        public string MouseDown
        { get { return m_onMouseDown; } }
        public bool MouseDownSent
        { get; set; }

        public Rectangle Rectangle
        {
            get { return m_rectangle; }
            set { m_rectangle = value; }
        }

        public DropDownMenuItem(string name, string text, DropDownMenu menu, VisualKey visualKey, string onMouseDown, string command) : base()
        {
            m_name = name;
            m_text = text;
            DropDownMenu = menu;
            Font = menu.Font;
            m_visualKey = visualKey;
            m_onMouseDown = onMouseDown;
            MouseDownSent = false;
            Command = command;

            if (text == "-")
                m_isSeparator = true;
            if (text.StartsWith("#"))
            {
                m_isLabel = true;
                text = text.Replace("#", "");
            }

            m_textColor = Client.ClientSettings.ColorDropDownMenuItemText;

            if (m_isSeparator) m_textColor = Client.ClientSettings.ColorDropDownMenuSeparator;
            if (m_isLabel) m_textColor = Client.ClientSettings.ColorDropDownMenuLabelText;

            m_tintColor = Client.ClientSettings.ColorDropDownMenuItemBackground;
            m_textOverColor = Client.ClientSettings.ColorDropDownMenuItemHighlight;
            m_font = Client.ClientSettings.DefaultDropDownMenuFont;

            IsLocked = true;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                if (m_text != null && m_text.Length > 0)
                {
                    Color textColor = m_textColor;

                    if (m_disabled)
                        textColor = Client.ClientSettings.ColorDropDownMenuItemDisabledText;

                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                    // draw string in textbox
                    Rectangle rect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
                    // change color of text if mouse over text color is not null
                    BitmapFont.ActiveFonts[Font].TextBox(rect, textColor, m_text);
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for DropDownMenuItem [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
        }

        public override void Update(GameTime gameTime)
        {
            if (Text == "" || Text.Length <= 0)
            {
                IsVisible = false;
                return;
            }

            if(DropDownMenu != null)
                Width = DropDownMenu.Width;

            base.Update(gameTime);
        }

        protected override void OnMouseOver(MouseState ms)
        {
            if(!m_disabled)
                TextColor = Client.ClientSettings.ColorDropDownMenuItemHighlight;

            base.OnMouseOver(ms);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            if(!m_disabled)
                TextColor = Client.ClientSettings.ColorDropDownMenuItemText;

            if (GuiManager.ActiveDropDownMenu == Owner)
                GuiManager.ActiveDropDownMenu = "";

            base.OnMouseLeave(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (m_disabled)
                return;

            if (!MouseDownSent && ms.LeftButton == ButtonState.Pressed)
            {
                GuiManager.AwaitMouseButtonRelease = true;

                if (m_onMouseDown != "")
                {
                    Events.RegisterEvent((Events.EventName)Enum.Parse(typeof(Events.EventName), m_onMouseDown, true), this);
                    MouseDownSent = true;
                    DropDownMenu.IsVisible = false;
                    GuiManager.ActiveDropDownMenu = "";
                }

                if(DropDownMenu != null)
                {
                    if(DropDownMenu.DropDownMenuOwner is DragAndDropButton dButton)
                    {
                        if (dButton.Owner.Contains("GridBoxWindow") || dButton.Name.StartsWith("RH") || dButton.Name.StartsWith("LH"))
                        {
                            if (dButton.GridBoxUpdateRequests.Count >= DropDownMenu.MenuItems.IndexOf(this) + 1)
                            {
                                GridBoxWindow.RequestUpdateFromServer(dButton.GridBoxUpdateRequests[DropDownMenu.MenuItems.IndexOf(this)]);
                                dButton.GridBoxUpdateRequests.Clear();                               
                            }

                            GridBoxWindow.GridBoxPurpose p = (GuiManager.GetControl(DropDownMenu.DropDownMenuOwner.Owner) as GridBoxWindow).GridBoxPurposeType;
                            GridBoxWindow.RequestUpdateFromServer(p);
                            //Utils.Log("Sent GridBoxWindow.RequestUpdateFromServer for " + p.ToString());
                        }
                        // otherwise Inventory
                    }

                    DropDownMenu.IsVisible = false;
                    GuiManager.ActiveDropDownMenu = "";
                    //DropDownMenu = null;
                }

                if (DropDownMenu != null)
                    GuiManager.Dispose(DropDownMenu);

                base.OnMouseDown(ms);
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            // this is likely never called as DropDownMenus are nulled upon click of DropDownMenuItem

            if (m_disabled)
                return;

            MouseDownSent = false;

            if(GuiManager.ActiveDropDownMenu == Owner)
                GuiManager.ActiveDropDownMenu = "";
        }
    }
}
