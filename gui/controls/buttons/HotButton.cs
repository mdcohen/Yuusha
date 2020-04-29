using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class HotButton : Button
    {
        public int FadeIncrement = 10;
        public double FadeDelay = .035;
        readonly int m_originalVisualAlpha = 255;

        public HotButton(string name, string owner, Rectangle rectangle, string text, bool textVisible,
            Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor,
            byte visualAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment,
            int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command, string popUpText)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, popUpText, Client.ClientSettings.DefaultOnClickSound)
        {
            m_originalVisualAlpha = visualAlpha;
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (m_containsMousePointer)
            {
                bool controlDown = ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
                bool altDown = ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt);
                MouseState ms = GuiManager.MouseState;

                if ((controlDown || altDown) && ms.LeftButton != ButtonState.Pressed)
                {
                    if(Text.Length > 0)
                        TextCue.AddMouseCursorTextCue(Text, Client.ClientSettings.HotButtonPopUpText_ForeColor, Client.ClientSettings.HotButtonPopUpText_BackColor, Client.ClientSettings.HotButtonPopUpText_BackColorAlpha, Client.ClientSettings.DefaultPopUpFont);
                    return true;
                }
                else if(controlDown && altDown && ms.LeftButton == ButtonState.Pressed)
                {
                    Text = ""; // Hmm. This is how we bypass drawing a cleared HotButton.
                    Command = "";
                    VisualKey = "WhiteSpace";

                    TextCue.ClearMouseCursorTextCue();

                    TextCue.AddClientInfoTextCue("Hot Button Cleared", Color.LimeGreen, Color.Black, 1000);

                    Events.StoreHotButtons(Name.ToLower().Contains("horizontal"), GuiManager.GetControl(Owner) as Window);
                    Character.Settings.Save();

                    return true;
                }
            }

            return base.OnKeyDown(ks);
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            if (Owner.Contains("HotButtonWindow") && !string.IsNullOrEmpty(Text))
            {
                TextCue.AddMouseCursorTextCue(Text, Client.ClientSettings.HotButtonPopUpText_ForeColor, Client.ClientSettings.HotButtonPopUpText_BackColor, Client.ClientSettings.HotButtonPopUpText_BackColorAlpha, Client.ClientSettings.DefaultPopUpFont);
            }
            else if(Owner.Contains("TalentsWindow") && !string.IsNullOrEmpty(PopUpText))
            {
                TextCue.AddMouseCursorTextCue(Text, Client.ClientSettings.TalentsHotButtonPopUpText_ForeColor, Client.ClientSettings.TalentsHotButtonPopUpText_BackColor, Client.ClientSettings.TalentsHotButtonPopUpText_BackColorAlpha, Client.ClientSettings.DefaultPopUpFont);
            }
            else if (!string.IsNullOrEmpty(PopUpText))
                TextCue.AddMouseCursorTextCue(PopUpText, Client.ClientSettings.ColorDefaultPopUpFore, Client.ClientSettings.ColorDefaultPopUpBack, Client.ClientSettings.DefaultPopUpBackAlpha, Client.ClientSettings.DefaultPopUpFont);

            if (Border == null)
                GuiManager.GenericSheet.CreateSquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Color.OldLace, 255);

            if (Border != null)
                Border.IsVisible = true;            
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
                Border.IsVisible = false;

            TextCue.ClearMouseCursorTextCue();
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (IsDisabled || !IsVisible) return;

            if (GuiManager.DraggedControl != null && GuiManager.DraggedControl.Name == Owner)
                return;

            bool controlDown = GuiManager.KeyboardState.IsKeyDown(Keys.LeftControl) || GuiManager.KeyboardState.IsKeyDown(Keys.RightControl);
            bool altDown = GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt) || GuiManager.KeyboardState.IsKeyDown(Keys.RightAlt);

            if ((Owner.Contains("HotButtonWindow") || !altDown) && !controlDown)
                base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (Command.Length <= 0)
                {
                    if (Owner.Contains("HotButtonWindow") && GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl != null &&
                        GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl.Name.ToLower().Contains("hotbutton") && GameHUD.DraggedSpell != null)
                    {
                        VisualKey = GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl.VisualKey;
                        Text = GameHUD.DraggedSpell.Name;
                        Command = GameHUD.DraggedSpell.Incantation;

                        // Not sure if to call Character_Save event here
                        Events.StoreHotButtons(Name.ToLower().Contains("horizontal"), GuiManager.GetControl(Owner) as Window);
                        Character.Settings.Save();

                        GuiManager.Dispose(GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl);
                        GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl = null;
                        GameHUD.DraggedSpell = null;
                    }

                    return;
                }
                else
                {
                    base.OnMouseDown(ms);
                }

                m_visualAlpha = 40;
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                if (!Owner.Contains("HotButtonWindow"))
                {
                    // SpellbookWindow.
                    if(Owner.ToLower().Contains("spellbookwindow"))// || Owner.ToLower().Contains("spellringwindow"))
                    {
                        if (!m_locked)
                        {
                            MouseCursor cursor = GuiManager.Cursors[GuiManager.GenericSheet.Cursor];
                            if (cursor.DraggedControl != null)
                                return;
                            else
                            {
                                Label label = new Label(Name + "DraggingLabel", "", new Rectangle(cursor.Position.X + 1, cursor.Position.Y + 1, Width, Height), Text, TextColor, true,
                                    false, Font, new VisualKey(VisualKey), TintColor, 255, 255, TextAlignment, XTextOffset, YTextOffset, "", "", new List<Enums.EAnchorType>(), "");
                                GuiManager.GenericSheet.AddControl(label);
                                SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, Color.PaleGreen, 255);
                                GuiManager.GenericSheet.AddControl(border);
                                cursor.DraggedControl = label;

                                //if (Owner.ToLower().Contains("spellbookwindow"))
                                //{
                                    if (Name.ToLower().Contains("right"))
                                        GameHUD.DraggedSpell = (GuiManager.GetControl("SpellbookWindow") as SpellBookWindow).RightPageSpell;
                                    else if (Name.ToLower().Contains("left"))
                                        GameHUD.DraggedSpell = (GuiManager.GetControl("SpellbookWindow") as SpellBookWindow).LeftPageSpell;
                                //}
                                //else if(Owner.ToLower().Contains("spellringwindow"))
                                //{
                                //    GameHUD.DraggedSpell = World.GetSpellByName(PopUpText);
                                //}
                            }

                            //m_originalPosition = new Point(Position.X, Position.Y);

                            // puts the control dead center under mouse cursor
                            //Position = new Point(cursor.Position.X - 10, cursor.Position.Y - 10);
                            //Position = new Point(cursor.Position.X - (this.m_rectangle.Width / 2), cursor.Position.Y - (this.m_rectangle.Width / 2));

                            // Attach to cursor.
                            //m_draggingToDrop = true;
                        }
                    }

                    return;
                }
                else
                {
                    if (GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl != null && GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl.Name.ToLower().Contains("hotbutton") && GameHUD.DraggedSpell != null)
                    {
                        // Decision time: clear the dragged control, or allow it to set

                        VisualKey = GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl.VisualKey;
                        Text = GameHUD.DraggedSpell.Name;
                        Command = GameHUD.DraggedSpell.Incantation;

                        // Not sure if to call Character_Save event here
                        Events.StoreHotButtons(Name.ToLower().Contains("horizontal"), GuiManager.GetControl(Owner) as Window);
                        Character.Settings.Save();

                        GuiManager.Dispose(GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl);
                        GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl = null;
                        GameHUD.DraggedSpell = null;
                        GuiManager.AwaitMouseButtonRelease = true;
                        return;
                    }
                }

                if (GuiManager.AwaitMouseButtonRelease)
                    return;

                //if(!Client.IsFullScreen)
                //{
                //    TextCue.AddClientInfoTextCue("Hot Button Edit Mode requires full screen display.",
                //        TextCue.TextCueTag.None, Color.Red, Color.Black, 255, 2000, false, false, true);
                //    return;
                //}

                try
                {
                    Sheet sheet = GuiManager.Sheets["HotButtonEditMode"];

                    if (sheet != null)
                    {
                        if (sheet["HotButtonEditWindow"] is HotButtonEditWindow window)
                        {
                            // Register game state change event.
                            Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.HotButtonEditMode);
                            // Set edit mode variables.
                            window.SelectedHotButton = Name;

                            try
                            {
                                window.IconImagePrefix = VisualKey.Substring(0, this.VisualKey.IndexOf("_"));
                            }
                            catch
                            {
                                window.IconImagePrefix = "hotbutton";
                            }

                            window.SelectedIconLabel = window["HotButtonEditWindowSelectedIconLabel"] as Label;
                            window.SelectedIconLabel.VisualKey = VisualKey;
                            if (window.SelectedIconLabel.VisualKey == "")
                                window.SelectedIconLabel.VisualKey = "WhiteSpace";
                            window.SelectedVisualKey = VisualKey;
                            if (window.SelectedVisualKey == "")
                                window.SelectedVisualKey = "WhiteSpace";
                            window.OriginatingWindow = Owner;

                            // Set textbox text.
                            window["HotButtonEditWindowTextBox"].Text = Text;
                            (window["HotButtonEditWindowTextBox"] as TextBox).SelectAll();
                            (window["HotButtonEditWindowTextBox"] as TextBox).DeselectText();

                            window["HotButtonEditWindowCommandTextBox"].Text = Command;

                            window.ForceMaximize(); // fill the screen

                            // Create labels for choosing a hotbutton icon.
                            window.CreateIconSelectionButtons();

                            // macros and hotkeys are part of the generic sheet, always visible -- hide them when in edit mode
                            //(GuiManager.CurrentSheet[this.Owner] as Window).IsVisible = false;

                            window.IsVisible = true;
                        }
                    }
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_containsMousePointer && Border != null)
                Border.IsVisible = true;
            else if (!m_containsMousePointer && Border != null)
                Border.IsVisible = false;

            if (m_visualAlpha < m_originalVisualAlpha)
            {
                //Decrement the delay by the number of seconds that have elapsed since
                //the last time that the Update method was called
                FadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, then it is time to 
                //fade in/fade out the image a little bit more.
                if (FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    FadeDelay = .035;

                    //Increment/Decrement the fade value for the image
                    m_visualAlpha += FadeIncrement;

                    //If the AlphaValue is equal or above the max Alpha value or
                    //has dropped below or equal to the min Alpha value, then 
                    //reverse the fade
                    if (m_visualAlpha >= 255 || m_visualAlpha > m_originalVisualAlpha)// || m_visualAlpha <= 0)
                    {
                        m_visualAlpha = m_originalVisualAlpha;
                        //mFadeIncrement *= -1;
                    }
                }
            }

            base.Update(gameTime);

            if (Border != null) Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            if(VisualKey == "WhiteSpace" && GuiManager.GetControl(Owner) is HotButtonEditWindow)
            {
                if (Border != null && Border.IsVisible) Border.Draw(gameTime);
                return;
            }

            // don't draw HotButtons for SpellringWindow if ALT key down.
            if (Owner.ToLower().Contains("spellringwindow") && (GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt) || GuiManager.KeyboardState.IsKeyDown(Keys.RightAlt)))
                return;

            Color color = new Color(m_tintColor, (byte)MathHelper.Clamp(m_visualAlpha, 0, 255));

            if (string.IsNullOrEmpty(Command) && Owner.Contains("HotButtonWindow"))
                color = new Color(Color.Black, 125);

            //if(IsDisabled) color = new Color(ColorDisabledStandard, (byte)MathHelper.Clamp(m_visualAlpha, 0, 255));

            if (GuiManager.Visuals.ContainsKey(m_visualKey.Key))
            {
                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];

                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(color.R, color.G, color.B, color.A));
            }

            if (!string.IsNullOrEmpty(m_text) && IsTextVisible)
            {
                if (BitmapFont.ActiveFonts.ContainsKey(Font))
                {
                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                    // draw string in textbox, using x and y text offsets to create new rectangle
                    Rectangle rect = new Rectangle(m_rectangle.X + XTextOffset, m_rectangle.Y + YTextOffset, m_rectangle.Width, m_rectangle.Height);
                    if (!m_disabled)
                    {
                        // change color of text if mouse over text color is not null
                        if (m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
                        {
                            BitmapFont.ActiveFonts[Font].TextBox(rect, m_textOverColor, m_text);
                        }
                        else
                        {
                            BitmapFont.ActiveFonts[Font].TextBox(rect, m_textColor, m_text);
                        }
                    }
                    else
                    {
                        BitmapFont.ActiveFonts[Font].TextBox(rect, ColorDisabledStandard, m_text);
                    }
                }
                else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for Button [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
            }

            if (Border != null) Border.Draw(gameTime);
        }
    }
}
