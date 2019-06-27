using System;
using System.Timers;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class CritterListLabel : Label
    {
        public Character Critter
        { get; set; }

        public bool CenterCell
        { get; set; }

        public DropDownMenu DropDownMenu
        { get; set; }

        public CritterListLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner,  rectangle, text, textColor, visible,
            disabled, font, visualKey, tintColor, visualAlpha, textAlpha,
            textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent,
            cursorOverride, anchors, popUpText)
        {
        }

        public override bool MouseHandler(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            if (this.DropDownMenu != null)
                this.DropDownMenu.MouseHandler(ms);

            return base.MouseHandler(ms);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible || IsDisabled || Critter == null)
            {
                if (DropDownMenu != null)
                    DropDownMenu = null;
            }

            base.Update(gameTime);

            if (Border != null)
            {
                Border.IsVisible = false;

                if (Critter != null && GameHUD.CurrentTarget != null && this.Critter.ID == GameHUD.CurrentTarget.ID)
                    Border.IsVisible = true;
            }

            if (DropDownMenu != null)
                DropDownMenu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (DropDownMenu != null)
                DropDownMenu.Draw(gameTime);
        }

        protected override void OnDoubleLeftClick()
        {
            if (!Client.HasFocus) return;

            if (this.Critter != null)
            {
                GameHUD.CurrentTarget = this.Critter;
                
                Events.RegisterEvent(Events.EventName.Attack_Critter, this);
            }

            m_leftClickCount = 0;
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            DropDownMenu = null;

            base.OnClientResize(prev, now, ownerOverride);            
        }

        protected override void OnMouseDown(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && (DropDownMenu == null || !DropDownMenu.IsVisible))
            {
                if (Critter != null && Critter != GameHUD.CurrentTarget)
                    Events.RegisterEvent(Events.EventName.Target_Select, Critter);
            }
            else if (ms.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (Critter != null && DropDownMenu == null)
                {
                    Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 200, 100); // default height for 5 drop down menu items

                    // readjust Y if out of client width bounds
                    if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                    GuiManager.Sheets[Sheet].CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                        Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, VisualAlpha, true, Map.Direction.Northwest, 5);

                    DropDownMenu.Border = new SquareBorder(DropDownMenu.Name + "Border", DropDownMenu.Name, Client.ClientSettings.DropDownMenuBorderWidth, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255)
                    {
                        IsVisible = true
                    };

                    DropDownMenu.HasFocus = true;
                    int height = 0;
                    // add drop down menu items
                    if (Character.Settings.CritterListDropDownMenuItem1 != "")
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem1, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (Character.Settings.CritterListDropDownMenuItem2 != "")
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem2, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (Character.Settings.CritterListDropDownMenuItem3 != "")
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem3, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (Character.Settings.CritterListDropDownMenuItem4 != "")
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem4, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (Character.Settings.CritterListDropDownMenuItem5 != "")
                    {
                        height += 20;
                        this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem5, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }

                    DropDownMenu.Height = height;
                }
            }
        }
    }
}
