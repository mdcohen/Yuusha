using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        public PercentageBarLabel HealthBar
        { get; private set; }

        public Label RightHandItemLabel
        { get; private set; }
        public Label LeftHandItemLabel
        { get; private set; }
        public Label ArmorItemLabel
        { get; private set; }

        private bool m_mouseReleased = true;

        public CritterListLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner,  rectangle, text, textColor, visible,
            disabled, font, visualKey, tintColor, visualAlpha, textAlpha,
            textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent,
            cursorOverride, anchors, popUpText)
        {
            int x = rectangle.X;
            int y = rectangle.Y + rectangle.Height;

            if(GuiManager.GetControl(owner) is Window w)
            {
                x = w.Position.X + x;
                y = w.Position.Y + y;
            }

            HealthBar = new PercentageBarLabel(name + "PercentageBarLabel", name, new Rectangle(x, y, rectangle.Width, 3), "", Color.White,
                true, false, Font, new VisualKey("WhiteSpace"), Color.Black, 0, 150, BitmapFont.TextAlignment.Center, 0, 0, "", "", anchors, "", false);
            HealthBar.MidLabel = new Label(HealthBar.Name + "ForeLabel", HealthBar.Name, new Rectangle(x, y, rectangle.Width, 3), "", Color.White,
                true, false, Font, new VisualKey("WhiteSpace"), Color.Red, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", anchors, "");

            RightHandItemLabel = new Label(name + "RightHandItemLabel", name, new Rectangle(Width - (Height * 3), Position.Y, Height, Height), "", Color.White, false, false,
                font, new VisualKey("WhiteSpace"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", anchors, "");
            LeftHandItemLabel = new Label(name + "LeftHandItemLabel", name, new Rectangle(Width - (Height * 2), Position.Y, Height, Height), "", Color.White, false, false,
                font, new VisualKey("WhiteSpace"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", anchors, "");
            ArmorItemLabel = new Label(name + "RightHandItemLabel", name, new Rectangle(Width - Height, Position.Y, Height, Height), "", Color.White, false, false,
                font, new VisualKey("WhiteSpace"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", anchors, "");

        }

        public override bool MouseHandler(MouseState ms)
        {
            if (DropDownMenu != null)
                DropDownMenu.MouseHandler(ms);

            if (HealthBar != null)
                HealthBar.MouseHandler(ms);

            if (RightHandItemLabel != null)
                RightHandItemLabel.MouseHandler(ms);

            if (LeftHandItemLabel != null)
                LeftHandItemLabel.MouseHandler(ms);

            if (ArmorItemLabel != null)
                ArmorItemLabel.MouseHandler(ms);

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

            if (Critter != null && Text == "")
                Critter = null;

            if (Border != null)
            {
                Border.Update(gameTime);

                Border.IsVisible = false;

                if (Critter != null && GameHUD.CurrentTarget != null && Critter.UniqueID == GameHUD.CurrentTarget.UniqueID)
                    Border.IsVisible = true;
            }

            if (HealthBar != null)
            {
                HealthBar.Position = new Point(Position.X, Position.Y + Height);

                if (Critter != null)
                {
                    HealthBar.Percentage = Critter.healthPercentage;
                    //if (HealthBar.Percentage < 100)
                    //    HealthBar.ForeLabel.Text = string.Format("{0:0.00}%", HealthBar.Percentage);
                    //else HealthBar.ForeLabel.Text = "100%";
                }

                HealthBar.Update(gameTime);
            }

            if (DropDownMenu != null)
                DropDownMenu.Update(gameTime);

            if (RightHandItemLabel != null)
            {
                RightHandItemLabel.Position = new Point(Position.X + Width - (Height * 3), Position.Y);
                if(Critter != null) RightHandItemLabel.Command = "look at " + Critter.UniqueID + "'s right";
                RightHandItemLabel.Update(gameTime);                
            }

            if(LeftHandItemLabel != null)
            {
                LeftHandItemLabel.Position = new Point(Position.X + Width - (Height * 2), Position.Y);
                if (Critter != null) LeftHandItemLabel.Command = "look at " + Critter.UniqueID + "'s left";
                LeftHandItemLabel.Update(gameTime);
            }

            if(ArmorItemLabel != null)
            {
                ArmorItemLabel.Position = new Point(Position.X + Width - Height, Position.Y);
                // RightHandItemLabel.Command = "look at " + Critter.UniqueID + "'s right";
                ArmorItemLabel.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime);

            if (HealthBar != null)
                HealthBar.Draw(gameTime);

            if (RightHandItemLabel != null)
                RightHandItemLabel.Draw(gameTime);

            if (LeftHandItemLabel != null)
                LeftHandItemLabel.Draw(gameTime);

            if (ArmorItemLabel != null)
                ArmorItemLabel.Draw(gameTime);

            if (DropDownMenu != null)
                DropDownMenu.Draw(gameTime);

            if (Border != null)
                Border.Draw(gameTime);
        }

        protected override void OnDoubleLeftClick()
        {
            if (!Client.HasFocus) return;

            if (Critter != null)
            {
                GameHUD.CurrentTarget = Critter;
                
                Events.RegisterEvent(Events.EventName.Attack_Critter, this);
            }

            Audio.AudioManager.PlaySoundEffect(Client.ClientSettings.DefaultOnClickSound);

            m_leftClickCount = 0;
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            DropDownMenu = null;
            base.OnClientResize(prev, now, ownerOverride);
            if (HealthBar != null) HealthBar.OnClientResize(prev, now, true);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed && (DropDownMenu == null || !DropDownMenu.IsVisible))
            {
                if (m_mouseReleased)
                {
                    if (Critter != null && RightHandItemLabel.IsVisible && RightHandItemLabel.Contains(ms.Position))
                    {
                        IO.Send("look at " + Critter.UniqueID + "'s right");
                        m_mouseReleased = false;
                    }
                    else if (Critter != null && LeftHandItemLabel.IsVisible && LeftHandItemLabel.Contains(ms.Position))
                    {
                        IO.Send("look at " + Critter.UniqueID + "'s left");
                        m_mouseReleased = false;
                    }
                    else if (Critter != null && ArmorItemLabel.IsVisible && ArmorItemLabel.Contains(ms.Position))
                    {
                        IO.Send("look closely at " + Critter.UniqueID);
                        m_mouseReleased = false;
                    }
                }

                if (GameHUD.CurrentTarget == null || (Critter != null && Critter != GameHUD.CurrentTarget))
                    Events.RegisterEvent(Events.EventName.Target_Select, Critter);
                
            }
            else if (ms.RightButton == ButtonState.Pressed)
            {
                if (Critter != null && DropDownMenu == null)
                {
                    Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 200, 100); // default height for 5 drop down menu items

                    // readjust Y if out of client width bounds
                    if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                    GuiManager.Sheets[Sheet].CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                        Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, VisualAlpha, true, Map.Direction.Northwest, 5);

                    // Uncomment below line to change border for this button while drop down menu is visible
                    //Border = new SquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);
                    DropDownMenu.Border = new SquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);

                    DropDownMenu.HasFocus = true;
                    int height = 0;
                    // add drop down menu items
                    if (!string.IsNullOrEmpty(Character.Settings.CritterListDropDownMenuItem1))
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem1, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (!string.IsNullOrEmpty(Character.Settings.CritterListDropDownMenuItem2))
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem2, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (!string.IsNullOrEmpty(Character.Settings.CritterListDropDownMenuItem3))
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem3, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (!string.IsNullOrEmpty(Character.Settings.CritterListDropDownMenuItem4))
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem4, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }
                    if (!string.IsNullOrEmpty(Character.Settings.CritterListDropDownMenuItem5))
                    {
                        height += 20;
                        DropDownMenu.AddDropDownMenuItem(Character.Settings.CritterListDropDownMenuItem5, DropDownMenu.Name, new VisualKey("WhiteSpace"), "Attack_Critter", "", false);
                    }

                    DropDownMenu.Height = height;
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            base.OnMouseRelease(ms);

            m_mouseReleased = true;
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            if(RightHandItemLabel != null && RightHandItemLabel.IsVisible && RightHandItemLabel.Contains(ms.Position))
            {
                PopUpWindow.CreateSquareIconPopUpWindow(RightHandItemLabel, 60);
            }
            else if (LeftHandItemLabel != null && LeftHandItemLabel.IsVisible && LeftHandItemLabel.Contains(ms.Position))
            {
                PopUpWindow.CreateSquareIconPopUpWindow(LeftHandItemLabel, 60);
            }
        }

        public override bool KeyboardHandler(KeyboardState ks)
        {
            if (DropDownMenu != null)
                return DropDownMenu.KeyboardHandler(ks);

            return false;
        }
    }
}
