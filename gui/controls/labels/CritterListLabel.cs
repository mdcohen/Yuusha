using System;
using System.Timers;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class CritterListLabel : Label
    {
        private bool m_centerCell;
        private Character m_critter;
        private DropDownMenu m_dropDownMenu;

        public Character Critter
        {
            get { return m_critter; }
            set { m_critter = value; }
        }

        public bool CenterCell
        {
            get { return m_centerCell; }
            set { m_centerCell = value; }
        }

        public DropDownMenu DropDownMenu
        {
            get { return m_dropDownMenu; }
            set { m_dropDownMenu = value; }
        }

        public CritterListLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner,  rectangle, text, textColor, visible,
            disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha,
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
            if (!this.IsVisible || this.IsDisabled || this.Critter == null)
            {
                if (this.DropDownMenu != null)
                    this.DropDownMenu = null;
            }

            base.Update(gameTime);

            if (this.Border != null)
            {
                this.Border.IsVisible = false;

                if (this.Critter != null && GameHUD.CurrentTarget != null && this.Critter.ID == GameHUD.CurrentTarget.ID)
                    this.Border.IsVisible = true;
            }

            if (this.DropDownMenu != null)
                this.DropDownMenu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.DropDownMenu != null)
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
            this.DropDownMenu = null;

            base.OnClientResize(prev, now, ownerOverride);            
        }

        protected override void OnMouseDown(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && (this.DropDownMenu == null || !this.DropDownMenu.IsVisible))
            {
                if (this.Critter != null && this.Critter != GameHUD.CurrentTarget)
                    Events.RegisterEvent(Events.EventName.Target_Select, this.Critter);
            }
            else if (ms.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (this.Critter != null && this.DropDownMenu == null)
                {
                    Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 200, 100);

                    // readjust Y if out of client width bounds
                    if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;
                    
                    GuiManager.CurrentSheet.CreateDropDownMenu(this.Name + "DropDownMenu", this, "", dropDownRectangle, true,
                        this.Font, new VisualKey("WhiteSpace"), Client.UserSettings.ColorDropDownMenu, this.VisualAlpha, true, Map.Direction.Northwest, 5);

                    this.DropDownMenu.HasFocus = true;

                    // add drop down menu items
                    this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem1, this.Name, new VisualKey("WhiteSpace"), "Attack_Critter");
                    this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem2, this.Name, new VisualKey("WhiteSpace"), "Attack_Critter");
                    this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem3, this.Name, new VisualKey("WhiteSpace"), "Attack_Critter");
                    this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem4, this.Name, new VisualKey("WhiteSpace"), "Attack_Critter");
                    this.DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem5, this.Name, new VisualKey("WhiteSpace"), "Attack_Critter");
                }
            }
        }
    }
}
